import type { Country } from '@/features/master-data/types'
import { COUNTRY_REGION_OPTIONS } from '@/features/master-data/countries/regionOptions'
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
    region: '',
    description: '',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên quốc gia', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true, hint: 'Tự gợi ý từ tên khi tạo mới' },
    { name: 'isoCode', label: 'Mã quốc gia (Code)', type: 'text', required: true, placeholder: 'JP', col: 6 },
    { name: 'region', label: 'Khu vực', type: 'select', options: COUNTRY_REGION_OPTIONS, col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'description', label: 'Mô tả', type: 'textarea', placeholder: 'Mô tả ngắn về quốc gia' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
