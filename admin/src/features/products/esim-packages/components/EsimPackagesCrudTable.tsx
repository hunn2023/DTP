import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { buildEsimPackageColumns } from '@/features/products/esim-packages/columns'
import EsimPackageCard from '@/features/products/esim-packages/components/EsimPackageCard'
import { esimPackagesLabels } from '@/features/products/esim-packages/data'
import { ESIM_PACKAGE_PAGE_SIZE_OPTIONS } from '@/apis/esimPackagesApi'
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
    <>
      <Row className="g-3 align-items-start">
        <Col xs={12} lg={3} xl={2}>
          <Card className="esim-packages-filter-card shadow-none border h-100">
            <CardBody className="d-flex flex-column gap-3">
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

              <ListFilterSelect
                label="Quốc gia"
                value={crud.countryFilter}
                onChange={crud.setCountryFilter}
                options={crud.filterOptions.countryOptions}
                allLabel="Tất cả quốc gia"
                minWidth="0"
              />
              <ListFilterSelect
                label="Nhà mạng"
                value={crud.carrierFilter}
                onChange={crud.setCarrierFilter}
                options={crud.filterOptions.carrierOptions}
                allLabel="Tất cả nhà mạng"
                minWidth="0"
              />
              <ListFilterSelect
                label="Biến thể"
                value={crud.variantFilter}
                onChange={crud.setVariantFilter}
                options={crud.variantFilterOptions}
                allLabel="Tất cả biến thể"
                minWidth="0"
                onFocus={() => void crud.loadVariantFilterOptions()}
              />
              <div className="d-flex flex-column">
                <label className="form-label mb-1 small text-muted">Trạng thái</label>
                <ActiveFilterSelect value={crud.activeFilter} onChange={crud.setActiveFilter} />
              </div>

              <Button
                variant="primary"
                className="w-100 mt-1"
                onClick={crud.openCreate}
                disabled={!crud.filtersReady}>
                <LuPlus className="fs-sm me-1" />
                {esimPackagesLabels.addButton}
              </Button>
            </CardBody>
          </Card>
        </Col>

        <Col xs={12} lg={9} xl={10}>
          {crud.isLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" size="sm" className="me-2" />
              Đang tải gói eSIM...
            </div>
          ) : crud.items.length === 0 ? (
            <Card className="shadow-none border">
              <CardBody className="text-center text-muted py-5">{esimPackagesLabels.emptyMessage}</CardBody>
            </Card>
          ) : (
            <Row className="row-cols-1 row-cols-sm-2 row-cols-xl-3 row-cols-xxl-4 g-3">
              {crud.items.map((pkg) => (
                <Col key={pkg.id}>
                  <EsimPackageCard
                    pkg={pkg}
                    price={crud.priceByVariantId.get(pkg.productVariantId)}
                    onClick={() => crud.openEdit(pkg)}
                    onToggleActive={crud.toggleActive}
                  />
                </Col>
              ))}
            </Row>
          )}

          {!crud.isLoading && crud.paginationInfo.total > 0 && (
            <div className="mt-3">
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
            </div>
          )}
        </Col>
      </Row>

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
    </>
  )
}

export default EsimPackagesCrudTable
