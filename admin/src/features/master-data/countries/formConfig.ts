import type { Country } from '@/features/master-data/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export const countryFormConfig: EntityFormConfig<Country> = {
  entityName: 'quốc gia',
  slugFromName: true,
  getDefaultValues: () => ({
    id: '',
    name: '',
    slug: '',
    isoCode: '',
    flagUrl: '',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên quốc gia', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true },
    { name: 'isoCode', label: 'Mã quốc gia (Code)', type: 'text', required: true, placeholder: 'JP', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'flagUrl', label: 'URL cờ (FlagUrl)', type: 'url' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
