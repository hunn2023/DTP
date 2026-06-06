import type { PhoneCard } from '@/features/products/phone-cards/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type PhoneCardDtoRaw = Record<string, unknown>

export type PhoneCardDto = {
  id: string
  productVariantId: string
  productVariantName: string
  providerId: string
  providerName: string
  name: string
  slug: string
  faceValue: number
  price: number
  currency: string
  isActive: boolean
  sortOrder: number
}

export type PhoneCardCreatePayload = {
  productVariantId: string
  providerId: string
  name: string
  slug: string
  faceValue: number
  price: number
  currency: string
  sortOrder: number
  isActive: boolean
}

export type PhoneCardUpdatePayload = Omit<PhoneCardCreatePayload, 'productVariantId'>

export type PagedPhoneCardsDto = {
  items: PhoneCardDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: PhoneCardDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: PhoneCardDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: PhoneCardDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: PhoneCardDtoRaw): PhoneCardDto {
  return {
    id: readString(raw, 'id', 'Id'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    productVariantName: readString(raw, 'productVariantName', 'ProductVariantName'),
    providerId: readString(raw, 'providerId', 'ProviderId'),
    providerName: readString(raw, 'providerName', 'ProviderName'),
    name: readString(raw, 'name', 'Name'),
    slug: readString(raw, 'slug', 'Slug'),
    faceValue: readNumber(raw, 'faceValue', 'FaceValue'),
    price: readNumber(raw, 'price', 'Price'),
    currency: readString(raw, 'currency', 'Currency'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
  }
}

function mapDto(dto: PhoneCardDto): PhoneCard {
  return { ...dto }
}

function normalizePaged(raw: Record<string, unknown>): PagedPhoneCardsDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as PhoneCardDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export const PHONE_CARD_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type PhoneCardsPageResult = {
  items: PhoneCard[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<PhoneCardsPageResult>>()

export async function fetchPhoneCardsPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PhoneCardsPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminPhoneCards(pageIndex, pageSize, keyword).then((paged) => ({
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

export async function fetchAdminPhoneCards(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedPhoneCardsDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.adminPhoneCards, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(raw)
}

export async function createPhoneCard(payload: PhoneCardCreatePayload): Promise<string> {
  const raw = await httpPost<{ id?: string; Id?: string }>(API_PATHS.adminPhoneCards, payload)
  return raw.id ?? raw.Id ?? ''
}

export async function updatePhoneCard(id: string, payload: PhoneCardUpdatePayload): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminPhoneCards}/${id}`, payload)
}

export async function deletePhoneCard(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminPhoneCards}/${id}`)
}
