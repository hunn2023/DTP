import { useCallback } from 'react'

import { fetchPermissions } from '@/apis/permissionsApi'
import { buildPermissionColumns } from '@/features/system/permissions/columns'
import { useFetchedList } from '@/features/system/shared/useFetchedList'

export function usePermissionsPage() {
  const fetchAll = useCallback(() => fetchPermissions(), [])
  const buildColumns = useCallback(() => buildPermissionColumns(), [])

  return useFetchedList({
    fetchAll,
    buildColumns,
    emptyMessage: 'Chưa có quyền',
  })
}
