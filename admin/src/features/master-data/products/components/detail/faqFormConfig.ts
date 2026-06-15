import type { ProductFaqRow } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultFaqValues(productId: string): ProductFaqRow {
  return {
    id: '',
    productId,
    question: '',
    answer: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function buildFaqFormConfig(productId: string): EntityFormConfig<ProductFaqRow> {
  return {
    entityName: 'FAQ',
    getDefaultValues: () => getDefaultFaqValues(productId),
    fields: [
      { name: 'question', label: 'Câu hỏi', type: 'text', required: true, col: 12 },
      { name: 'answer', label: 'Trả lời', type: 'textarea', required: true, col: 12 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
    ],
  }
}
