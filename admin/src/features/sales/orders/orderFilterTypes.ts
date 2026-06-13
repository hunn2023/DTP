import type { OrdersQueryFilters } from '@/apis/ordersApi'

export type OrderFilterForm = {
  customerId: string
  status: string
  paymentStatus: string
}

export function defaultOrderFilterForm(initial: OrdersQueryFilters = {}): OrderFilterForm {
  return {
    customerId: initial.customerId ?? '',
    status: initial.status != null ? String(initial.status) : '',
    paymentStatus: initial.paymentStatus != null ? String(initial.paymentStatus) : '',
  }
}

function parseOptionalInt(value: string): number | undefined {
  if (!value) return undefined
  const parsed = Number(value)
  return Number.isNaN(parsed) ? undefined : parsed
}

export function toOrdersQueryFilters(form: OrderFilterForm): OrdersQueryFilters {
  return {
    customerId: form.customerId.trim() || undefined,
    status: parseOptionalInt(form.status),
    paymentStatus: parseOptionalInt(form.paymentStatus),
  }
}

export function orderFilterFormKey(form: OrderFilterForm): string {
  return `${form.customerId}:${form.status}:${form.paymentStatus}`
}
