import type { Carrier } from '@/features/master-data/types'

export const carriersLabels = {
  searchPlaceholder: 'Tìm nhà mạng, mã...',
  addButton: 'Thêm nhà mạng',
  emptyMessage: 'Chưa có nhà mạng nào',
  itemName: 'nhà mạng',
} as const

/** Dropdown tạm cho module chưa nối API. */
export const carriersData: Carrier[] = []
