export const seoLabels = {
  searchPlaceholder: 'Tìm meta title, route...',
  addButton: 'Thêm SEO',
  emptyMessage: 'Chưa có cấu hình SEO',
  itemName: 'cấu hình SEO',
} as const

export const SEO_ENTITY_TYPE_OPTIONS = [
  { value: 'homepage', label: 'Trang chủ' },
  { value: 'category', label: 'Danh mục' },
  { value: 'product', label: 'Sản phẩm' },
  { value: 'article', label: 'Bài viết' },
  { value: 'page', label: 'Trang tĩnh' },
  { value: 'country', label: 'Quốc gia' },
] as const

export function getSeoEntityTypeLabel(entityType: string): string {
  const match = SEO_ENTITY_TYPE_OPTIONS.find((item) => item.value === entityType)
  return match?.label ?? entityType
}
