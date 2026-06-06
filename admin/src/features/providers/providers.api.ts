import type { CatalogProvider } from '@/features/providers/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import type { FormFieldOption } from '@/modules/crud/form/types'

type ProviderDtoRaw = Record<string, unknown>

export type ProviderDto = {
  id: string
  code: string
  name: string
  apiBaseUrl: string
  apiKey: string
  apiSecret: string
  webhookUrl: string
  supportEmail: string
  isActive: boolean
}

export type ProviderPayload = {
  code: string
  name: string
  apiBaseUrl?: string
  apiKey?: string
  apiSecret?: string
  webhookUrl?: string
  supportEmail?: string
}

export type ProviderUpdatePayload = ProviderPayload & {
  isActive: boolean
}

export type PagedProvidersDto = {
  items: ProviderDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

function readString(raw: ProviderDtoRaw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function readBool(raw: ProviderDtoRaw, camel: string, pascal: string): boolean {
  const value = raw[camel] ?? raw[pascal]
  return Boolean(value)
}

function readNumber(raw: ProviderDtoRaw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function normalizeDto(raw: ProviderDtoRaw): ProviderDto {
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    apiBaseUrl: readString(raw, 'apiBaseUrl', 'ApiBaseUrl'),
    apiKey: readString(raw, 'apiKey', 'ApiKey'),
    apiSecret: readString(raw, 'apiSecret', 'ApiSecret'),
    webhookUrl: readString(raw, 'webhookUrl', 'WebhookUrl'),
    supportEmail: readString(raw, 'supportEmail', 'SupportEmail'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function mapDto(dto: ProviderDto): CatalogProvider {
  return { ...dto }
}

function normalizePaged(raw: Record<string, unknown>): PagedProvidersDto {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as ProviderDtoRaw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeDto) : [],
    totalCount: readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export const PROVIDER_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

export type ProvidersPageResult = {
  items: CatalogProvider[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

const inflightPages = new Map<string, Promise<ProvidersPageResult>>()

let cachedProviderOptions: FormFieldOption[] | null = null
let inflightProviderOptions: Promise<FormFieldOption[]> | null = null

export function invalidateProvidersCache(): void {
  cachedProviderOptions = null
  inflightProviderOptions = null
}

export async function fetchProvidersPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<ProvidersPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminProviders(pageIndex, pageSize, keyword).then((paged) => ({
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

export async function fetchAdminProviders(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
): Promise<PagedProvidersDto> {
  const raw = await httpGet<Record<string, unknown>>(API_PATHS.adminProviders, {
    params: { pageIndex, pageSize, keyword: keyword?.trim() || undefined },
  })
  return normalizePaged(raw)
}

export async function fetchProviderOptions(): Promise<FormFieldOption[]> {
  if (cachedProviderOptions) return cachedProviderOptions
  if (inflightProviderOptions) return inflightProviderOptions

  inflightProviderOptions = fetchAdminProviders(1, 500)
    .then((paged) => {
      cachedProviderOptions = paged.items.map((item) => ({
        value: item.id,
        label: `${item.code} — ${item.name}`,
      }))
      return cachedProviderOptions
    })
    .finally(() => {
      inflightProviderOptions = null
    })

  return inflightProviderOptions
}

export async function createProvider(payload: ProviderPayload): Promise<string> {
  const raw = await httpPost<{ id?: string; Id?: string }>(API_PATHS.adminProviders, payload)
  invalidateProvidersCache()
  return raw.id ?? raw.Id ?? ''
}

export async function updateProvider(id: string, payload: ProviderUpdatePayload): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminProviders}/${id}`, payload)
  invalidateProvidersCache()
}

export async function deleteProvider(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProviders}/${id}`)
  invalidateProvidersCache()
}
