import type { Denomination } from '@/views/settings/types'

export const denominationsData: Denomination[] = [
  { id: 1, value: 10000, displayName: '10.000đ', currencyCode: 'VND', sortOrder: 1, isActive: true },
  { id: 2, value: 20000, displayName: '20.000đ', currencyCode: 'VND', sortOrder: 2, isActive: true },
  { id: 3, value: 50000, displayName: '50.000đ', currencyCode: 'VND', sortOrder: 3, isActive: true },
  { id: 4, value: 100000, displayName: '100.000đ', currencyCode: 'VND', sortOrder: 4, isActive: true },
  { id: 5, value: 200000, displayName: '200.000đ', currencyCode: 'VND', sortOrder: 5, isActive: true },
  { id: 6, value: 500000, displayName: '500.000đ', currencyCode: 'VND', sortOrder: 6, isActive: false },
]

export const denominationsLabels = {
  searchPlaceholder: 'Tìm mệnh giá, hiển thị...',
  addButton: 'Thêm mệnh giá',
  emptyMessage: 'Chưa có mệnh giá nào',
  itemName: 'mệnh giá',
} as const
