import type { ProductPriceRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readBool, readDateString, readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductPricePayload = {
  productId: string
  productVariantId?: string | null
  currency: string
  originalPrice: number
  salePrice: number
  costPrice: number
  startDate?: string | null
  endDate?: string | null
}

export type ProductPriceUpdatePayload = {
  currency: string
  originalPrice: number
  salePrice: number
  costPrice: number
  startDate?: string | null
  endDate?: string | null
  isActive: boolean
}

function normalizePrice(raw: Raw): ProductPriceRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    productName: readString(raw, 'productName', 'ProductName'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    productVariantName: readString(raw, 'productVariantName', 'ProductVariantName'),
    currency: readString(raw, 'currency', 'Currency') || 'VND',
    originalPrice: readNumber(raw, 'originalPrice', 'OriginalPrice'),
    salePrice: readNumber(raw, 'salePrice', 'SalePrice'),
    costPrice: readNumber(raw, 'costPrice', 'CostPrice'),
    startDate: readDateString(raw, 'startDate', 'StartDate'),
    endDate: readDateString(raw, 'endDate', 'EndDate'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

export async function fetchProductPrices(filters?: {
  productId?: string
  productVariantId?: string
}): Promise<ProductPriceRow[]> {
  const raw = await httpGet<unknown>(API_PATHS.adminProductPrices, {
    params: {
      productId: filters?.productId || undefined,
      productVariantId: filters?.productVariantId || undefined,
    },
  })
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizePrice)
}

export async function createProductPrice(payload: ProductPricePayload): Promise<string> {
  const body = {
    ...payload,
    productVariantId: payload.productVariantId || null,
    startDate: payload.startDate || null,
    endDate: payload.endDate || null,
  }
  const raw = await httpPost<{ id?: string; Id?: string } | string>(API_PATHS.adminProductPrices, body)
  if (typeof raw === 'string') return raw
  return raw.id ?? raw.Id ?? ''
}

export async function updateProductPrice(
  id: string,
  payload: ProductPriceUpdatePayload,
): Promise<void> {
  const body = {
    ...payload,
    startDate: payload.startDate || null,
    endDate: payload.endDate || null,
  }
  await httpPut<boolean>(`${API_PATHS.adminProductPrices}/${id}`, body)
}

export async function deleteProductPrice(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductPrices}/${id}`)
}
