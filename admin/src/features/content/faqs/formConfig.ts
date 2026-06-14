import type { ContentFaq } from '@/features/content/types'
import type { EntityFormConfig, FormFieldOption } from '@/modules/crud/form/types'

export function getDefaultFaqValues(): ContentFaq {
  return {
    id: '',
    question: '',
    answer: '',
    categoryCode: '',
    sortOrder: 0,
    isActive: true,
  }
}

export function buildFaqFormConfig(categoryOptions: FormFieldOption[]): EntityFormConfig<ContentFaq> {
  return {
    entityName: 'FAQ',
    getDefaultValues: getDefaultFaqValues,
    fields: [
      { name: 'question', label: 'Câu hỏi', type: 'text', required: true },
      { name: 'answer', label: 'Trả lời', type: 'textarea', required: true },
      {
        name: 'categoryCode',
        label: 'Danh mục',
        type: 'select',
        col: 6,
        options: categoryOptions,
      },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
    ],
  }
}

/** @deprecated Dùng buildFaqFormConfig — giữ để tương thích import cũ */
export const faqFormConfig = buildFaqFormConfig([])