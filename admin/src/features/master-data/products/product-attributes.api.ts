import type { ProductAttributeRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductAttributePayload = {
  productId: string
  name: string
  value: string
  sortOrder: number
}

export type ProductAttributeUpdatePayload = Omit<ProductAttributePayload, 'productId'>

function normalizeAttribute(raw: Raw): ProductAttributeRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    name: readString(raw, 'name', 'Name'),
    value: readString(raw, 'value', 'Value'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: true,
  }
}

export async function fetchProductAttributes(productId: string): Promise<ProductAttributeRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductAttributesByProduct}/${productId}`)
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeAttribute)
}

export async function createProductAttribute(payload: ProductAttributePayload): Promise<string> {
  const raw = await httpPost<string | Raw>(API_PATHS.adminProductAttributes, payload)
  if (typeof raw === 'string') return raw
  return readString(raw, 'id', 'Id')
}

export async function updateProductAttribute(
  id: string,
  payload: ProductAttributeUpdatePayload,
): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminProductAttributes}/${id}`, payload)
}

export async function deleteProductAttribute(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductAttributes}/${id}`)
}
