import type { SettingsCrudLabels, SettingsEntityBase } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { ColumnDef } from '@tanstack/react-table'
import type { FormFieldType } from '@/views/settings/form/types'

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
  /** Omit to use English label from getFieldLabel(name). */
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

export type AdminEntityDefinition<T extends SettingsEntityBase> = {
  path: string
  title: string
  description: string
  breadcrumbSubtitle: string
  entityName: string
  labels: SettingsCrudLabels
  seedData: T[]
  fields: EntityFieldDef<T>[]
  capabilities?: CrudCapabilities
  slugFromName?: boolean
  onBeforeSave?: SettingsFormConfig<T>['onBeforeSave']
  dataFilter?: (rows: T[]) => T[]
  buildColumns?: (handlers: SettingsTableHandlers<T>) => ColumnDef<T>[]
  formConfig?: SettingsFormConfig<T>
}
