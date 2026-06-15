import { API_PATHS } from '@/shared/config/api'
import { readNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type PaymentRow = {
  id: string
  orderId: string
  orderCode: string
  customerId: string
  amount: number
  currency: string
  provider: string
  method: string
  status: string
  requestId: string
  providerTransactionId: string
  qrCode: string
  qrImageUrl: string
  paymentUrl: string
  paidAt: string
  createdAt: string
  expiredAt: string
}

export type PaymentDetailResult = {
  byId: PaymentRow | null
  byOrder: PaymentRow | null
}

function normalizePayment(raw: Raw): PaymentRow {
  return {
    id: readString(raw, 'id', 'Id'),
    orderId: readString(raw, 'orderId', 'OrderId'),
    orderCode: readString(raw, 'orderCode', 'OrderCode'),
    customerId: readString(raw, 'customerId', 'CustomerId'),
    amount: readNumber(raw, 'amount', 'Amount'),
    currency: readString(raw, 'currency', 'Currency') || 'VND',
    provider: readString(raw, 'provider', 'Provider'),
    method: readString(raw, 'method', 'Method'),
    status: readString(raw, 'status', 'Status'),
    requestId: readString(raw, 'requestId', 'RequestId'),
    providerTransactionId: readString(raw, 'providerTransactionId', 'ProviderTransactionId'),
    qrCode: readString(raw, 'qrCode', 'QrCode'),
    qrImageUrl: readString(raw, 'qrImageUrl', 'QrImageUrl'),
    paymentUrl: readString(raw, 'paymentUrl', 'PaymentUrl'),
    paidAt: readString(raw, 'paidAt', 'PaidAt'),
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
    expiredAt: readString(raw, 'expiredAt', 'ExpiredAt'),
  }
}

export async function fetchPaymentById(id: string): Promise<PaymentRow> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminPayments}/${id}`)
  return normalizePayment(raw)
}

export async function fetchPaymentByOrderId(orderId: string): Promise<PaymentRow> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminPayments}/orders/${orderId}`)
  return normalizePayment(raw)
}

export async function fetchPaymentDetailByOrderId(orderId: string): Promise<PaymentDetailResult> {
  const byOrder = await fetchPaymentByOrderId(orderId)
  let byId: PaymentRow | null = null
  if (byOrder.id) {
    try {
      byId = await fetchPaymentById(byOrder.id)
    } catch {
      byId = null
    }
  }
  return { byOrder, byId }
}

export async function fetchPaymentDetailById(id: string): Promise<PaymentDetailResult> {
  const byId = await fetchPaymentById(id)
  let byOrder: PaymentRow | null = null
  if (byId.orderId) {
    try {
      byOrder = await fetchPaymentByOrderId(byId.orderId)
    } catch {
      byOrder = null
    }
  }
  return { byId, byOrder }
}
