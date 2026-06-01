import type { Brand } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

export const brandFormConfig: SettingsFormConfig<Brand> = {
  entityName: 'brand',
  slugFromName: true,
  getDefaultValues: () => ({
    id: 0,
    name: '',
    slug: '',
    logoUrl: '',
    brandColor: '#000000',
    websiteUrl: '',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên brand', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true },
    { name: 'logoUrl', label: 'Logo URL', type: 'url', placeholder: 'https://...' },
    { name: 'brandColor', label: 'Màu thương hiệu', type: 'color', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'websiteUrl', label: 'Website', type: 'url', placeholder: 'https://...' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
