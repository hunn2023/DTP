import type { Category } from '@/features/master-data/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readBool,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

import type { FormFieldOption } from '@/modules/crud/form/types'

type Raw = Record<string, unknown>

export type CategoryDto = {
  id: string
  name: string
  slug: string
  code: string
  isActive: boolean
  sortOrder: number
}

export type CategoryPayload = {
  name: string
  code: string
  slug: string
  isActive: boolean
  sortOrder: number
}

export type CategoryUpdatePayload = CategoryPayload

export type PagedCategoriesDto = {
  items: CategoryDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function normalizeDto(raw: Raw): CategoryDto {
  return {
    id: readString(raw, 'id', 'Id'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    code: readString(raw, 'code', 'Code'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: CategoryDto): Category {
  return {
    id: dto.id,
    name: dto.name,
    slug: dto.slug,
    code: dto.code,
    isActive: dto.isActive,
    sortOrder: dto.sortOrder,
  }
}

export const CATEGORY_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type CategoriesPageResult = {
  items: Category[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<CategoriesPageResult>>()

let cachedCategoryOptions: FormFieldOption[] | null = null
let inflightCategoryOptions: Promise<FormFieldOption[]> | null = null

export function invalidateCategoriesCache(): void {
  cachedCategoryOptions = null
  inflightCategoryOptions = null
}

export async function fetchCategoriesPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<CategoriesPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminCategories(pageIndex, pageSize, keyword).then((paged) => ({
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

export async function fetchAdminCategories(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedCategoriesDto> {
  const data = await httpGet<Raw>(API_PATHS.adminCategories, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(data, normalizeDto)
}

/** Dropdown danh mục — dùng cho Products, EsimPackages... */
export async function fetchCategoryOptions(): Promise<FormFieldOption[]> {
  if (cachedCategoryOptions) return cachedCategoryOptions
  if (inflightCategoryOptions) return inflightCategoryOptions

  inflightCategoryOptions = fetchCategoriesPage(1, 500)
    .then((paged) => {
      cachedCategoryOptions = paged.items.map((item) => ({
        value: item.id,
        label: item.name,
      }))
      return cachedCategoryOptions
    })
    .finally(() => {
      inflightCategoryOptions = null
    })

  return inflightCategoryOptions
}

export async function createCategory(payload: CategoryPayload): Promise<Category> {
  const dto = await httpPost<Raw>(API_PATHS.adminCategories, payload)
  invalidateCategoriesCache()
  return mapDto(normalizeDto(dto))
}

export async function updateCategory(id: string, payload: CategoryUpdatePayload): Promise<Category> {
  const dto = await httpPut<Raw>(`${API_PATHS.adminCategories}/${id}`, payload)
  invalidateCategoriesCache()
  return mapDto(normalizeDto(dto))
}

export async function deleteCategory(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminCategories}/${id}`)
  invalidateCategoriesCache()
}
