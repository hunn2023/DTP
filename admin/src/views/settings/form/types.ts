import type { SettingsEntityBase } from '@/views/settings/types'

export type FormFieldType =
  | 'text'
  | 'textarea'
  | 'number'
  | 'checkbox'
  | 'color'
  | 'url'
  | 'select'
  | 'multiselect'
  | 'password'

export type FormFieldOption = {
  value: string
  label: string
}

export type FormFieldConfig<T> = {
  name: keyof T & string
  /** Omit to use English label from getFieldLabel(name). */
  label?: string
  type: FormFieldType
  required?: boolean
  placeholder?: string
  options?: FormFieldOption[]
  col?: 6 | 12
  hint?: string
  /** Select trả về number (vd. countryId) */
  parseAsNumber?: boolean
}

export type FormModalMode = 'create' | 'edit' | 'view'

export type SettingsFormConfig<T extends SettingsEntityBase> = {
  entityName: string
  fields: FormFieldConfig<T>[]
  getDefaultValues: () => T
  /** Tự sinh slug từ name khi tạo mới */
  slugFromName?: boolean
  /** Chuẩn hóa dữ liệu trước khi lưu (vd. map countryId → countryName) */
  onBeforeSave?: (values: T, mode: FormModalMode) => T
}
