import { countriesData } from '@/views/settings/countries/data'
import type { Carrier } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

const countryOptions = countriesData.map((c) => ({
  value: String(c.id),
  label: `${c.flagEmoji} ${c.name}`,
}))

export const carrierFormConfig: SettingsFormConfig<Carrier> = {
  entityName: 'nhà mạng',
  slugFromName: true,
  getDefaultValues: () => ({
    id: 0,
    name: '',
    slug: '',
    countryId: countriesData[0]?.id ?? 1,
    countryName: countriesData[0]?.name ?? '',
    logoUrl: '',
    support5G: false,
    coverageNote: '',
    sortOrder: 1,
    isActive: true,
  }),
  onBeforeSave: (values) => ({
    ...values,
    countryId: Number(values.countryId),
    countryName: countriesData.find((c) => c.id === Number(values.countryId))?.name ?? values.countryName,
  }),
  fields: [
    { name: 'name', label: 'Tên nhà mạng', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true },
    { name: 'countryId', label: 'Quốc gia', type: 'select', required: true, parseAsNumber: true, options: countryOptions },
    { name: 'logoUrl', label: 'URL logo', type: 'url' },
    { name: 'support5G', label: 'Hỗ trợ 5G', type: 'checkbox', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'coverageNote', label: 'Ghi chú phủ sóng', type: 'textarea' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
