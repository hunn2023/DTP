import type { Category } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

export const categoryFormConfig: SettingsFormConfig<Category> = {
  entityName: 'danh mục',
  slugFromName: true,
  getDefaultValues: () => ({
    id: 0,
    name: '',
    slug: '',
    icon: '',
    description: '',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên danh mục', type: 'text', required: true, placeholder: 'eSIM du lịch' },
    { name: 'slug', label: 'Slug', type: 'text', required: true, placeholder: 'esim-du-lich', hint: 'URL thân thiện, tự gợi ý từ tên khi tạo mới' },
    { name: 'icon', label: 'Icon', type: 'text', placeholder: '✈️ hoặc class icon', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'description', label: 'Mô tả', type: 'textarea', placeholder: 'Mô tả ngắn danh mục' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
