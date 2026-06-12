import { useCallback } from 'react'

import { buildPermissionColumns } from '@/features/system/permissions/columns'
import { fetchPermissions } from '@/apis/permissionsApi'
import SystemListTable from '@/features/system/shared/SystemListTable'
import { useFetchedList } from '@/features/system/shared/useFetchedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PermissionsPage = () => {
  const fetchAll = useCallback(() => fetchPermissions(), [])
  const buildColumns = useCallback(() => buildPermissionColumns(), [])

  const list = useFetchedList({
    fetchAll,
    buildColumns,
    emptyMessage: 'Chưa có quyền',
  })

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
