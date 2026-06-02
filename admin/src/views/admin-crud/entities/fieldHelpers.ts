import type { EntityFieldDef } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'

/** Table-only ID column (hidden on create form). */
export function idField<T extends SettingsEntityBase>(): EntityFieldDef<T> {
  return { name: 'id', type: 'number', table: true, form: false }
}

export function isActiveField<T extends SettingsEntityBase>(col: 6 | 12 = 12): EntityFieldDef<T> {
  return { name: 'isActive', type: 'checkbox', form: { col }, table: false }
}

export function sortOrderField<T extends { sortOrder: number }>(col: 6 | 12 = 6): EntityFieldDef<T> {
  return { name: 'sortOrder', type: 'number', table: true, form: { required: true, col } }
}

export function withIdFields<T extends SettingsEntityBase>(fields: EntityFieldDef<T>[]): EntityFieldDef<T>[] {
  if (fields.some((f) => f.name === 'id')) return fields
  return [idField<T>(), ...fields]
}

export const statusOptions = (map: Record<string, string>) =>
  Object.entries(map).map(([value, label]) => ({ value, label }))

export const saleStatusMap = { active: 'Đang bán', hidden: 'Ẩn', draft: 'Nháp' }
export const providerStatusMap = { active: 'Hoạt động', inactive: 'Ngừng hoạt động' }

export function statusField<T extends SettingsEntityBase & { status: string }>(
  table = true,
): EntityFieldDef<T> {
  return {
    name: 'status',
    type: 'select',
    table: table ? { variant: 'badge', badgeMap: saleStatusMap } : false,
    form: { required: true, options: statusOptions(saleStatusMap) },
  }
}
