import { useRolesPage } from '@/features/system/roles/useRolesPage'
import SystemListTable from '@/features/system/shared/SystemListTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const RolesPage = () => {
  const list = useRolesPage()

  return (
    <EntityPageLayout
      title="Vai trò"
      subtitle="Hệ thống"
      description="Danh sách role từ API admin/roles.">
      <SystemListTable
        searchPlaceholder="Tìm tên, mã role..."
        loadingLabel="Đang tải vai trò..."
        {...list}
      />
    </EntityPageLayout>
  )
}

export default RolesPage
