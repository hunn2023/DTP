import type { Carrier } from '@/features/master-data/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import type { FormFieldOption } from '@/modules/crud/form/types'

type CarrierDtoRaw = Record<string, unknown>

export type CarrierDto = {
  id: string
  name: string
  slug: string
  code: string
  countryId: string
  logoUrl: string
  isActive: boolean
  sortOrder: number
}

export type CarrierPayload = {
  name: string
  slug: string
  code?: string
  countryId: string
  logoUrl?: string
  sortOrder: number
}

export type CarrierUpdatePayload = CarrierPayload & {
  isActive: boolean
}

export type PagedCarriersDto = {
  items: CarrierDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: CarrierDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: CarrierDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: CarrierDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: CarrierDtoRaw): CarrierDto {
  return {
    id: readString(raw, 'id', 'Id'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    code: readString(raw, 'code', 'Code'),
    countryId: readString(raw, 'countryId', 'CountryId'),
    logoUrl: readString(raw, 'logoUrl', 'LogoUrl'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: CarrierDto, countryName = ''): Carrier {
  return {
    id: dto.id,
    name: dto.name,
    slug: dto.slug,
    code: dto.code,
    countryId: dto.countryId,
    countryName,
    logoUrl: dto.logoUrl,
    isActive: dto.isActive,
    sortOrder: dto.sortOrder,
  }
}

function normalizePaged(raw: Record<string, unknown>): PagedCarriersDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as CarrierDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export const CARRIER_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type CarriersPageResult = {
  items: Carrier[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<CarriersPageResult>>()

export async function fetchCarriersPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
  countryNameById?: Map<string, string>,
): Promise<CarriersPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminCarriers(pageIndex, pageSize, keyword).then((paged) => ({
    items: paged.items.map((dto) =>
      mapDto(dto, countryNameById?.get(dto.countryId) ?? ''),
    ),
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

export async function fetchAdminCarriers(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedCarriersDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.adminCarriers, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(raw)
}

export async function fetchCarrierOptions(): Promise<FormFieldOption[]> {
  const paged = await fetchAdminCarriers(1, 500)
  return paged.items.map((item) => ({
    value: item.id,
    label: item.name,
  }))
}

export async function createCarrier(payload: CarrierPayload): Promise<string> {
  const raw = await httpPost<string>(API_PATHS.adminCarriers, payload)
  return typeof raw === 'string' ? raw : String(raw)
}

export async function updateCarrier(id: string, payload: CarrierUpdatePayload): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminCarriers}/${id}`, payload)
}

export async function deleteCarrier(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminCarriers}/${id}`)
}

export async function fetchCarriers(countryNameById?: Map<string, string>): Promise<Carrier[]> {
  const result = await fetchCarriersPage(1, 500, undefined, countryNameById)
  return result.items
}
