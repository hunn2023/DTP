import SettingsCrudTable from '@/views/settings/components/SettingsCrudTable'
import SettingsPageLayout from '@/views/settings/components/SettingsPageLayout'
import type { ResolvedAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import type { SettingsEntityBase } from '@/views/settings/types'

export function createCrudPage<T extends SettingsEntityBase>(entity: ResolvedAdminEntity<T>) {
  const Page = () => (
    <SettingsPageLayout title={entity.title} subtitle={entity.breadcrumbSubtitle} description={entity.description}>
      <SettingsCrudTable
        initialData={entity.initialData}
        buildColumns={entity.buildColumns!}
        formConfig={entity.formConfig}
        labels={entity.labels}
        capabilities={entity.capabilities}
      />
    </SettingsPageLayout>
  )
  Page.displayName = entity.title
  return Page
}
