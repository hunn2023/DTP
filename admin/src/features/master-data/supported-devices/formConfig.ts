import type { SupportedDevice } from '@/features/master-data/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export const supportedDeviceFormConfig: EntityFormConfig<SupportedDevice> = {
  entityName: 'thiết bị',
  getDefaultValues: () => ({
    id: 0,
    brandName: '',
    deviceName: '',
    modelCode: '',
    supportEsim: true,
    note: '',
    isActive: true,
  }),
  fields: [
    { name: 'brandName', label: 'Hãng', type: 'text', required: true, placeholder: 'Apple' },
    { name: 'deviceName', label: 'Tên thiết bị', type: 'text', required: true, placeholder: 'iPhone XS' },
    { name: 'modelCode', label: 'Mã model', type: 'text', required: true, placeholder: 'A2097' },
    { name: 'supportEsim', label: 'Hỗ trợ eSIM', type: 'checkbox', col: 6 },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 6 },
    { name: 'note', label: 'Ghi chú', type: 'textarea' },
  ],
}
