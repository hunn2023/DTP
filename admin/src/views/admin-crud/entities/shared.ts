import type { SettingsCrudLabels } from '@/views/settings/types'

export const labels = (itemName: string, search: string, add: string): SettingsCrudLabels => ({
  searchPlaceholder: search,
  addButton: add,
  emptyMessage: `Chưa có ${itemName} nào`,
  itemName,
})
