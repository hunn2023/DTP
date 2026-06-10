import { useCallback } from 'react'

import { buildUserColumns } from '@/features/system/users/columns'
import { fetchUsersPage } from '@/features/system/users/users.api'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import { usePagedList } from '@/features/sales/shared/usePagedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const UsersPage = () => {
  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      fetchUsersPage(pageIndex + 1, pageSize, keyword),
    [],
  )

  const buildColumns = useCallback(() => buildUserColumns(), [])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    emptyMessage: 'Chưa có tài khoản',
  })

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
