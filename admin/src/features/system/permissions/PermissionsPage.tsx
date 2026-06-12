import { usePermissionsPage } from '@/features/system/permissions/usePermissionsPage'
import SystemListTable from '@/features/system/shared/SystemListTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PermissionsPage = () => {
  const list = usePermissionsPage()

  return (
    <EntityPageLayout
      title="Phân quyền"
      subtitle="Hệ thống"
      description="Danh sách permission từ API admin/permissions.">
      <SystemListTable
        searchPlaceholder="Tìm code, tên, module..."
        loadingLabel="Đang tải phân quyền..."
        {...list}
      />
    </EntityPageLayout>
  )
}

export default PermissionsPage
