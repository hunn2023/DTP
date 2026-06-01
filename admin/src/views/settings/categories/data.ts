import type { Category } from '@/views/settings/types'

export const categoriesData: Category[] = [
  {
    id: 1,
    name: 'eSIM du lịch',
    slug: 'esim-du-lich',
    icon: '✈️',
    description: 'Gói eSIM quốc tế cho khách du lịch',
    sortOrder: 1,
    isActive: true,
  },
  {
    id: 2,
    name: 'Thẻ game',
    slug: 'the-game',
    icon: '🎮',
    description: 'Thẻ nạp game online, gift card',
    sortOrder: 2,
    isActive: true,
  },
  {
    id: 3,
    name: 'Thẻ viễn thông',
    slug: 'the-vien-thong',
    icon: '📱',
    description: 'Thẻ cào điện thoại, nạp tiền di động',
    sortOrder: 3,
    isActive: true,
  },
  {
    id: 4,
    name: 'Data 4G/5G',
    slug: 'data-4g-5g',
    icon: '📶',
    description: 'Gói data trong nước, không roaming',
    sortOrder: 4,
    isActive: true,
  },
  {
    id: 5,
    name: 'Dịch vụ số',
    slug: 'dich-vu-so',
    icon: '💳',
    description: 'Voucher, subscription số',
    sortOrder: 5,
    isActive: false,
  },
]

export const categoriesLabels = {
  searchPlaceholder: 'Tìm tên, slug, mô tả...',
  addButton: 'Thêm danh mục',
  emptyMessage: 'Chưa có danh mục nào',
  itemName: 'danh mục',
} as const
