import type { Tag } from '@/views/settings/types'

export const tagsData: Tag[] = [
  { id: 1, name: 'Hot', slug: 'hot', color: '#ef4444', icon: '🔥', type: 'marketing', sortOrder: 1, isActive: true },
  { id: 2, name: 'Bán chạy', slug: 'ban-chay', color: '#f59e0b', icon: '⭐', type: 'marketing', sortOrder: 2, isActive: true },
  { id: 3, name: '5G', slug: '5g', color: '#3b82f6', icon: '📶', type: 'technical', sortOrder: 3, isActive: true },
  { id: 4, name: 'Unlimited', slug: 'unlimited', color: '#10b981', icon: '♾️', type: 'product', sortOrder: 4, isActive: true },
  { id: 5, name: 'Giảm giá', slug: 'giam-gia', color: '#8b5cf6', icon: '🏷️', type: 'marketing', sortOrder: 5, isActive: false },
]

const tagTypeLabels: Record<Tag['type'], string> = {
  product: 'Sản phẩm',
  marketing: 'Marketing',
  technical: 'Kỹ thuật',
}

export const getTagTypeLabel = (type: Tag['type']): string => tagTypeLabels[type]

export const tagsLabels = {
  searchPlaceholder: 'Tìm tên, slug, loại tag...',
  addButton: 'Thêm tag',
  emptyMessage: 'Chưa có tag nào',
  itemName: 'tag',
} as const
