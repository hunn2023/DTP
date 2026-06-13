import { API_PATHS } from '@/shared/config/api'
import { readNumber, readString, normalizePaged } from '@/shared/lib/dtoNormalize'
import { httpGet, httpPost } from '@/shared/lib/http'

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
  items: OrderItem[]
  histories: OrderHistory[]
}

export type OrderItem = {
  id: string
  itemType: number
  productId: string
  productVariantId: string
  esimPackageId: string
  phoneCardId: string
  productName: string
  variantName: string
  sku: string
  quantity: number
  unitPrice: number
  totalPrice: number
  currency: string
}

export type OrderHistory = {
  id: string
  fromStatus: number
  toStatus: number
  note: string
  changedBy: string
  createdAt: string
}

export type MarkOrderPaidPayload = {
  paymentTransactionId: string
  changedBy?: string
}

export type CompleteOrderPayload = {
  changedBy?: string
}

export type CancelOrderPayload = {
  reason: string
  changedBy?: string
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

function readArray(raw: Raw, camel: string, pascal: string): Raw[] {
  const value = raw[camel] ?? raw[pascal]
  return Array.isArray(value) ? (value as Raw[]) : []
}

function normalizeOrderItem(raw: Raw): OrderItem {
  return {
    id: readString(raw, 'id', 'Id'),
    itemType: readEnum(raw, 'itemType', 'ItemType'),
    productId: readString(raw, 'productId', 'ProductId'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    esimPackageId: readString(raw, 'esimPackageId', 'EsimPackageId'),
    phoneCardId: readString(raw, 'phoneCardId', 'PhoneCardId'),
    productName: readString(raw, 'productName', 'ProductName'),
    variantName: readString(raw, 'variantName', 'VariantName'),
    sku: readString(raw, 'sku', 'Sku'),
    quantity: readNumber(raw, 'quantity', 'Quantity'),
    unitPrice: readNumber(raw, 'unitPrice', 'UnitPrice'),
    totalPrice: readNumber(raw, 'totalPrice', 'TotalPrice'),
    currency: readString(raw, 'currency', 'Currency') || 'VND',
  }
}

function normalizeOrderHistory(raw: Raw): OrderHistory {
  return {
    id: readString(raw, 'id', 'Id'),
    fromStatus: readEnum(raw, 'fromStatus', 'FromStatus'),
    toStatus: readEnum(raw, 'toStatus', 'ToStatus'),
    note: readString(raw, 'note', 'Note'),
    changedBy: readString(raw, 'changedBy', 'ChangedBy'),
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
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
    items: readArray(raw, 'items', 'Items').map(normalizeOrderItem),
    histories: readArray(raw, 'histories', 'Histories').map(normalizeOrderHistory),
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

export async function markOrderPaid(id: string, payload: MarkOrderPaidPayload): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminOrders}/${id}/mark-paid`, {
    paymentTransactionId: payload.paymentTransactionId.trim(),
    changedBy: payload.changedBy || null,
  })
}

export async function completeOrder(id: string, payload: CompleteOrderPayload = {}): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminOrders}/${id}/complete`, {
    changedBy: payload.changedBy || null,
  })
}

export async function cancelOrder(id: string, payload: CancelOrderPayload): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminOrders}/${id}/cancel`, {
    reason: payload.reason.trim(),
    changedBy: payload.changedBy || null,
  })
}
