import RolesCrudTable from '@/features/system/roles/components/RolesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const RolesPage = () => (
  <EntityPageLayout
    title="Vai trò"
    subtitle="Hệ thống"
    description="Quản lý role, trạng thái và gán permission.">
    <RolesCrudTable />
  </EntityPageLayout>
)

export default RolesPage
