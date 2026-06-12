import { useCallback } from 'react'

import { fetchRoles } from '@/apis/rolesApi'
import { buildRoleColumns } from '@/features/system/roles/columns'
import { useFetchedList } from '@/features/system/shared/useFetchedList'

export function useRolesPage() {
  const fetchAll = useCallback(() => fetchRoles(), [])
  const buildColumns = useCallback(() => buildRoleColumns(), [])

  return useFetchedList({
    fetchAll,
    buildColumns,
    emptyMessage: 'Chưa có vai trò',
  })
}
