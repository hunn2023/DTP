import type { Country } from '@/features/master-data/types'

export const countriesLabels = {
  searchPlaceholder: 'Tìm tên, mã quốc gia...',
  addButton: 'Thêm quốc gia',
  emptyMessage: 'Chưa có quốc gia nào',
  itemName: 'quốc gia',
} as const

/** Dropdown tạm cho module chưa nối API — dùng useCountriesQuery khi cần dữ liệu thật. */
export const countriesData: Country[] = []
