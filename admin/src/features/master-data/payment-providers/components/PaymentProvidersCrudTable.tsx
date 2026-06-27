import { Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'
import { PAYMENT_PROVIDER_PAGE_SIZE_OPTIONS } from '@/apis/paymentProvidersApi'
import { buildPaymentProviderColumns } from '@/features/master-data/payment-providers/columns'
import { paymentProvidersLabels } from '@/features/master-data/payment-providers/data'
import { usePaymentProvidersCrud } from '@/features/master-data/payment-providers/usePaymentProvidersCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const PaymentProvidersCrudTable = () => {
  const crud = usePaymentProvidersCrud({
    buildColumns: buildPaymentProviderColumns,
  })

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <div className="app-search">
            <input
              type="search"
              className="form-control"
              placeholder={paymentProvidersLabels.searchPlaceholder}
              value={crud.globalFilter}
              onChange={(e) => crud.setGlobalFilter(e.target.value)}
            />
            <LuSearch className="app-search-icon text-muted" />
          </div>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải cổng thanh toán...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={paymentProvidersLabels.emptyMessage}
          onRowClick={crud.openEdit}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={paymentProvidersLabels.itemName}
            pageSize={crud.pageSize}
            pageSizeOptions={PAYMENT_PROVIDER_PAGE_SIZE_OPTIONS}
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

      {crud.formMode && crud.formValues && (
        <EntityFormModal
          show
          mode={crud.formMode}
          entityName={crud.formConfig.entityName}
          fields={crud.formConfig.fields}
          viewFields={crud.formConfig.viewFields}
          initialValues={crud.formValues}
          onHide={crud.closeFormModal}
          onSubmit={(values) => void crud.saveForm(values)}
        />
      )}
    </Card>
  )
}

export default PaymentProvidersCrudTable
