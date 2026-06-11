import type { ProductVariant } from '@/features/master-data/products/types'
import { API_PATHS } from '@/shared/config/api'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'
import { readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'
import type { FormFieldOption } from '@/modules/crud/form/types'

type Raw = Record<string, unknown>

export type ProductVariantPayload = {
  productId: string
  sku?: string
  name: string
  shortName?: string
  description?: string
  sortOrder: number
  isActive?: boolean
}

export type ProductVariantUpdatePayload = Omit<ProductVariantPayload, 'productId'>

function normalizeVariant(raw: Raw): ProductVariant {
  return {
    id: readString(raw, 'id', 'Id'),
    productId: readString(raw, 'productId', 'ProductId'),
    sku: readString(raw, 'sku', 'Sku'),
    name: readString(raw, 'name', 'Name'),
    shortName: readString(raw, 'shortName', 'ShortName'),
    description: readString(raw, 'description', 'Description'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

const inflightProductVariants = new Map<string, Promise<ProductVariant[]>>()

export async function fetchProductVariants(productId: string): Promise<ProductVariant[]> {
  const inflight = inflightProductVariants.get(productId)
  if (inflight) return inflight

  const promise = httpGet<unknown>(`${API_PATHS.adminProductVariantsByProduct}/${productId}`)
    .then((raw) => {
      if (!Array.isArray(raw)) return []
      return (raw as Raw[]).map(normalizeVariant)
    })
    .finally(() => {
      inflightProductVariants.delete(productId)
    })

  inflightProductVariants.set(productId, promise)
  return promise
}

export async function fetchProductVariantOptions(productId: string): Promise<FormFieldOption[]> {
  const variants = await fetchProductVariants(productId)
  return variants.map((item) => ({
    value: item.id,
    label: item.name,
  }))
}

export async function createProductVariant(payload: ProductVariantPayload): Promise<string> {
  const raw = await httpPost<string | Raw>(API_PATHS.adminProductVariants, payload)
  if (typeof raw === 'string') return raw
  return readString(raw, 'id', 'Id')
}

export async function updateProductVariant(
  id: string,
  payload: ProductVariantUpdatePayload,
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminProductVariants}/${id}`, payload)
}

export async function deleteProductVariant(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminProductVariants}/${id}`)
}

export async function fetchAllProductVariantOptions(): Promise<FormFieldOption[]> {
  const { fetchAdminProducts } = await import('@/features/master-data/products/products.api')
  const paged = await fetchAdminProducts(1, 100, { isActive: true })
  const groups = await Promise.all(
    paged.items.map(async (product) => {
      const variants = await fetchProductVariants(product.id)
      return variants.map((variant) => ({
        value: variant.id,
        label: `${product.name} — ${variant.name}`,
      }))
    }),
  )
  return groups.flat()
}
