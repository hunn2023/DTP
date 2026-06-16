export type FormFieldType =
  | 'text'
  | 'textarea'
  | 'number'
  | 'checkbox'
  | 'color'
  | 'url'
  | 'date'
  | 'select'
  | 'multiselect'
  | 'countrySearch'
  | 'password'

export type FormFieldOption = {
  value: string
  label: string
}

export type FormFieldConfig<T> = {
  name: keyof T & string
  label?: string
  type: FormFieldType
  required?: boolean
  placeholder?: string
  options?: FormFieldOption[]
  col?: 6 | 12
  hint?: string
  parseAsNumber?: boolean
}

export type FormModalMode = 'create' | 'edit' | 'view'

export type EntityFormConfig<T extends { isActive: boolean }> = {
  entityName: string
  fields: FormFieldConfig<T>[]
  viewFields?: FormFieldConfig<T>[]
  getDefaultValues: () => T
  slugFromName?: boolean
  onBeforeSave?: (values: T, mode: FormModalMode) => T
}

/** @deprecated Use EntityFormConfig */
export type SettingsFormConfig<T extends { isActive: boolean }> = EntityFormConfig<T>
