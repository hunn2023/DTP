import type { CatalogProduct } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import {
  normalizePaged,
  readBool,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import type { FormFieldOption } from '@/modules/crud/form/types'

type Raw = Record<string, unknown>

export type ProductCreatePayload = {
  code?: string
  name: string
  slug: string
  categoryId: string
  countryId?: string
  shortDescription?: string
  description?: string
  locationText?: string
  isFeatured?: boolean
  isHot?: boolean
  sortOrder: number
  isActive: boolean
}

export type ProductUpdatePayload = ProductCreatePayload

export const PRODUCT_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type ProductsPageResult = {
  items: CatalogProduct[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<ProductsPageResult>>()

let cachedProductOptions: FormFieldOption[] | null = null
let inflightProductOptions: Promise<FormFieldOption[]> | null = null

function normalizeProductDto(raw: Raw): CatalogProduct {
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    categoryId: readString(raw, 'categoryId', 'CategoryId'),
    categoryName: readString(raw, 'categoryName', 'CategoryName'),
    countryId: readString(raw, 'countryId', 'CountryId'),
    countryName: readString(raw, 'countryName', 'CountryName'),
    shortDescription: readString(raw, 'shortDescription', 'ShortDescription'),
    description: readString(raw, 'description', 'Description'),
    locationText: readString(raw, 'locationText', 'LocationText'),
    thumbnailUrl: readString(raw, 'thumbnailUrl', 'ThumbnailUrl'),
    isFeatured: readBool(raw, 'isFeatured', 'IsFeatured'),
    isHot: readBool(raw, 'isHot', 'IsHot'),
    soldCount: readNumber(raw, 'soldCount', 'SoldCount'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function parseCreatedId(raw: unknown): string {
  if (typeof raw === 'string') return raw
  if (typeof raw === 'object' && raw !== null) {
    return readString(raw as Raw, 'id', 'Id')
  }
  return String(raw ?? '')
}

export function invalidateProductsCache(): void {
  cachedProductOptions = null
  inflightProductOptions = null
}

export type ProductListFilters = {
  keyword?: string
  categoryId?: string
  countryId?: string
  carrierId?: string
  isActive?: boolean
}

export async function fetchProductsPage(
  pageIndex = 1,
  pageSize = 10,
  filters: ProductListFilters = {},
): Promise<ProductsPageResult> {
  const key = `${pageIndex}:${pageSize}:${JSON.stringify(filters)}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminProducts(pageIndex, pageSize, filters).then((paged) => ({
    items: paged.items,
    totalCount: paged.totalCount,
    pageIndex: paged.pageIndex,
    pageSize: paged.pageSize,
  }))

  inflightPages.set(key, request)
  try {
    return await request
  } finally {
    inflightPages.delete(key)
  }
}

export async function fetchAdminProducts(
  pageIndex = 1,
  pageSize = 10,
  filters: ProductListFilters = {},
) {
  const data = await httpGet<Raw>(API_PATHS.adminProducts, {
    params: {
      pageIndex,
      pageSize,
      keyword: filters.keyword?.trim() || undefined,
      categoryId: filters.categoryId || undefined,
      countryId: filters.countryId || undefined,
      carrierId: filters.carrierId || undefined,
      isActive: filters.isActive,
    },
  })
  return normalizePaged(data, normalizeProductDto)
}

const inflightProductDetails = new Map<string, Promise<CatalogProduct | null>>()

export async function fetchProductDetail(id: string): Promise<CatalogProduct | null> {
  const inflight = inflightProductDetails.get(id)
  if (inflight) return inflight

  const promise = (async () => {
    try {
      const raw = await httpGet<Raw>(`${API_PATHS.adminProducts}/${id}`)
      return normalizeProductDto(raw)
    } catch {
      return null
    }
  })().finally(() => {
    inflightProductDetails.delete(id)
  })

  inflightProductDetails.set(id, promise)
  return promise
}

export async function fetchProductOptions(): Promise<FormFieldOption[]> {
  if (cachedProductOptions) return cachedProductOptions
  if (inflightProductOptions) return inflightProductOptions

  inflightProductOptions = fetchAdminProducts(1, 500)
    .then((paged) => {
      cachedProductOptions = paged.items.map((item) => ({
        value: item.id,
        label: `${item.code || item.slug} — ${item.name}`,
      }))
      return cachedProductOptions
    })
    .finally(() => {
      inflightProductOptions = null
    })

  return inflightProductOptions
}

export async function createProduct(payload: ProductCreatePayload): Promise<string> {
  const id = await httpPost<unknown>(API_PATHS.adminProducts, payload)
  invalidateProductsCache()
  return parseCreatedId(id)
}

export async function updateProduct(id: string, payload: ProductUpdatePayload): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProducts}/${id}`, payload)
  invalidateProductsCache()
}

export async function deleteProduct(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProducts}/${id}`)
  invalidateProductsCache()
}
