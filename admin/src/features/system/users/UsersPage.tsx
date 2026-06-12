import { useUsersPage } from '@/features/system/users/useUsersPage'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const UsersPage = () => {
  const list = useUsersPage()

  return (
    <EntityPageLayout
      title="Tài khoản quản trị"
      subtitle="Hệ thống"
      description="Danh sách user từ API admin/users.">
      <PagedListTable
        searchPlaceholder="Tìm email, tên, SĐT..."
        loadingLabel="Đang tải tài khoản..."
        {...list}
      />
    </EntityPageLayout>
  )
}

export default UsersPage
