import { useCallback, useMemo, useState } from 'react'

import AuditLogDetailModal from '@/features/system/audit/AuditLogDetailModal'
import { defaultAuditLogFilterForm } from '@/features/system/audit/auditFilterTypes'
import { AuditLogsTable } from '@/features/system/audit/AuditLogsTable'
import {
  fetchAuditLogById,
  type AuditLogDetail,
  type AuditLogRow,
  type AuditLogsQueryFilters,
} from '@/apis/auditLogsApi'
import type { AuditLogFilterForm } from '@/features/system/audit/auditFilterTypes'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'
import { useNotificationContext } from '@/context/useNotificationContext'

type AuditLogsPageProps = {
  title: string
  description: string
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

const AuditLogsPage = ({ title, description, fixedFilters = EMPTY_FIXED_FILTERS }: AuditLogsPageProps) => {
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

  return (
    <EntityPageLayout title={title} subtitle="Hệ thống" description={description}>
      <AuditLogsTable
        filterForm={filterForm}
        queryFilterForm={queryFilterForm}
        onFilterChange={setFilterForm}
        onRowClick={(row) => void openDetail(row)}
      />
      <AuditLogDetailModal
        open={detailOpen}
        loading={detailLoading}
        detail={detail}
        onClose={closeDetail}
      />
    </EntityPageLayout>
  )
}

export default AuditLogsPage
