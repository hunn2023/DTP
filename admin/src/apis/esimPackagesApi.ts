import type { EsimPackage } from '@/features/products/esim-packages/types'
import type { EsimPackageCarrierLink } from '@/features/products/esim-wizard/types'
import { API_PATHS } from '@/shared/config/api'
import { parseCreatedId } from '@/shared/lib/createdId'
import { readBool, readNumber, readOptionalNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type EsimPackagePayload = {
  productId: string
  productVariantId: string
  providerId: string
  countryId: string
  name: string
  slug: string
  providerPackageCode: string
  dataAmount?: number | null
  dataUnit: string
  validityDays: number
  isUnlimited: boolean
  coverageType: string
  coverageDescription?: string
  activationPolicy: string
  speedPolicy?: string
  hotspotSupported: boolean
  phoneNumberSupported: boolean
  smsSupported: boolean
  kycRequired: boolean
  qrDeliveryType: string
  sortOrder: number
  isActive: boolean
  carrierIds: string[]
}

export type EsimPackageUpdatePayload = EsimPackagePayload

export type PagedEsimPackagesDto = {
  items: EsimPackage[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function normalizeCarrier(raw: Raw): EsimPackageCarrierLink {
  return {
    carrierId: readString(raw, 'carrierId', 'CarrierId'),
    carrierName: readString(raw, 'carrierName', 'CarrierName'),
  }
}

function normalizeDto(raw: Raw): EsimPackage {
  const carriersRaw = (raw.carriers ?? raw.Carriers ?? []) as Raw[]
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    productName: readString(raw, 'productName', 'ProductName'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    productVariantName: readString(raw, 'productVariantName', 'ProductVariantName'),
    providerId: readString(raw, 'providerId', 'ProviderId'),
    providerName: readString(raw, 'providerName', 'ProviderName'),
    countryId: readString(raw, 'countryId', 'CountryId'),
    countryName: readString(raw, 'countryName', 'CountryName'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    providerPackageCode: readString(raw, 'providerPackageCode', 'ProviderPackageCode'),
    dataAmount: readOptionalNumber(raw, 'dataAmount', 'DataAmount'),
    dataUnit: readString(raw, 'dataUnit', 'DataUnit'),
    validityDays: readNumber(raw, 'validityDays', 'ValidityDays'),
    isUnlimited: readBool(raw, 'isUnlimited', 'IsUnlimited'),
    coverageType: readString(raw, 'coverageType', 'CoverageType'),
    coverageDescription: readString(raw, 'coverageDescription', 'CoverageDescription'),
    activationPolicy: readString(raw, 'activationPolicy', 'ActivationPolicy'),
    speedPolicy: readString(raw, 'speedPolicy', 'SpeedPolicy'),
    hotspotSupported: readBool(raw, 'hotspotSupported', 'HotspotSupported'),
    phoneNumberSupported: readBool(raw, 'phoneNumberSupported', 'PhoneNumberSupported'),
    smsSupported: readBool(raw, 'smsSupported', 'SmsSupported'),
    kycRequired: readBool(raw, 'kycRequired', 'KycRequired'),
    qrDeliveryType: readString(raw, 'qrDeliveryType', 'QrDeliveryType'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    carriers: Array.isArray(carriersRaw) ? carriersRaw.map(normalizeCarrier) : [],
  }
}

function normalizePaged(raw: Record<string, unknown>): PagedEsimPackagesDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as Raw[]
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

export type EsimPackageListFilters = {
  keyword?: string
  countryId?: string
  carrierId?: string
  productVariantId?: string
  isActive?: boolean
}

export async function fetchEsimPackagesPage(
  pageIndex = 1,
  pageSize = 10,
  filters: EsimPackageListFilters = {},
): Promise<EsimPackagesPageResult> {
  const key = `${pageIndex}:${pageSize}:${JSON.stringify(filters)}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminEsimPackages(pageIndex, pageSize, filters).then((paged) => ({
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

const inflightAdminEsimPackages = new Map<string, Promise<PagedEsimPackagesDto>>()

export async function fetchAdminEsimPackages(
  pageIndex = 1,
  pageSize = 10,
  filters: EsimPackageListFilters = {},
): Promise<PagedEsimPackagesDto> {
  const key = `${pageIndex}:${pageSize}:${JSON.stringify(filters)}`
  const inflight = inflightAdminEsimPackages.get(key)
  if (inflight) return inflight

  const promise = httpGet<Record<string, unknown>>(API_PATHS.adminEsimPackages, {
    params: {
      pageIndex,
      pageSize,
      keyword: filters.keyword?.trim() || undefined,
      countryId: filters.countryId || undefined,
      carrierId: filters.carrierId || undefined,
      productVariantId: filters.productVariantId || undefined,
      isActive: filters.isActive,
    },
  })
    .then((raw) => normalizePaged(raw))
    .finally(() => {
      inflightAdminEsimPackages.delete(key)
    })

  inflightAdminEsimPackages.set(key, promise)
  return promise
}

const inflightPackageDetails = new Map<string, Promise<EsimPackage | null>>()

export async function fetchEsimPackageDetail(id: string): Promise<EsimPackage | null> {
  const inflight = inflightPackageDetails.get(id)
  if (inflight) return inflight

  const promise = httpGet<Raw>(`${API_PATHS.adminEsimPackages}/${id}`)
    .then((raw) => normalizeDto(raw))
    .catch(() => null)
    .finally(() => {
      inflightPackageDetails.delete(id)
    })

  inflightPackageDetails.set(id, promise)
  return promise
}

export async function createEsimPackage(payload: EsimPackagePayload): Promise<string> {
  const raw = await httpPost<unknown>(API_PATHS.adminEsimPackages, payload)
  return parseCreatedId(raw)
}

export async function updateEsimPackage(id: string, payload: EsimPackageUpdatePayload): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminEsimPackages}/${id}`, payload)
}

export async function deleteEsimPackage(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminEsimPackages}/${id}`)
}
