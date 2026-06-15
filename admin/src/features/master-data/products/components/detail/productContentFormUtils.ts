import type { ProductContentRow, ProductContentType } from '@/features/master-data/products/types'
import type { ProductContentPayload, ProductContentUpdatePayload } from '@/apis/productContentsApi'

export type ProductContentFormErrors = Partial<Record<'title' | 'bodyHtml' | 'contentType', string>>

export function getDefaultProductContentValues(productId: string): ProductContentRow {
  return {
    id: '',
    productId,
    contentType: 1,
    contentTypeName: 'Overview',
    title: '',
    summary: '',
    bodyHtml: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function validateProductContentForm(values: ProductContentRow): ProductContentFormErrors {
  const errors: ProductContentFormErrors = {}
  if (!values.title.trim()) errors.title = 'Tiêu đề là bắt buộc'
  if (!values.bodyHtml.trim()) errors.bodyHtml = 'Nội dung là bắt buộc'
  if (values.contentType < 1 || values.contentType > 6) errors.contentType = 'Loại nội dung không hợp lệ'
  return errors
}

export function toProductContentPayload(values: ProductContentRow): ProductContentPayload {
  return {
    productId: values.productId,
    contentType: values.contentType as ProductContentType,
    title: values.title.trim(),
    summary: values.summary.trim() || undefined,
    bodyHtml: values.bodyHtml.trim(),
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

export function toProductContentUpdatePayload(values: ProductContentRow): ProductContentUpdatePayload {
  const { productId: _, ...rest } = toProductContentPayload(values)
  return rest
}
