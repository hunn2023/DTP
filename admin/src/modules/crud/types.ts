import type { EntityFormConfig } from '@/modules/crud/form/types'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { ColumnDef } from '@tanstack/react-table'
import type { FormFieldType } from '@/modules/crud/form/types'

export type CrudEntityBase = {
  id: number
  isActive: boolean
}

export type EntityCrudLabels = {
  searchPlaceholder: string
  addButton: string
  emptyMessage: string
  itemName: string
  deleteTitle?: string
  deleteConfirm?: string
}

export type CrudCapabilities = {
  create?: boolean
  edit?: boolean
  view?: boolean
  delete?: boolean
  toggleActive?: boolean
}

export const defaultCrudCapabilities: CrudCapabilities = {
  create: true,
  edit: true,
  view: true,
  delete: true,
  toggleActive: true,
}

export const readonlyCrudCapabilities: CrudCapabilities = {
  create: false,
  edit: false,
  view: true,
  delete: false,
  toggleActive: false,
}

export type EntityFieldDef<T> = {
  name: keyof T & string
  label?: string
  type: FormFieldType | 'badge' | 'date' | 'multiselect'
  table?: boolean | { variant?: 'primary' | 'code' | 'badge'; badgeMap?: Record<string, string> }
  form?: false | {
    required?: boolean
    col?: 6 | 12
    options?: { value: string; label: string }[]
    parseAsNumber?: boolean
    hint?: string
    placeholder?: string
  }
}

export type AdminEntityDefinition<T extends CrudEntityBase> = {
  path: string
  title: string
  description: string
  breadcrumbSubtitle: string
  entityName: string
  labels: EntityCrudLabels
  seedData: T[]
  fields: EntityFieldDef<T>[]
  capabilities?: CrudCapabilities
  slugFromName?: boolean
  onBeforeSave?: EntityFormConfig<T>['onBeforeSave']
  dataFilter?: (rows: T[]) => T[]
  buildColumns?: (handlers: EntityTableHandlers<T>) => ColumnDef<T>[]
  formConfig?: EntityFormConfig<T>
}

/** @deprecated Use CrudEntityBase */
export type SettingsEntityBase = CrudEntityBase

/** @deprecated Use EntityCrudLabels */
export type SettingsCrudLabels = EntityCrudLabels
