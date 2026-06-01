import type { Brand } from '@/views/settings/types'

export const brandsData: Brand[] = [
  {
    id: 1,
    name: 'Viettel',
    slug: 'viettel',
    logoUrl: 'https://upload.wikimedia.org/wikipedia/commons/f/fe/Viettel_logo_2021.svg',
    brandColor: '#ee0033',
    websiteUrl: 'https://viettel.com.vn',
    sortOrder: 1,
    isActive: true,
  },
  {
    id: 2,
    name: 'Garena',
    slug: 'garena',
    logoUrl: 'https://cdn.worldvectorlogo.com/logos/garena-1.svg',
    brandColor: '#e4002b',
    websiteUrl: 'https://garena.vn',
    sortOrder: 2,
    isActive: true,
  },
  {
    id: 3,
    name: 'Steam',
    slug: 'steam',
    logoUrl: 'https://upload.wikimedia.org/wikipedia/commons/8/83/Steam_icon_logo.svg',
    brandColor: '#1b2838',
    websiteUrl: 'https://store.steampowered.com',
    sortOrder: 3,
    isActive: true,
  },
  {
    id: 4,
    name: 'SoftBank',
    slug: 'softbank',
    logoUrl: 'https://upload.wikimedia.org/wikipedia/commons/5/5c/SoftBank_Mobile_logo.svg',
    brandColor: '#bdbdbd',
    websiteUrl: 'https://www.softbank.jp',
    sortOrder: 4,
    isActive: false,
  },
]

export const brandsLabels = {
  searchPlaceholder: 'Tìm tên, slug, website...',
  addButton: 'Thêm brand',
  emptyMessage: 'Chưa có brand nào',
  itemName: 'brand',
} as const
