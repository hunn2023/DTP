import type { Denomination } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

export const denominationFormConfig: SettingsFormConfig<Denomination> = {
  entityName: 'mệnh giá',
  getDefaultValues: () => ({
    id: 0,
    value: 0,
    displayName: '',
    currencyCode: 'VND',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'value', label: 'Giá trị (số)', type: 'number', required: true, placeholder: '100000' },
    { name: 'displayName', label: 'Tên hiển thị', type: 'text', required: true, placeholder: '100.000đ' },
    {
      name: 'currencyCode',
      label: 'Tiền tệ',
      type: 'select',
      required: true,
      options: [
        { value: 'VND', label: 'VND' },
        { value: 'USD', label: 'USD' },
      ],
    },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
