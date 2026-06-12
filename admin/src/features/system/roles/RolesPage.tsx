import { useCallback } from 'react'

import { buildRoleColumns } from '@/features/system/roles/columns'
import { fetchRoles } from '@/apis/rolesApi'
import SystemListTable from '@/features/system/shared/SystemListTable'
import { useFetchedList } from '@/features/system/shared/useFetchedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const RolesPage = () => {
  const fetchAll = useCallback(() => fetchRoles(), [])
  const buildColumns = useCallback(() => buildRoleColumns(), [])

  const list = useFetchedList({
    fetchAll,
    buildColumns,
    emptyMessage: 'Chưa có vai trò',
  })

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
