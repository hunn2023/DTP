import type { EntityCrudLabels } from '@/modules/crud/types'

export const labels = (itemName: string, search: string, add: string): EntityCrudLabels => ({
  searchPlaceholder: search,
  addButton: add,
  emptyMessage: `Chưa có ${itemName} nào`,
  itemName,
})
