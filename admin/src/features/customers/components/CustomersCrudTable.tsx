import { Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuSearch } from 'react-icons/lu'

import { CUSTOMER_PAGE_SIZE_OPTIONS } from '@/apis/customersApi'
import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'
import { buildCustomerColumns } from '@/features/customers/columns'
import CustomerDetailModal from '@/features/customers/components/CustomerDetailModal'
import { useCustomersCrud } from '@/features/customers/useCustomersCrud'

type CustomersCrudTableProps = {
  isActiveFilter?: boolean
  searchPlaceholder?: string
}

const CustomersCrudTable = ({
  isActiveFilter,
  searchPlaceholder = 'Tìm tên, email, SĐT...',
}: CustomersCrudTableProps) => {
  const crud = useCustomersCrud({
    buildColumns: buildCustomerColumns,
    isActiveFilter,
  })

  return (
    <Card>
      <CardHeader className="border-light">
        <div className="app-search">
          <input
            type="search"
            className="form-control"
            placeholder={searchPlaceholder}
            value={crud.globalFilter}
            onChange={(e) => crud.setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải khách hàng...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage="Chưa có khách hàng"
          onRowClick={(row) => void crud.openView(row)}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            pageSize={crud.pageSize}
            pageSizeOptions={CUSTOMER_PAGE_SIZE_OPTIONS}
            onPageSizeChange={crud.setPageSize}
            previousPage={crud.table.previousPage}
            canPreviousPage={crud.table.getCanPreviousPage()}
            pageCount={crud.pageCount}
            pageIndex={crud.table.getState().pagination.pageIndex}
            setPageIndex={crud.table.setPageIndex}
            nextPage={crud.table.nextPage}
            canNextPage={crud.table.getCanNextPage()}
          />
        </CardFooter>
      )}

      <CustomerDetailModal
        show={Boolean(crud.detail) || crud.detailLoading}
        customer={crud.detail}
        isLoading={crud.detailLoading}
        isUpdating={crud.statusUpdating}
        onHide={crud.closeDetail}
        onToggleLock={() => {
          if (crud.detail) void crud.toggleLock(crud.detail)
        }}
      />
    </Card>
  )
}

export default CustomersCrudTable
