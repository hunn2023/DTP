import { Badge, Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuFilter, LuPlus, LuSearch, LuWifi } from 'react-icons/lu'

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
    <Card className="border-0 shadow-sm">
      <CardHeader className="border-0 bg-transparent flex-column align-items-stretch gap-3 p-4">
        <div className="d-flex justify-content-between flex-wrap gap-3">
          <div className="d-flex align-items-start gap-3">
            <span className="avatar-md rounded bg-primary-subtle text-primary d-inline-flex align-items-center justify-content-center">
              <LuWifi className="fs-24" />
            </span>
            <div>
              <div className="d-flex align-items-center gap-2 flex-wrap mb-1">
                <h5 className="mb-0 fw-semibold">Kho gói eSIM</h5>
                <Badge bg="primary-subtle" text="primary" className="border border-primary-subtle">
                  {crud.paginationInfo.total} gói
                </Badge>
              </div>
              <p className="text-muted mb-0 fs-sm">
                Tìm kiếm, lọc theo quốc gia/nhà mạng và quản lý trạng thái hiển thị.
              </p>
            </div>
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

        <div className="d-flex align-items-center gap-2 flex-wrap p-2 rounded bg-light">
          <div className="app-search flex-grow-1" style={{ minWidth: '16rem' }}>
            <input
              type="search"
              className="form-control border-0 bg-white"
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

        <div className="d-flex align-items-end gap-2 flex-wrap">
          <div className="d-flex align-items-center gap-1 text-muted fs-sm me-1 pb-2">
            <LuFilter />
            <span>Bộ lọc</span>
          </div>
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
