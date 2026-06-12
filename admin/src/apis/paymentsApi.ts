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
  providerTransactionId: string
  paidAt: string
  createdAt: string
  expiredAt: string
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
    providerTransactionId: readString(raw, 'providerTransactionId', 'ProviderTransactionId'),
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
