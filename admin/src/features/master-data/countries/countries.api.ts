import type { Country } from '@/features/master-data/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readBool,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPostForm, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type CountryDto = {
  id: string
  name: string
  slug: string
  code: string
  flagUrl: string
  region: string
  description: string
  isActive: boolean
  sortOrder: number
}

export type CountryCreatePayload = {
  code: string
  name: string
  slug: string
  region?: string
  description?: string
  sortOrder: number
  isActive: boolean
}

export type CountryUpdatePayload = CountryCreatePayload

export type PagedCountriesDto = {
  items: CountryDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function normalizeDto(raw: Raw): CountryDto {
  return {
    id: readString(raw, 'id', 'Id'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    code: readString(raw, 'code', 'Code'),
    flagUrl: readString(raw, 'flagUrl', 'FlagUrl'),
    region: readString(raw, 'region', 'Region'),
    description: readString(raw, 'description', 'Description'),
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
    region: dto.region,
    description: dto.description,
    isActive: dto.isActive,
    sortOrder: dto.sortOrder,
  }
}

function parseCreatedId(raw: unknown): string {
  if (typeof raw === 'string') return raw
  if (typeof raw === 'object' && raw !== null) {
    return readString(raw as Raw, 'id', 'Id')
  }
  return String(raw ?? '')
}

export const COUNTRY_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type CountriesPageResult = {
  items: Country[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<CountriesPageResult>>()

let cachedCountries: Country[] | null = null
let inflightCountries: Promise<Country[]> | null = null

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
  const data = await httpGet<Raw>(API_PATHS.adminCountries, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(data, normalizeDto)
}

export async function createCountry(payload: CountryCreatePayload): Promise<string> {
  const id = await httpPost<unknown>(API_PATHS.adminCountries, payload)
  invalidateCountriesCache()
  return parseCreatedId(id)
}

export async function updateCountry(id: string, payload: CountryUpdatePayload): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminCountries}/${id}`, payload)
  invalidateCountriesCache()
}

export async function uploadCountryFlag(countryId: string, file: File): Promise<Country> {
  const formData = new FormData()
  formData.append('File', file)

  const dto = await httpPostForm<Raw>(`${API_PATHS.adminCountries}/upload`, formData, {
    params: { countryId },
  })
  invalidateCountriesCache()
  return mapDto(normalizeDto(dto))
}

export async function deleteCountry(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminCountries}/${id}`)
  invalidateCountriesCache()
}

export async function fetchCountries(): Promise<Country[]> {
  if (cachedCountries) return cachedCountries
  if (inflightCountries) return inflightCountries

  inflightCountries = fetchCountriesPage(1, 500)
    .then((result) => {
      cachedCountries = result.items
      return result.items
    })
    .finally(() => {
      inflightCountries = null
    })

  return inflightCountries
}

export function invalidateCountriesCache(): void {
  cachedCountries = null
}
