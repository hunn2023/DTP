import { API_PATHS } from '@/shared/config/api'
import { readNumber, readString, normalizePaged } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type OrdersQueryFilters = {
  status?: number
  paymentStatus?: number
  customerId?: string
}

export type OrderRow = {
  id: string
  orderCode: string
  customerId: string
  customerName: string
  customerEmail: string
  customerPhone: string
  totalAmount: number
  currency: string
  status: number
  paymentStatus: number
  paymentMethod: string
  createdAt: string
  paidAt: string
}

export type OrderDetail = OrderRow & {
  subTotal: number
  discountAmount: number
  note: string
  paymentTransactionId: string
  paymentExpiredAt: string
  cancelReason: string
  updatedAt: string
  cancelledAt: string
}

function readEnum(raw: Raw, camel: string, pascal: string): number {
  const value = raw[camel] ?? raw[pascal]
  if (typeof value === 'number') return value
  const parsed = Number(value)
  return Number.isNaN(parsed) ? 0 : parsed
}

function normalizeOrder(raw: Raw): OrderRow {
  return {
    id: readString(raw, 'id', 'Id'),
    orderCode: readString(raw, 'orderCode', 'OrderCode'),
    customerId: readString(raw, 'customerId', 'CustomerId'),
    customerName: readString(raw, 'customerName', 'CustomerName'),
    customerEmail: readString(raw, 'customerEmail', 'CustomerEmail'),
    customerPhone: readString(raw, 'customerPhone', 'CustomerPhone'),
    totalAmount: readNumber(raw, 'totalAmount', 'TotalAmount'),
    currency: readString(raw, 'currency', 'Currency') || 'VND',
    status: readEnum(raw, 'status', 'Status'),
    paymentStatus: readEnum(raw, 'paymentStatus', 'PaymentStatus'),
    paymentMethod: readString(raw, 'paymentMethod', 'PaymentMethod'),
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
    paidAt: readString(raw, 'paidAt', 'PaidAt'),
  }
}

function normalizeOrderDetail(raw: Raw): OrderDetail {
  return {
    ...normalizeOrder(raw),
    subTotal: readNumber(raw, 'subTotal', 'SubTotal'),
    discountAmount: readNumber(raw, 'discountAmount', 'DiscountAmount'),
    note: readString(raw, 'note', 'Note'),
    paymentTransactionId: readString(raw, 'paymentTransactionId', 'PaymentTransactionId'),
    paymentExpiredAt: readString(raw, 'paymentExpiredAt', 'PaymentExpiredAt'),
    cancelReason: readString(raw, 'cancelReason', 'CancelReason'),
    updatedAt: readString(raw, 'updatedAt', 'UpdatedAt'),
    cancelledAt: readString(raw, 'cancelledAt', 'CancelledAt'),
  }
}

export async function fetchOrdersPage(
  pageIndex: number,
  pageSize: number,
  keyword?: string,
  filters: OrdersQueryFilters = {},
): Promise<{ items: OrderRow[]; totalCount: number; pageIndex: number; pageSize: number }> {
  const raw = await httpGet<Raw>(API_PATHS.adminOrders, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      status: filters.status,
      paymentStatus: filters.paymentStatus,
      customerId: filters.customerId,
    },
  })

  return normalizePaged(raw, normalizeOrder)
}

export async function fetchOrderById(id: string): Promise<OrderDetail> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminOrders}/${id}`)
  return normalizeOrderDetail(raw)
}
