import { Card, CardBody, CardFooter, Spinner } from 'react-bootstrap'

import AuditLogFiltersBar from '@/features/system/audit/AuditLogFiltersBar'
import type { AuditLogFilterForm } from '@/features/system/audit/auditFilterTypes'
import { useAuditLogsList } from '@/features/system/audit/useAuditLogsList'
import type { AuditLogRow } from '@/apis/auditLogsApi'
import { buildAuditLogColumns } from '@/features/system/audit/columns'
import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'

const PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

type AuditLogsTableProps = {
  filterForm: AuditLogFilterForm
  queryFilterForm: AuditLogFilterForm
  onFilterChange: (next: AuditLogFilterForm) => void
  onRowClick: (row: AuditLogRow) => void
}

export function AuditLogsTable({
  filterForm,
  queryFilterForm,
  onFilterChange,
  onRowClick,
}: AuditLogsTableProps) {
  const list = useAuditLogsList({
    filterForm: queryFilterForm,
    buildColumns: buildAuditLogColumns,
  })

  return (
    <Card>
      <CardBody className="pb-0">
        <AuditLogFiltersBar value={filterForm} onChange={onFilterChange} />
      </CardBody>

      {list.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải nhật ký...
        </div>
      ) : (
        <DataTable
          table={list.table}
          emptyMessage="Chưa có nhật ký"
          onRowClick={onRowClick}
        />
      )}

      {!list.isLoading && list.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={list.paginationInfo.total}
            start={list.paginationInfo.start}
            end={list.paginationInfo.end}
            pageSize={list.pageSize}
            pageSizeOptions={PAGE_SIZE_OPTIONS}
            onPageSizeChange={list.setPageSize}
            previousPage={list.table.previousPage}
            canPreviousPage={list.table.getCanPreviousPage()}
            pageCount={list.pageCount}
            pageIndex={list.table.getState().pagination.pageIndex}
            setPageIndex={list.table.setPageIndex}
            nextPage={list.table.nextPage}
            canNextPage={list.table.getCanNextPage()}
          />
        </CardFooter>
      )}
    </Card>
  )
}
