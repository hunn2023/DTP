import { useCallback, useEffect, useState } from 'react'

import { fetchPermissions, fetchPermissionsByModule, type PermissionsByModule } from '@/apis/permissionsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { buildPermissionColumns } from '@/features/system/permissions/columns'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import { useFetchedList } from '@/features/system/shared/useFetchedList'

export function usePermissionsPage() {
  const { showNotification } = useNotificationContext()
  const [activeTab, setActiveTab] = useState<'list' | 'module'>('list')
  const [grouped, setGrouped] = useState<PermissionsByModule>({})
  const [moduleLoading, setModuleLoading] = useState(false)

  const fetchAll = useCallback(() => fetchPermissions(), [])
  const buildColumns = useCallback(() => buildPermissionColumns(), [])

  const list = useFetchedList({
    fetchAll,
    buildColumns,
    emptyMessage: 'Chưa có quyền',
  })

  useEffect(() => {
    if (activeTab !== 'module') return

    let cancelled = false
    setModuleLoading(true)
    void fetchPermissionsByModule()
      .then((data) => {
        if (!cancelled) setGrouped(data)
      })
      .catch((error) => {
        if (cancelled) return
        showNotification({
          title: 'Lỗi',
          message: getErrorMessage(error, 'Không tải được quyền theo module'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => {
        if (!cancelled) setModuleLoading(false)
      })

    return () => {
      cancelled = true
    }
  }, [activeTab, showNotification])

  return {
    activeTab,
    setActiveTab,
    grouped,
    moduleLoading,
    list,
  }
}
