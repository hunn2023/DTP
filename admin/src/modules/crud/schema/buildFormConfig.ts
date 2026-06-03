import { getFieldLabel } from '@/modules/crud/entities/fieldLabels'
import type { FormFieldConfig, FormFieldType, EntityFormConfig } from '@/modules/crud/form/types'
import type { CrudEntityBase } from '@/modules/crud/types'
import type { EntityFieldDef } from '@/modules/crud/types'

function toFormFieldType(type: EntityFieldDef<CrudEntityBase>['type']): FormFieldType {
  if (type === 'badge' || type === 'date') return 'text'
  return type
}

function isSyntheticIsActiveField<T extends CrudEntityBase>(field: EntityFieldDef<T>): boolean {
  return field.name === 'isActive' && field.form === false && !field.table
}

type EntityFormMeta = {
  required?: boolean
  col?: 6 | 12
  options?: { value: string; label: string }[]
  parseAsNumber?: boolean
  hint?: string
  placeholder?: string
}

function resolveFormMeta<T extends CrudEntityBase>(field: EntityFieldDef<T>): EntityFormMeta {
  if (field.form === false || field.form === undefined) return {}
  return field.form
}

function mapEntityFieldToFormField<T extends CrudEntityBase>(
  field: EntityFieldDef<T>,
  formMeta: EntityFormMeta,
): FormFieldConfig<T> {
  return {
    name: field.name,
    label: field.label ?? getFieldLabel(field.name),
    type: toFormFieldType(field.type),
    required: formMeta.required,
    col: formMeta.col,
    options: formMeta.options,
    parseAsNumber: formMeta.parseAsNumber,
    hint: formMeta.hint,
    placeholder: formMeta.placeholder,
  }
}

/** Cột chi tiết cho modal view (kể cả field chỉ có trên bảng, log, textarea). */
export function buildViewFieldsFromEntityFields<T extends CrudEntityBase>(
  fields: EntityFieldDef<T>[],
): FormFieldConfig<T>[] {
  return fields
    .filter((field) => !isSyntheticIsActiveField(field))
    .map((field) => mapEntityFieldToFormField(field, resolveFormMeta(field)))
}

export function buildFormConfigFromFields<T extends CrudEntityBase>(
  entityName: string,
  fields: EntityFieldDef<T>[],
  getDefaultValues: () => T,
  slugFromName?: boolean,
  onBeforeSave?: EntityFormConfig<T>['onBeforeSave'],
): EntityFormConfig<T> {
  const formFields: FormFieldConfig<T>[] = fields
    .filter((field) => field.form !== false)
    .map((field) => mapEntityFieldToFormField(field, resolveFormMeta(field)))

  return {
    entityName,
    fields: formFields,
    viewFields: buildViewFieldsFromEntityFields(fields),
    getDefaultValues,
    slugFromName,
    onBeforeSave,
  }
}
