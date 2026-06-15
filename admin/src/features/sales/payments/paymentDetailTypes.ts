import type { PaymentDetailResult } from '@/apis/paymentsApi'

export function mergePaymentDetail(detail: PaymentDetailResult) {
  const primary = detail.byId ?? detail.byOrder
  if (!primary) return null
  return { ...detail.byOrder, ...detail.byId, ...primary }
}
