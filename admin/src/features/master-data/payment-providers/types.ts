import type { TableRowBase } from '@/modules/crud/types'

export type PaymentProvider = TableRowBase & {
  id: string
  code: string
  name: string
  paymentMethod: string
  isDefault: boolean
  sortOrder: number
  minAmount: number | null
  maxAmount: number | null
  currency: string
  logoUrl: string
  description: string
  createdAt: string
  updatedAt: string
}
