import { getFieldLabel } from '@/views/admin-crud/entities/fieldLabels'
import type { SettingsEntityBase } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'
import type { EntityFieldDef } from '@/views/admin-crud/types'

export function buildFormConfigFromFields<T extends SettingsEntityBase>(
  entityName: string,
  fields: EntityFieldDef<T>[],
  getDefaultValues: () => T,
  slugFromName?: boolean,
  onBeforeSave?: SettingsFormConfig<T>['onBeforeSave'],
): SettingsFormConfig<T> {
  const formFields = fields
    .filter((f) => f.form !== false)
    .map((f) => {
      const form = f.form === false || f.form === undefined ? {} : f.form
      return {
        name: f.name,
        label: getFieldLabel(f.name),
        type: f.type === 'badge' || f.type === 'date' ? 'text' : f.type === 'multiselect' ? 'multiselect' : f.type,
        required: form.required,
        col: form.col,
        options: form.options,
        parseAsNumber: form.parseAsNumber,
        hint: form.hint,
        placeholder: form.placeholder,
      }
    })

  return {
    entityName,
    fields: formFields,
    getDefaultValues,
    slugFromName,
    onBeforeSave,
  }
}
