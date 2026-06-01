import { withIdFields } from '@/views/admin-crud/entities/fieldHelpers'
import { buildColumnsFromFields } from '@/views/admin-crud/schema/buildColumns'
import { buildFormConfigFromFields } from '@/views/admin-crud/schema/buildFormConfig'
import type { AdminEntityDefinition } from '@/views/admin-crud/types'
import { defaultCrudCapabilities } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'

export type ResolvedAdminEntity<T extends SettingsEntityBase> = AdminEntityDefinition<T> & {
  initialData: T[]
  buildColumns: AdminEntityDefinition<T>['buildColumns']
  formConfig: SettingsFormConfig<T>
}

export function defineAdminEntity<T extends SettingsEntityBase>(
  def: AdminEntityDefinition<T>,
): ResolvedAdminEntity<T> {
  const capabilities = { ...defaultCrudCapabilities, ...def.capabilities }
  const initialData = def.dataFilter ? def.dataFilter([...def.seedData]) : [...def.seedData]

  const getDefaultValues = () => {
    const base = def.seedData[0]
    if (base) {
      const draft = { ...base, id: 0 } as T
      if ('tagIds' in draft && Array.isArray((draft as T & { tagIds: number[] }).tagIds)) {
        return { ...draft, tagIds: [] } as T
      }
      return draft
    }
    return def.formConfig?.getDefaultValues() ?? ({ id: 0, isActive: true } as T)
  }

  const entityFields = withIdFields(def.fields)

  const formConfig =
    def.formConfig ??
    buildFormConfigFromFields(def.entityName, entityFields, getDefaultValues, def.slugFromName, def.onBeforeSave)

  const buildColumns =
    def.buildColumns ??
    ((handlers: SettingsTableHandlers<T>) => buildColumnsFromFields(entityFields, handlers, capabilities))

  return {
    ...def,
    capabilities,
    initialData,
    buildColumns,
    formConfig,
  }
}
