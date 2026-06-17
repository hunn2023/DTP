import { Alert, Spinner } from 'react-bootstrap'
import type { ReactNode } from 'react'

import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'
import ReportDateFilter from '@/features/reports/components/ReportDateFilter'
import type { ReportFilters } from '@/features/reports/reportTypes'

import '../reports.scss'

type ReportPageShellProps = {
  title: string
  description?: string
  filters: ReportFilters
  onFiltersChange: (filters: ReportFilters) => void
  onApplyFilters: () => void
  isLoading: boolean
  error: string | null
  showGroupType?: boolean
  children: ReactNode
}

const ReportPageShell = ({
  title,
  description,
  filters,
  onFiltersChange,
  onApplyFilters,
  isLoading,
  error,
  showGroupType = true,
  children,
}: ReportPageShellProps) => (
  <EntityPageLayout title={title} subtitle="Báo cáo" description={description}>
    <ReportDateFilter
      value={filters}
      onChange={onFiltersChange}
      onApply={onApplyFilters}
      isLoading={isLoading}
      showGroupType={showGroupType}
    />
    {error && (
      <Alert variant="danger" className="mb-3 border-0 shadow-sm">
        {error}
      </Alert>
    )}
    {isLoading && !error ? (
      <div className="text-center py-5">
        <Spinner animation="border" size="sm" className="me-2 text-primary" />
        <span className="text-muted">Đang tải báo cáo...</span>
      </div>
    ) : (
      !error && <div className="report-page-content">{children}</div>
    )}
  </EntityPageLayout>
)

export default ReportPageShell
