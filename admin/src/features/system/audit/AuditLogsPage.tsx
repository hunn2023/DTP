import type { AuditLogsQueryFilters } from '@/apis/auditLogsApi'
import AuditLogDetailModal from '@/features/system/audit/components/AuditLogDetailModal'
import { AuditLogsTable } from '@/features/system/audit/components/AuditLogsTable'
import { useAuditLogsPage } from '@/features/system/audit/useAuditLogsPage'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

type AuditLogsPageProps = {
  title: string
  description: string
  fixedFilters?: AuditLogsQueryFilters
}

const AuditLogsPage = ({ title, description, fixedFilters }: AuditLogsPageProps) => {
  const {
    filterForm,
    setFilterForm,
    queryFilterForm,
    detailOpen,
    detailLoading,
    detail,
    openDetail,
    closeDetail,
  } = useAuditLogsPage({ fixedFilters })

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
