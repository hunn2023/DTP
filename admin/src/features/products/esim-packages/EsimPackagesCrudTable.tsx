import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { buildEsimPackageColumns } from '@/features/products/esim-packages/columns'
import { esimPackagesLabels } from '@/features/products/esim-packages/data'
import { ESIM_PACKAGE_PAGE_SIZE_OPTIONS } from '@/features/products/esim-packages/esim-packages.api'
import { useEsimPackagesCrud } from '@/features/products/esim-packages/useEsimPackagesCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const EsimPackagesCrudTable = () => {
  const crud = useEsimPackagesCrud({
    buildColumns: buildEsimPackageColumns,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${esimPackagesLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${esimPackagesLabels.itemName} này?`

  const statusColumn = crud.table.getColumn('isActive')

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <div className="app-search">
            <input
              type="search"
              className="form-control"
              placeholder={esimPackagesLabels.searchPlaceholder}
              value={crud.globalFilter}
              onChange={(e) => crud.setGlobalFilter(e.target.value)}
            />
            <LuSearch className="app-search-icon text-muted" />
          </div>
          {crud.selectedCount > 0 && (
            <Button variant="danger" size="sm" onClick={crud.requestBulkDelete}>
              Xóa ({crud.selectedCount})
            </Button>
          )}
        </div>
        <div className="card-action d-flex flex-nowrap align-items-center gap-2">
          <select
            className="form-select form-select-sm w-auto"
            aria-label="Số dòng mỗi trang"
            value={crud.pageSize}
            onChange={(e) => crud.setPageSize(Number(e.target.value))}>
            {ESIM_PACKAGE_PAGE_SIZE_OPTIONS.map((size) => (
              <option key={size} value={size}>
                {size}
              </option>
            ))}
          </select>
          {statusColumn && (
            <select
              className="form-select form-select-sm"
              style={{ minWidth: '9.75rem', width: 'auto' }}
              aria-label="Lọc theo trạng thái"
              value={String(statusColumn.getFilterValue() ?? 'all')}
              onChange={(e) => {
                const value = e.target.value
                statusColumn.setFilterValue(value === 'all' ? undefined : value === 'true')
              }}>
              <option value="all">Tất cả</option>
              <option value="true">Đang hiển thị</option>
              <option value="false">Đang ẩn</option>
            </select>
          )}
          <Button
            variant="primary"
            size="sm"
            className="text-nowrap"
            onClick={crud.openCreate}
            disabled={!crud.lookupsReady}>
            <LuPlus className="fs-sm me-1" />
            {esimPackagesLabels.addButton}
          </Button>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải gói eSIM...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={esimPackagesLabels.emptyMessage}
          onRowClick={crud.openView}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={esimPackagesLabels.itemName}
            showInfo
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

      <DeleteConfirmationModal
        show={crud.showDeleteModal}
        onHide={crud.closeDeleteModal}
        onConfirm={() => void crud.confirmDelete()}
        selectedCount={crud.pendingDeleteCount}
        itemName={esimPackagesLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>

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

export default EsimPackagesCrudTable
