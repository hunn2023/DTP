import { API_PATHS } from '@/shared/config/api'
import { readNumber, readString, normalizePaged } from '@/shared/lib/dtoNormalize'
import { httpGet, httpPost } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type DeliveriesQueryFilters = {
  status?: number
}

export type DeliveryRow = {
  id: string
  orderId: string
  orderCode: string
  customerName: string
  customerEmail: string
  deliveryType: number
  status: number
  attemptCount: number
  lastError: string
  sentAt: string
  createdAt: string
}

export type MarkDeliveryDeliveredPayload = {
  note?: string
}

export type MarkDeliveryFailedPayload = {
  error: string
}

function readEnum(raw: Raw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  if (typeof value === 'number') return value
  const parsed = Number(value)
  return Number.isNaN(parsed) ? 0 : parsed
}

function normalizeDelivery(raw: Raw): DeliveryRow {
  const deliveredAt = readString(raw, 'deliveredAt', 'DeliveredAt')
  return {
    id: readString(raw, 'id', 'Id'),
    orderId: readString(raw, 'orderId', 'OrderId'),
    orderCode: readString(raw, 'orderCode', 'OrderCode'),
    customerName: readString(raw, 'customerName', 'CustomerName'),
    customerEmail: readString(raw, 'customerEmail', 'CustomerEmail'),
    deliveryType: readEnum(raw, 'deliveryType', 'DeliveryType'),
    status: readEnum(raw, 'status', 'Status'),
    attemptCount: readNumber(raw, 'attemptCount', 'AttemptCount'),
    lastError: readString(raw, 'lastError', 'LastError'),
    sentAt: deliveredAt,
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
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
