import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { buildEsimPackageColumns } from '@/features/products/esim-packages/columns'
import { esimPackagesLabels } from '@/features/products/esim-packages/data'
import { ESIM_PACKAGE_PAGE_SIZE_OPTIONS } from '@/features/products/esim-packages/esim-packages.api'
import { useEsimPackagesCrud } from '@/features/products/esim-packages/useEsimPackagesCrud'
import ActiveFilterSelect from '@/modules/crud/components/ActiveFilterSelect'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'
const EsimPackagesCrudTable = () => {
  const crud = useEsimPackagesCrud({
    buildColumns: buildEsimPackageColumns,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${esimPackagesLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${esimPackagesLabels.itemName} này?`

  return (
    <Card>
      <CardHeader className="border-light flex-column align-items-stretch gap-2">
        <div className="d-flex justify-content-between flex-wrap gap-2">
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
          <ActiveFilterSelect value={crud.activeFilter} onChange={crud.setActiveFilter} />
          <Button
            variant="primary"
            size="sm"
            className="text-nowrap"
            onClick={crud.openCreate}
            disabled={!crud.filtersReady}>
            <LuPlus className="fs-sm me-1" />
            {esimPackagesLabels.addButton}
          </Button>
        </div>
        </div>
        <div className="d-flex align-items-end gap-2 flex-wrap">
          <ListFilterSelect
            label="Quốc gia"
            value={crud.countryFilter}
            onChange={crud.setCountryFilter}
            options={crud.filterOptions.countryOptions}
            allLabel="Tất cả quốc gia"
          />
          <ListFilterSelect
            label="Nhà mạng"
            value={crud.carrierFilter}
            onChange={crud.setCarrierFilter}
            options={crud.filterOptions.carrierOptions}
            allLabel="Tất cả nhà mạng"
          />
          <ListFilterSelect
            label="Biến thể"
            value={crud.variantFilter}
            onChange={crud.setVariantFilter}
            options={crud.variantFilterOptions}
            allLabel="Tất cả biến thể"
            minWidth="12rem"
            onFocus={() => void crud.loadVariantFilterOptions()}
          />
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
            pageSize={crud.pageSize}
            pageSizeOptions={ESIM_PACKAGE_PAGE_SIZE_OPTIONS}
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

    </Card>
  )
}

export default EsimPackagesCrudTable
