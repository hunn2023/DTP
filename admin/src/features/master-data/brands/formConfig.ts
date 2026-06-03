import type { Brand } from '@/features/master-data/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export const brandFormConfig: EntityFormConfig<Brand> = {
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
    { name: 'logoUrl', label: 'URL logo', type: 'url', placeholder: 'https://...' },
    { name: 'brandColor', label: 'Màu thương hiệu', type: 'color', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'websiteUrl', label: 'Website', type: 'url', placeholder: 'https://...' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
