import type { Category } from '@/features/master-data/types'

export const categoriesData: Category[] = [
  {
    id: '00000000-0000-0000-0000-000000000001',
    name: 'eSIM du lịch',
    slug: 'esim-du-lich',
    code: 'ESIM',
    icon: '✈️',
    description: 'Gói eSIM quốc tế cho khách du lịch',
    sortOrder: 1,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000002',
    name: 'Thẻ game',
    slug: 'the-game',
    code: 'GAME',
    icon: '🎮',
    description: 'Thẻ nạp game online, gift card',
    sortOrder: 2,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000003',
    name: 'Thẻ viễn thông',
    slug: 'the-vien-thong',
    code: 'TELCO',
    icon: '📱',
    description: 'Thẻ cào điện thoại, nạp tiền di động',
    sortOrder: 3,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000004',
    name: 'Data 4G/5G',
    slug: 'data-4g-5g',
    code: 'DATA',
    icon: '📶',
    description: 'Gói data trong nước, không roaming',
    sortOrder: 4,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000005',
    name: 'Dịch vụ số',
    slug: 'dich-vu-so',
    code: 'DIGITAL',
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
