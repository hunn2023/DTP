import type { PaymentProvider } from '@/features/master-data/payment-providers/types'
import { API_PATHS } from '@/shared/config/api'
import {
  readBool,
  readNumber,
  readOptionalNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpGet, httpPatch } from '@/shared/lib/http'

type Raw = Record<string, unknown>

function normalizePaymentProvider(raw: Raw): PaymentProvider {
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    paymentMethod: readString(raw, 'paymentMethod', 'PaymentMethod'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    isDefault: readBool(raw, 'isDefault', 'IsDefault'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    minAmount: readOptionalNumber(raw, 'minAmount', 'MinAmount'),
    maxAmount: readOptionalNumber(raw, 'maxAmount', 'MaxAmount'),
    currency: readString(raw, 'currency', 'Currency') || 'VND',
    logoUrl: readString(raw, 'logoUrl', 'LogoUrl'),
    description: readString(raw, 'description', 'Description'),
    createdAt: readString(raw, 'createdAt', 'CreatedAt'),
    updatedAt: readString(raw, 'updatedAt', 'UpdatedAt'),
  }
}

function normalizeList(raw: unknown): PaymentProvider[] {
  if (Array.isArray(raw)) {
    return raw.map((item) => normalizePaymentProvider(item as Raw))
  }
  return []
}

export const PAYMENT_PROVIDER_PAGE_SIZE_OPTIONS = [10, 20, 50] as const

export async function fetchPaymentProviders(): Promise<PaymentProvider[]> {
  const raw = await httpGet<unknown>(API_PATHS.adminPaymentProviders)
  return normalizeList(raw)
}

export async function setPaymentProviderActive(id: string, isActive: boolean): Promise<void> {
  await httpPatch(API_PATHS.adminPaymentProviders + `/${id}/active`, { isActive })
}

export async function setPaymentProviderDefault(id: string): Promise<void> {
  await httpPatch(API_PATHS.adminPaymentProviders + `/${id}/default`, {})
}

export async function updatePaymentProviderLimits(
  id: string,
  minAmount: number | null,
  maxAmount: number | null,
): Promise<void> {
  await httpPatch(API_PATHS.adminPaymentProviders + `/${id}/limits`, {
    minAmount,
    maxAmount,
  })
}

export async function updatePaymentProviderSortOrder(id: string, sortOrder: number): Promise<void> {
  await httpPatch(API_PATHS.adminPaymentProviders + `/${id}/sort-order`, { sortOrder })
}
