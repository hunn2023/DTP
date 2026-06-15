import UsersCrudTable from '@/features/system/users/components/UsersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const UsersPage = () => (
  <EntityPageLayout
    title="Tài khoản quản trị"
    subtitle="Hệ thống"
    description="Quản lý user, khóa/mở khóa và gán vai trò.">
    <UsersCrudTable />
  </EntityPageLayout>
)

export default UsersPage
