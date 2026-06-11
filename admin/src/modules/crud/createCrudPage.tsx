import EntityCrudTable from '@/modules/crud/components/EntityCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'
import type { ResolvedAdminEntity } from '@/modules/crud/schema/defineEntity'
import type { CrudEntityBase } from '@/modules/crud/types'

export function createCrudPage<T extends CrudEntityBase>(entity: ResolvedAdminEntity<T>) {
  const Page = () => (
    <EntityPageLayout title={entity.title} subtitle={entity.breadcrumbSubtitle} description={entity.description}>
      <EntityCrudTable
        initialData={entity.initialData}
        buildColumns={entity.buildColumns!}
        formConfig={entity.formConfig}
        labels={entity.labels}
        capabilities={entity.capabilities}
      />
    </EntityPageLayout>
  )
  Page.displayName = entity.title
  return Page
}
