import { useCallback, useMemo, useState } from 'react'

import { fetchAuditLogById, type AuditLogDetail, type AuditLogRow, type AuditLogsQueryFilters } from '@/apis/auditLogsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  defaultAuditLogFilterForm,
  type AuditLogFilterForm,
} from '@/features/system/audit/auditFilterTypes'

type UseAuditLogsPageParams = {
  fixedFilters?: AuditLogsQueryFilters
}

const EMPTY_FIXED_FILTERS: AuditLogsQueryFilters = {}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function applyFixedFilters(
  form: AuditLogFilterForm,
  fixedFilters: AuditLogsQueryFilters,
): AuditLogFilterForm {
  return {
    ...form,
    actionType:
      fixedFilters.actionType != null ? String(fixedFilters.actionType) : form.actionType,
    module: fixedFilters.module ?? form.module,
    status: fixedFilters.status != null ? String(fixedFilters.status) : form.status,
  }
}

export function useAuditLogsPage({ fixedFilters = EMPTY_FIXED_FILTERS }: UseAuditLogsPageParams = {}) {
  const { showNotification } = useNotificationContext()
  const [filterForm, setFilterForm] = useState<AuditLogFilterForm>(defaultAuditLogFilterForm)

  const queryFilterForm = useMemo(
    () => applyFixedFilters(filterForm, fixedFilters),
    [filterForm, fixedFilters],
  )

  const [detailOpen, setDetailOpen] = useState(false)
  const [detailLoading, setDetailLoading] = useState(false)
  const [detail, setDetail] = useState<AuditLogDetail | null>(null)

  const openDetail = useCallback(
    async (row: AuditLogRow) => {
      setDetailOpen(true)
      setDetail(null)
      setDetailLoading(true)
      try {
        const data = await fetchAuditLogById(row.id)
        setDetail(data)
      } catch (error) {
        setDetailOpen(false)
        showNotification({
          title: 'Lỗi',
          message: getErrorMessage(error, 'Không tải được chi tiết nhật ký'),
          variant: 'danger',
          delay: 4000,
        })
      } finally {
        setDetailLoading(false)
      }
    },
    [showNotification],
  )

  const closeDetail = useCallback(() => {
    setDetailOpen(false)
    setDetail(null)
  }, [])

  return {
    filterForm,
    setFilterForm,
    queryFilterForm,
    detailOpen,
    detailLoading,
    detail,
    openDetail,
    closeDetail,
  }
}
