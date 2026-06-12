import { useCallback } from 'react'

import { fetchUsersPage } from '@/apis/usersApi'
import { buildUserColumns } from '@/features/system/users/columns'
import { usePagedList } from '@/features/sales/shared/usePagedList'

export function useUsersPage() {
  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      fetchUsersPage(pageIndex + 1, pageSize, keyword),
    [],
  )

  const buildColumns = useCallback(() => buildUserColumns(), [])

  return usePagedList({
    fetchPage,
    buildColumns,
    emptyMessage: 'Chưa có tài khoản',
  })
}
