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

export type ProductDto = {
  id: string
  code: string
  name: string
  slug: string
  categoryId: string
  categoryName: string
  shortDescription: string
  description: string
  thumbnailUrl: string
  sortOrder: number
  isActive: boolean
}

export type ProductPayload = {
  code?: string
  name: string
  slug: string
  categoryId: string
  shortDescription?: string
  description?: string
  thumbnailUrl?: string
  sortOrder: number
}

export type ProductUpdatePayload = ProductPayload & {
  isActive: boolean
}

function normalizeProductDto(raw: Raw): ProductDto {
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    categoryId: readString(raw, 'categoryId', 'CategoryId'),
    categoryName: readString(raw, 'categoryName', 'CategoryName'),
    shortDescription: readString(raw, 'shortDescription', 'ShortDescription'),
    description: readString(raw, 'description', 'Description'),
    thumbnailUrl: readString(raw, 'thumbnailUrl', 'ThumbnailUrl'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function mapDto(dto: ProductDto): CatalogProduct {
  return { ...dto }
}

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
    items: paged.items.map(mapDto),
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
  const raw = await httpGet<Raw>(API_PATHS.adminProducts, {
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
  return normalizePaged(raw, normalizeProductDto)
}

export async function fetchProductDetail(id: string): Promise<CatalogProduct | null> {
  try {
    const raw = await httpGet<Raw>(`${API_PATHS.adminProducts}/${id}`)
    return mapDto(normalizeProductDto(raw))
  } catch {
    return null
  }
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

export async function createProduct(payload: ProductPayload): Promise<string> {
  const raw = await httpPost<string | Raw>(API_PATHS.adminProducts, payload)
  invalidateProductsCache()
  if (typeof raw === 'string') return raw
  return readString(raw, 'id', 'Id')
}

export async function updateProduct(id: string, payload: ProductUpdatePayload): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminProducts}/${id}`, payload)
  invalidateProductsCache()
}

export async function deleteProduct(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProducts}/${id}`)
  invalidateProductsCache()
}
