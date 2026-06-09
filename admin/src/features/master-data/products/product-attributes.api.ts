import type { ProductAttributeRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductAttributePayload = {
  productId: string
  key: string
  displayName?: string
  value: string
  sortOrder: number
  isVisible: boolean
}

export type ProductAttributeUpdatePayload = Omit<ProductAttributePayload, 'productId'>

function normalizeAttribute(raw: Raw): ProductAttributeRow {
  const key = readString(raw, 'key', 'Key')
  const displayName = readString(raw, 'displayName', 'DisplayName')
  const isVisible = readBool(raw, 'isVisible', 'IsVisible')
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    key,
    displayName: displayName || key,
    value: readString(raw, 'value', 'Value'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isVisible,
    isActive: isVisible,
  }
}

function normalizeAttributeList(raw: unknown): ProductAttributeRow[] {
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeAttribute)
}

function parseCreatedId(raw: unknown): string {
  if (typeof raw === 'string') return raw
  if (typeof raw === 'object' && raw !== null) {
    return readString(raw as Raw, 'id', 'Id')
  }
  return String(raw ?? '')
}

export async function fetchProductAttributes(productId: string): Promise<ProductAttributeRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductAttributesByProduct}/${productId}`)
  return normalizeAttributeList(raw)
}

export async function createProductAttribute(payload: ProductAttributePayload): Promise<string> {
  const id = await httpPost<unknown>(API_PATHS.adminProductAttributes, payload)
  return parseCreatedId(id)
}

export async function updateProductAttribute(
  id: string,
  payload: ProductAttributeUpdatePayload,
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductAttributes}/${id}`, payload)
}

export async function deleteProductAttribute(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductAttributes}/${id}`)
}
