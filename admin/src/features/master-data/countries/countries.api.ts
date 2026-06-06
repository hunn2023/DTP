import type { Country } from '@/features/master-data/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type CountryDtoRaw = Record<string, unknown>

export type CountryDto = {
  id: string
  name: string
  slug: string
  code: string
  flagUrl: string
  isActive: boolean
  sortOrder: number
}

export type CountryPayload = {
  name: string
  slug: string
  code: string
  flagUrl?: string
  sortOrder: number
}

export type CountryUpdatePayload = CountryPayload & {
  isActive: boolean
}

export type PagedCountriesDto = {
  items: CountryDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: CountryDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: CountryDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: CountryDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: CountryDtoRaw): CountryDto {
  return {
    id: readString(raw, 'id', 'Id'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    code: readString(raw, 'code', 'Code'),
    flagUrl: readString(raw, 'flagUrl', 'FlagUrl'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: CountryDto): Country {
  return {
    id: dto.id,
    name: dto.name,
    slug: dto.slug,
    isoCode: dto.code,
    flagUrl: dto.flagUrl,
    isActive: dto.isActive,
    sortOrder: dto.sortOrder,
  }
}

function normalizePaged(raw: Record<string, unknown>): PagedCountriesDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as CountryDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export const COUNTRY_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type CountriesPageResult = {
  items: Country[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<CountriesPageResult>>()

export async function fetchCountriesPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<CountriesPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminCountries(pageIndex, pageSize, keyword).then((paged) => ({
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

export async function fetchAdminCountries(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedCountriesDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.adminCountries, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(raw)
}

export async function createCountry(payload: CountryPayload): Promise<string> {
  return httpPost<string>(API_PATHS.adminCountries, payload)
}

export async function updateCountry(id: string, payload: CountryUpdatePayload): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminCountries}/${id}`, payload)
}

export async function deleteCountry(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminCountries}/${id}`)
}

export async function fetchCountries(): Promise<Country[]> {
  const result = await fetchCountriesPage(1, 500)
  return result.items
}
