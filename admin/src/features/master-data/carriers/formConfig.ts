import type { Carrier } from '@/features/master-data/types'
import type { EntityFormConfig, FormFieldOption } from '@/modules/crud/form/types'

export function getDefaultCarrierValues(): Carrier {
  return {
    id: '',
    name: '',
    slug: '',
    code: '',
    countryId: '',
    countryName: '',
    countryFlagUrl: '',
    logoUrl: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function buildCarrierFormConfig(countryOptions: FormFieldOption[]): EntityFormConfig<Carrier> {
  return {
    entityName: 'nhà mạng',
    slugFromName: true,
    getDefaultValues: getDefaultCarrierValues,
    fields: [
      { name: 'name', label: 'Tên nhà mạng', type: 'text', required: true },
      { name: 'slug', label: 'Slug', type: 'text', required: true },
      { name: 'code', label: 'Mã (Code)', type: 'text', placeholder: 'SOFTBANK', col: 6 },
      {
        name: 'countryId',
        label: 'Quốc gia',
        type: 'select',
        required: true,
        col: 6,
        options: countryOptions,
      },
      { name: 'logoUrl', label: 'URL logo', type: 'url' },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 6 },
    ],
  }
}
