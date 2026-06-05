import type { Category } from '@/features/master-data/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type CategoryDtoRaw = Record<string, unknown>

export type CategoryDto = {
  id: string
  name: string
  slug: string
  code?: string | null
  description?: string | null
  isActive: boolean
  sortOrder: number
}

export type CategoryPayload = {
  name: string
  code?: string
  slug: string
  description?: string
  isActive: boolean
  sortOrder: number
}

export type PagedCategoriesDto = {
  items: CategoryDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: CategoryDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: CategoryDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: CategoryDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: CategoryDtoRaw): CategoryDto {
  return {
    id: readString(raw, 'id', 'Id'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    code: readString(raw, 'code', 'Code') || null,
    description: readString(raw, 'description', 'Description') || null,
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: CategoryDto): Category {
  return {
    id: dto.id,
    name: dto.name,
    slug: dto.slug,
    code: dto.code ?? '',
    icon: '',
    description: dto.description ?? '',
    isActive: dto.isActive,
    sortOrder: dto.sortOrder,
  }
}

function normalizePaged(raw: Record<string, unknown>): PagedCategoriesDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as CategoryDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
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

/** Admin GET hiện trả [] — dùng public API phân trang (FE-only). */
export async function fetchCategoriesPage(
  pageIndex = 1,
  pageSize = 10,
): Promise<CategoriesPageResult> {
  const key = `${pageIndex}:${pageSize}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchPublicCategories(pageIndex, pageSize).then((paged) => ({
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

export async function fetchPublicCategories(
  pageIndex = 1,
  pageSize = 10,
): Promise<PagedCategoriesDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.publicCategories, {
    params: { pageIndex, pageSize },
  })
  return normalizePaged(raw)
}

export async function createCategory(payload: CategoryPayload): Promise<Category> {
  const raw = await httpPost<CategoryDtoRaw>(API_PATHS.adminCategories, payload)
  return mapDto(normalizeDto(raw))
}

export async function updateCategory(
  id: string,
  payload: Omit<CategoryPayload, 'slug'>,
): Promise<Category> {
  const raw = await httpPut<CategoryDtoRaw>(`${API_PATHS.adminCategories}/${id}`, payload)
  return mapDto(normalizeDto(raw))
}

export async function deleteCategory(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminCategories}/${id}`)
}
