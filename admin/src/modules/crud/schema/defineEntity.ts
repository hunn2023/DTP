import { withIdFields } from '@/modules/crud/entities/fieldHelpers'
import { buildColumnsFromFields } from '@/modules/crud/schema/buildColumns'
import { buildFormConfigFromFields } from '@/modules/crud/schema/buildFormConfig'
import type { AdminEntityDefinition } from '@/modules/crud/types'
import { defaultCrudCapabilities } from '@/modules/crud/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { CrudEntityBase } from '@/modules/crud/types'

export type ResolvedAdminEntity<T extends CrudEntityBase> = AdminEntityDefinition<T> & {
  initialData: T[]
  buildColumns: AdminEntityDefinition<T>['buildColumns']
  formConfig: EntityFormConfig<T>
}

export function defineAdminEntity<T extends CrudEntityBase>(
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
    ((handlers: EntityTableHandlers<T>) => buildColumnsFromFields(entityFields, handlers, capabilities))

  return {
    ...def,
    capabilities,
    initialData,
    buildColumns,
    formConfig,
  }
}
