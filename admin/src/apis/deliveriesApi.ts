import { API_PATHS } from '@/shared/config/api'
import { normalizePaged } from '@/shared/lib/dtoNormalize'
import { httpGet, httpPost } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type DeliveriesQueryFilters = {
  status?: number
}

export type DeliveryRow = {
  id: string
  orderId: string
  orderCode: string
  customerId: string
  customerName: string
  customerEmail: string
  deliveryType: number
  status: number
  attemptCount: number
  lastError: string
  ipAddress: string
  note: string
  deliveredAt: string
  failedAt: string
  createdAt: string
  emailSent: boolean
  emailSentAt: string
  emailError: string
  items: DeliveryItem[]
}

export type DeliveryItem = {
  id: string
  orderItemId: string
  productId: string
  productVariantId: string
  productName: string
  sku: string
  quantity: number
  qrCodeUrl: string
  activationCode: string
  serialNumber: string
  providerReference: string
  isDelivered: boolean
  deliveredAt: string
}

export type MarkDeliveryDeliveredPayload = {
  note?: string
}

export type MarkDeliveryFailedPayload = {
  error: string
}

function pascalCase(key: string): string {
  return key ? `${key[0].toUpperCase()}${key.slice(1)}` : key
}

function readValue(raw: Raw | null, ...keys: string[]): unknown {
  if (!raw) return undefined
  for (const key of keys) {
    const value = raw[key] ?? raw[pascalCase(key)]
    if (value !== undefined && value !== null) return value
  }
  return undefined
}

function readStringAny(raw: Raw | null, ...keys: string[]): string {
  const value = readValue(raw, ...keys)
  return value == null ? '' : String(value)
}

function readNumberAny(raw: Raw | null, ...keys: string[]): number {
  const value = readValue(raw, ...keys)
  return typeof value === 'number' ? value : Number(value ?? 0)
}

function readBoolAny(raw: Raw | null, ...keys: string[]): boolean {
  return Boolean(readValue(raw, ...keys))
}

function readArray(raw: Raw, ...keys: string[]): Raw[] {
  const value = readValue(raw, ...keys)
  return Array.isArray(value) ? (value as Raw[]) : []
}

function readEnum(raw: Raw, ...keys: string[]): number {
  const value = readValue(raw, ...keys)
  if (typeof value === 'number') return value
  const parsed = Number(value)
  return Number.isNaN(parsed) ? 0 : parsed
}

function normalizeDeliveryItem(raw: Raw): DeliveryItem {
  return {
    id: readStringAny(raw, 'id'),
    orderItemId: readStringAny(raw, 'orderItemId'),
    productId: readStringAny(raw, 'productId'),
    productVariantId: readStringAny(raw, 'productVariantId'),
    productName: readStringAny(raw, 'productName'),
    sku: readStringAny(raw, 'sku'),
    quantity: readNumberAny(raw, 'quantity'),
    qrCodeUrl: readStringAny(raw, 'qrCodeUrl'),
    activationCode: readStringAny(raw, 'activationCode'),
    serialNumber: readStringAny(raw, 'serialNumber'),
    providerReference: readStringAny(raw, 'providerReference'),
    isDelivered: readBoolAny(raw, 'isDelivered'),
    deliveredAt: readStringAny(raw, 'deliveredAt'),
  }
}

function normalizeDelivery(raw: Raw): DeliveryRow {
  return {
    id: readStringAny(raw, 'id'),
    orderId: readStringAny(raw, 'orderId'),
    orderCode: readStringAny(raw, 'orderCode'),
    customerId: readStringAny(raw, 'customerId'),
    customerName: readStringAny(raw, 'customerName'),
    customerEmail: readStringAny(raw, 'customerEmail'),
    deliveryType: readEnum(raw, 'deliveryType'),
    status: readEnum(raw, 'status'),
    attemptCount: readNumberAny(raw, 'attemptCount'),
    lastError: readStringAny(raw, 'lastError'),
    ipAddress: readStringAny(raw, 'ipAddress'),
    note: readStringAny(raw, 'note'),
    deliveredAt: readStringAny(raw, 'deliveredAt'),
    failedAt: readStringAny(raw, 'failedAt'),
    createdAt: readStringAny(raw, 'createdAt'),
    emailSent: readBoolAny(raw, 'emailSent'),
    emailSentAt: readStringAny(raw, 'emailSentAt'),
    emailError: readStringAny(raw, 'emailError'),
    items: readArray(raw, 'items', 'deliveryItems', 'digitalDeliveryItems').map(normalizeDeliveryItem),
  }
}

export async function fetchDeliveriesPage(
  pageIndex: number,
  pageSize: number,
  keyword?: string,
  filters: DeliveriesQueryFilters = {},
): Promise<{ items: DeliveryRow[]; totalCount: number; pageIndex: number; pageSize: number }> {
  const raw = await httpGet<Raw>(API_PATHS.adminDeliveries, {
    params: {
      page: pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      status: filters.status,
    },
  })

  return normalizePaged(raw, normalizeDelivery)
}

export async function fetchDeliveryById(id: string): Promise<DeliveryRow> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminDeliveries}/${id}`)
  return normalizeDelivery(raw)
}

export async function fetchDeliveriesByOrderId(orderId: string): Promise<DeliveryRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminDeliveries}/by-order/${orderId}`)
  if (Array.isArray(raw)) {
    return raw.map((item) => normalizeDelivery(item as Raw))
  }
  return [normalizeDelivery(raw as Raw)]
}

export async function processDelivery(id: string): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminDeliveries}/${id}/process`, {})
}

export async function markDeliveryDelivered(
  id: string,
  payload: MarkDeliveryDeliveredPayload = {},
): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminDeliveries}/${id}/mark-delivered`, {
    note: payload.note?.trim() || null,
  })
}

export async function markDeliveryFailed(id: string, payload: MarkDeliveryFailedPayload): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminDeliveries}/${id}/mark-failed`, {
    error: payload.error.trim(),
  })
}

export async function resendDeliveryEsimEmail(id: string): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminDeliveries}/${id}/resend-esim-email`, {})
}
