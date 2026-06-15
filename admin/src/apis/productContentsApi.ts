import type { ProductContentRow, ProductContentType } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ProductContentPayload = {
  productId: string
  contentType: ProductContentType
  title: string
  summary?: string
  bodyHtml: string
  sortOrder: number
  isActive: boolean
}

export type ProductContentUpdatePayload = Omit<ProductContentPayload, 'productId'>

function readContentType(raw: Raw): ProductContentType {
  const value = readNumber(raw, 'contentType', 'ContentType')
  if (value >= 1 && value <= 6) return value as ProductContentType
  return 1
}

function normalizeContent(raw: Raw): ProductContentRow {
  const contentType = readContentType(raw)
  const contentTypeName = readString(raw, 'contentTypeName', 'ContentTypeName')
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    contentType,
    contentTypeName: contentTypeName || String(contentType),
    title: readString(raw, 'title', 'Title'),
    summary: readString(raw, 'summary', 'Summary'),
    bodyHtml: readString(raw, 'bodyHtml', 'BodyHtml'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function normalizeContentList(raw: unknown): ProductContentRow[] {
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeContent)
}

export type ProductContentListParams = {
  onlyActive?: boolean
}

function buildListCacheKey(productId: string, contentType?: ProductContentType, onlyActive?: boolean): string {
  return `${productId}|${contentType ?? 'all'}|${onlyActive ? '1' : '0'}`
}

const inflightByProduct = new Map<string, Promise<ProductContentRow[]>>()

export async function fetchProductContentsByProduct(
  productId: string,
  params?: ProductContentListParams,
): Promise<ProductContentRow[]> {
  const cacheKey = buildListCacheKey(productId, undefined, params?.onlyActive)
  const inflight = inflightByProduct.get(cacheKey)
  if (inflight) return inflight

  const promise = httpGet<unknown>(`${API_PATHS.adminProductContentsByProduct}/${productId}`, {
    params: params?.onlyActive ? { onlyActive: true } : undefined,
  })
    .then(normalizeContentList)
    .finally(() => {
      inflightByProduct.delete(cacheKey)
    })

  inflightByProduct.set(cacheKey, promise)
  return promise
}

export async function fetchProductContentsByProductAndType(
  productId: string,
  contentType: ProductContentType,
  params?: ProductContentListParams,
): Promise<ProductContentRow[]> {
  const cacheKey = buildListCacheKey(productId, contentType, params?.onlyActive)
  const inflight = inflightByProduct.get(cacheKey)
  if (inflight) return inflight

  const promise = httpGet<unknown>(
    `${API_PATHS.adminProductContentsByProduct}/${productId}/type/${contentType}`,
    { params: params?.onlyActive ? { onlyActive: true } : undefined },
  )
    .then(normalizeContentList)
    .finally(() => {
      inflightByProduct.delete(cacheKey)
    })

  inflightByProduct.set(cacheKey, promise)
  return promise
}

export async function fetchProductContentById(id: string): Promise<ProductContentRow | null> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductContents}/${id}`)
  if (!raw || typeof raw !== 'object') return null
  return normalizeContent(raw as Raw)
}

export async function createProductContent(payload: ProductContentPayload): Promise<ProductContentRow> {
  const raw = await httpPost<unknown>(API_PATHS.adminProductContents, payload)
  if (!raw || typeof raw !== 'object') {
    return {
      id: '',
      contentTypeName: String(payload.contentType),
      summary: payload.summary ?? '',
      ...payload,
    }
  }
  return normalizeContent(raw as Raw)
}

export async function updateProductContent(
  id: string,
  payload: ProductContentUpdatePayload,
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductContents}/${id}`, payload)
}

export async function deleteProductContent(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductContents}/${id}`)
}
