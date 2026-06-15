import type { ProductContentRow } from '@/features/master-data/products/types'
import type { EntityFormConfig, FormFieldOption } from '@/modules/crud/form/types'

export const PRODUCT_CONTENT_TYPE_OPTIONS: FormFieldOption[] = [
  { value: '1', label: 'Tổng quan' },
  { value: '2', label: 'Hướng dẫn sử dụng' },
  { value: '3', label: 'Hướng dẫn kích hoạt eSIM' },
  { value: '4', label: 'Chính sách' },
  { value: '5', label: 'Lưu ý' },
  { value: '6', label: 'Block landing page' },
]

export function getContentTypeLabel(contentType: number): string {
  return PRODUCT_CONTENT_TYPE_OPTIONS.find((opt) => opt.value === String(contentType))?.label ?? String(contentType)
}

export function getDefaultContentValues(productId: string): ProductContentRow {
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

export function buildContentFormConfig(productId: string): EntityFormConfig<ProductContentRow> {
  return {
    entityName: 'nội dung',
    getDefaultValues: () => getDefaultContentValues(productId),
    fields: [
      {
        name: 'contentType',
        label: 'Loại nội dung',
        type: 'select',
        required: true,
        options: PRODUCT_CONTENT_TYPE_OPTIONS,
        parseAsNumber: true,
        col: 6,
      },
      { name: 'title', label: 'Tiêu đề', type: 'text', required: true, col: 6 },
      { name: 'summary', label: 'Tóm tắt', type: 'textarea', col: 12 },
      { name: 'bodyHtml', label: 'Nội dung (HTML)', type: 'textarea', required: true, col: 12 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
    ],
  }
}
