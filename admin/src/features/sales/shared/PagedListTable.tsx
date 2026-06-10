import type { Table } from '@tanstack/react-table'
import { Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'

const PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

type PagedListTableProps<T extends { id: string | number }> = {
  searchPlaceholder: string
  table: Table<T>
  globalFilter: string
  setGlobalFilter: (value: string) => void
  paginationInfo: { start: number; end: number; total: number }
  pageCount: number
  pageSize: number
  setPageSize: (size: number) => void
  isLoading: boolean
  emptyMessage: string
  loadingLabel: string
  onRowClick?: (row: T) => void
}

function PagedListTable<T extends { id: string | number }>({
  searchPlaceholder,
  table,
  globalFilter,
  setGlobalFilter,
  paginationInfo,
  pageCount,
  pageSize,
  setPageSize,
  isLoading,
  emptyMessage,
  loadingLabel,
  onRowClick,
}: PagedListTableProps<T>) {
  return (
    <Card>
      <CardHeader className="border-light">
        <div className="app-search">
          <input
            type="search"
            className="form-control"
            placeholder={searchPlaceholder}
            value={globalFilter}
            onChange={(event) => setGlobalFilter(event.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>
      </CardHeader>

      {isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          {loadingLabel}
        </div>
      ) : (
        <DataTable table={table} emptyMessage={emptyMessage} onRowClick={onRowClick} />
      )}

      {!isLoading && paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={paginationInfo.total}
            start={paginationInfo.start}
            end={paginationInfo.end}
            pageSize={pageSize}
            pageSizeOptions={PAGE_SIZE_OPTIONS}
            onPageSizeChange={setPageSize}
            previousPage={table.previousPage}
            canPreviousPage={table.getCanPreviousPage()}
            pageCount={pageCount}
            pageIndex={table.getState().pagination.pageIndex}
            setPageIndex={table.setPageIndex}
            nextPage={table.nextPage}
            canNextPage={table.getCanNextPage()}
          />
        </CardFooter>
      )}
    </Card>
  )
}

export default PagedListTable
