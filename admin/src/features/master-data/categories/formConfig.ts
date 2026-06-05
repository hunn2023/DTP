import type { Category } from '@/features/master-data/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export const categoryFormConfig: EntityFormConfig<Category> = {
  entityName: 'danh mục',
  slugFromName: true,
  getDefaultValues: () => ({
    id: '',
    name: '',
    slug: '',
    code: '',
    icon: '',
    description: '',
    sortOrder: 0,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên danh mục', type: 'text', required: true, placeholder: 'eSIM du lịch' },
    { name: 'code', label: 'Mã', type: 'text', placeholder: 'ESIM', col: 6 },
    { name: 'slug', label: 'Slug', type: 'text', required: true, placeholder: 'esim-du-lich', hint: 'URL thân thiện, tự gợi ý từ tên khi tạo mới', col: 6 },
    { name: 'icon', label: 'Icon', type: 'text', placeholder: '✈️ hoặc class icon', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'description', label: 'Mô tả', type: 'textarea', placeholder: 'Mô tả ngắn danh mục' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
