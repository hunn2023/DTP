import type { ProductImageRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductImagePayload = {
  productId: string
  imageUrl: string
  altText?: string
  sortOrder: number
  isThumbnail: boolean
}

export type ProductImageUpdatePayload = Omit<ProductImagePayload, 'productId'>

function normalizeImage(raw: Raw): ProductImageRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    imageUrl: readString(raw, 'imageUrl', 'ImageUrl'),
    altText: readString(raw, 'altText', 'AltText'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isThumbnail: readBool(raw, 'isThumbnail', 'IsThumbnail'),
    isActive: true,
  }
}

export async function fetchProductImages(productId: string): Promise<ProductImageRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductImagesByProduct}/${productId}`)
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeImage)
}

export async function createProductImage(payload: ProductImagePayload): Promise<string> {
  const raw = await httpPost<string | Raw>(API_PATHS.adminProductImages, payload)
  if (typeof raw === 'string') return raw
  return readString(raw, 'id', 'Id')
}

export async function updateProductImage(
  id: string,
  payload: ProductImageUpdatePayload,
): Promise<void> {
  await httpPut<boolean>(`${API_PATHS.adminProductImages}/${id}`, payload)
}

export async function deleteProductImage(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductImages}/${id}`)
}
