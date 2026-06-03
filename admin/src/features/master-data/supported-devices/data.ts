import type { SupportedDevice } from '@/features/master-data/types'

export const supportedDevicesData: SupportedDevice[] = [
  {
    id: 1,
    brandName: 'Apple',
    deviceName: 'iPhone XS',
    modelCode: 'A2097',
    supportEsim: true,
    note: 'Hỗ trợ eSIM quốc tế',
    isActive: true,
  },
  {
    id: 2,
    brandName: 'Apple',
    deviceName: 'iPhone 14 Pro',
    modelCode: 'A2890',
    supportEsim: true,
    note: 'Dual eSIM (một vật lý + một eSIM)',
    isActive: true,
  },
  {
    id: 3,
    brandName: 'Samsung',
    deviceName: 'Galaxy S23',
    modelCode: 'SM-S911B',
    supportEsim: true,
    note: 'Tùy phiên bản khu vực',
    isActive: true,
  },
  {
    id: 4,
    brandName: 'Google',
    deviceName: 'Pixel 7',
    modelCode: 'GVU6C',
    supportEsim: true,
    note: 'Pixel eSIM only trên một số carrier',
    isActive: true,
  },
  {
    id: 5,
    brandName: 'Xiaomi',
    deviceName: 'Redmi Note 12',
    modelCode: '22101316G',
    supportEsim: false,
    note: 'Chưa hỗ trợ eSIM — chỉ nano SIM',
    isActive: false,
  },
]

export const supportedDevicesLabels = {
  searchPlaceholder: 'Tìm hãng, model, mã thiết bị...',
  addButton: 'Thêm thiết bị',
  emptyMessage: 'Chưa có thiết bị nào',
  itemName: 'thiết bị',
} as const
