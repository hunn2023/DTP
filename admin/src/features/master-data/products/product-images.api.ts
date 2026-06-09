import type { ProductImageRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpGet, httpPostForm, httpPut, httpPutForm } from '@/shared/lib/http'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'

type Raw = Record<string, unknown>

export type ProductImageUpdatePayload = {
  imageUrl: string
  altText?: string
  sortOrder: number
  isThumbnail: boolean
}

function normalizeImage(raw: Raw): ProductImageRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    imageUrl: readString(raw, 'imageUrl', 'ImageUrl'),
    altText: readString(raw, 'altText', 'AltText'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isThumbnail: readBool(raw, 'isThumbnail', 'IsThumbnail'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    contentType: readString(raw, 'contentType', 'ContentType'),
    size: readNumber(raw, 'size', 'Size'),
  }
}

function normalizeImageList(raw: unknown): ProductImageRow[] {
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeImage)
}

export async function fetchProductImages(productId: string): Promise<ProductImageRow[]> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductImagesByProduct}/${productId}`)
  return normalizeImageList(raw)
}

export async function uploadProductImage(
  productId: string,
  file: File,
  options: { altText?: string; isThumbnail?: boolean } = {},
): Promise<ProductImageRow> {
  const formData = new FormData()
  formData.append('File', file)
  if (options.altText?.trim()) formData.append('AltText', options.altText.trim())
  formData.append('IsThumbnail', String(Boolean(options.isThumbnail)))

  const raw = await httpPostForm<Raw>(`${API_PATHS.adminProductImages}/upload`, formData, {
    params: { productId },
  })
  return normalizeImage(raw)
}

export async function updateProductImage(
  id: string,
  payload: ProductImageUpdatePayload,
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductImages}/${id}`, payload)
}

export async function replaceProductImage(
  productId: string,
  imageId: string,
  file: File,
): Promise<ProductImageRow> {
  const formData = new FormData()
  formData.append('File', file)

  const raw = await httpPutForm<Raw>(
    `${API_PATHS.adminProductImages}/${imageId}/replace`,
    formData,
    { params: { productId } },
  )
  return normalizeImage(raw)
}

export async function setProductThumbnail(productId: string, imageId: string): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductImages}/${imageId}/thumbnail`, undefined, {
    params: { productId },
  })
}
