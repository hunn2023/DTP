import type { Category } from '@/features/master-data/types'

export const categoriesData: Category[] = [
  {
    id: '00000000-0000-0000-0000-000000000001',
    name: 'eSIM du lịch',
    slug: 'esim-du-lich',
    code: 'ESIM',
    sortOrder: 1,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000002',
    name: 'Thẻ game',
    slug: 'the-game',
    code: 'GAME',
    sortOrder: 2,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000003',
    name: 'Thẻ viễn thông',
    slug: 'the-vien-thong',
    code: 'TELCO',
    sortOrder: 3,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000004',
    name: 'Data 4G/5G',
    slug: 'data-4g-5g',
    code: 'DATA',
    sortOrder: 4,
    isActive: true,
  },
  {
    id: '00000000-0000-0000-0000-000000000005',
    name: 'Dịch vụ số',
    slug: 'dich-vu-so',
    code: 'DIGITAL',
    sortOrder: 5,
    isActive: false,
  },
]

export const categoriesLabels = {
  searchPlaceholder: 'Tìm tên, slug, mã...',
  addButton: 'Thêm danh mục',
  emptyMessage: 'Chưa có danh mục nào',
  itemName: 'danh mục',
} as const
