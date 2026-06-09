import type { ProductVariantFeatureRow } from '@/features/products/esim-wizard/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductVariantFeaturePayload = {
  productVariantId: string
  text: string
  icon?: string
  sortOrder?: number
  isActive?: boolean
}

export type ProductVariantFeatureUpdatePayload = {
  id: string
  text: string
  icon?: string
  sortOrder: number
  isActive: boolean
}

function normalizeFeature(raw: Raw): ProductVariantFeatureRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productVariantId: readString(raw, 'productVariantId', 'ProductVariantId'),
    text: readString(raw, 'text', 'Text'),
    icon: readString(raw, 'icon', 'Icon'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

export async function fetchVariantFeatures(variantId: string): Promise<ProductVariantFeatureRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductVariantFeaturesByVariant}/${variantId}`)
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeFeature)
}

export async function createVariantFeature(payload: ProductVariantFeaturePayload): Promise<string> {
  const raw = await httpPost<string | Raw>(API_PATHS.adminProductVariantFeatures, payload)
  if (typeof raw === 'string') return raw
  return readString(raw, 'id', 'Id')
}

export async function updateVariantFeature(
  id: string,
  payload: Omit<ProductVariantFeatureUpdatePayload, 'id'>,
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductVariantFeatures}/${id}`, { id, ...payload })
}

export async function deleteVariantFeature(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductVariantFeatures}/${id}`)
}
