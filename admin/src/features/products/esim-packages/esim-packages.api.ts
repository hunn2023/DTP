import type { EsimPackage } from '@/features/products/esim-packages/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type EsimPackageDtoRaw = Record<string, unknown>

export type EsimPackageDto = {
  id: string
  productVariantId: string
  productVariantName: string
  productName: string
  countryId: string
  countryName: string
  carrierId: string
  carrierName: string
  name: string
  slug: string
  dataAmount: number
  dataUnit: string
  validityDays: number
  price: number
  currency: string
  isUnlimited: boolean
  isActive: boolean
  sortOrder: number
}

export type EsimPackageCreatePayload = {
  productVariantId: string
  countryId: string
  carrierId: string
  name: string
  slug: string
  dataAmount: number
  dataUnit: string
  validityDays: number
  price: number
  currency: string
  isUnlimited: boolean
  sortOrder: number
  isActive: boolean
}

export type EsimPackageUpdatePayload = Omit<EsimPackageCreatePayload, 'productVariantId'>

export type PagedEsimPackagesDto = {
  items: EsimPackageDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: EsimPackageDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: EsimPackageDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: EsimPackageDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: EsimPackageDtoRaw): EsimPackageDto {
  return {
    id: readString(raw, 'id', 'Id'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    productVariantName: readString(raw, 'productVariantName', 'ProductVariantName'),
    productName: readString(raw, 'productName', 'ProductName'),
    countryId: readString(raw, 'countryId', 'CountryId'),
    countryName: readString(raw, 'countryName', 'CountryName'),
    carrierId: readString(raw, 'carrierId', 'CarrierId'),
    carrierName: readString(raw, 'carrierName', 'CarrierName'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    dataAmount: readNumber(raw, 'dataAmount', 'DataAmount'),
    dataUnit: readString(raw, 'dataUnit', 'DataUnit'),
    validityDays: readNumber(raw, 'validityDays', 'ValidityDays'),
    price: readNumber(raw, 'price', 'Price'),
    currency: readString(raw, 'currency', 'Currency'),
    isUnlimited: readBool(raw, 'isUnlimited', 'IsUnlimited'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: EsimPackageDto): EsimPackage {
  return { ...dto }
}

function normalizePaged(raw: Record<string, unknown>): PagedEsimPackagesDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as EsimPackageDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export const ESIM_PACKAGE_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type EsimPackagesPageResult = {
  items: EsimPackage[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<EsimPackagesPageResult>>()

export async function fetchEsimPackagesPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<EsimPackagesPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminEsimPackages(pageIndex, pageSize, keyword).then((paged) => ({
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

export async function fetchAdminEsimPackages(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedEsimPackagesDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.adminEsimPackages, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(raw)
}

export async function createEsimPackage(payload: EsimPackageCreatePayload): Promise<string> {
  const raw = await httpPost<{ id?: string; Id?: string }>(API_PATHS.adminEsimPackages, payload)
  return raw.id ?? raw.Id ?? ''
}

export async function updateEsimPackage(
  id: string,
  payload: EsimPackageUpdatePayload,
): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminEsimPackages}/${id}`, payload)
}

export async function deleteEsimPackage(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminEsimPackages}/${id}`)
}
