import type { ProductFaqRow } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ProductFaqPayload = {
  productId: string
  question: string
  answer: string
  sortOrder: number
  isActive: boolean
}

export type ProductFaqUpdatePayload = Omit<ProductFaqPayload, 'productId'>

function normalizeFaq(raw: Raw): ProductFaqRow {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    question: readString(raw, 'question', 'Question'),
    answer: readString(raw, 'answer', 'Answer'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function normalizeFaqList(raw: unknown): ProductFaqRow[] {
  if (!Array.isArray(raw)) return []
  return (raw as Raw[]).map(normalizeFaq)
}

const inflightByProduct = new Map<string, Promise<ProductFaqRow[]>>()

export async function fetchProductFaqsByProduct(productId: string): Promise<ProductFaqRow[]> {
  const inflight = inflightByProduct.get(productId)
  if (inflight) return inflight

  const promise = httpGet<unknown>(`${API_PATHS.adminProductFaqsByProduct}/${productId}`)
    .then(normalizeFaqList)
    .finally(() => {
      inflightByProduct.delete(productId)
    })

  inflightByProduct.set(productId, promise)
  return promise
}

export async function fetchProductFaqById(id: string): Promise<ProductFaqRow | null> {
  const raw = await httpGet<unknown>(`${API_PATHS.adminProductFaqs}/${id}`)
  if (!raw || typeof raw !== 'object') return null
  return normalizeFaq(raw as Raw)
}

export async function createProductFaq(payload: ProductFaqPayload): Promise<ProductFaqRow> {
  const raw = await httpPost<unknown>(API_PATHS.adminProductFaqs, payload)
  if (!raw || typeof raw !== 'object') {
    return { id: '', ...payload }
  }
  return normalizeFaq(raw as Raw)
}

export async function updateProductFaq(id: string, payload: ProductFaqUpdatePayload): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductFaqs}/${id}`, payload)
}

export async function deleteProductFaq(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductFaqs}/${id}`)
}
