import type { Tag } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

export const tagFormConfig: SettingsFormConfig<Tag> = {
  entityName: 'tag',
  slugFromName: true,
  getDefaultValues: () => ({
    id: 0,
    name: '',
    slug: '',
    color: '#3b82f6',
    icon: '',
    type: 'marketing',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên tag', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true },
    { name: 'icon', label: 'Icon', type: 'text', placeholder: '🔥', col: 6 },
    { name: 'color', label: 'Màu', type: 'color', col: 6 },
    {
      name: 'type',
      type: 'select',
      required: true,
      options: [
        { value: 'product', label: 'Product' },
        { value: 'marketing', label: 'Marketing' },
        { value: 'technical', label: 'Technical' },
      ],
    },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
