import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import ProductCard from '@/features/master-data/products/components/ProductCard'
import '@/features/master-data/products/components/products-grid.scss'
import { buildProductColumns } from '@/features/master-data/products/columns'
import { productsLabels } from '@/features/master-data/products/data'
import { PRODUCT_PAGE_SIZE_OPTIONS } from '@/features/master-data/products/products.api'
import { useProductsCrud } from '@/features/master-data/products/useProductsCrud'
import ActiveFilterSelect from '@/modules/crud/components/ActiveFilterSelect'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'

const ProductsCrudTable = () => {
  const crud = useProductsCrud({ buildColumns: buildProductColumns })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${productsLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${productsLabels.itemName} này?`

  return (
    <>
      <Row className="g-3 align-items-start">
        <Col xs={12} lg={3} xl={2}>
          <Card className="products-filter-card shadow-none border h-100">
            <CardBody className="d-flex flex-column gap-3">
              <div className="app-search">
                <input
                  type="search"
                  className="form-control"
                  placeholder={productsLabels.searchPlaceholder}
                  value={crud.globalFilter}
                  onChange={(e) => crud.setGlobalFilter(e.target.value)}
                />
                <LuSearch className="app-search-icon text-muted" />
              </div>

              <ListFilterSelect
                label="Danh mục"
                value={crud.categoryFilter}
                onChange={crud.setCategoryFilter}
                options={crud.categoryFilterOptions}
                allLabel="Tất cả danh mục"
                minWidth="0"
              />
              <ListFilterSelect
                label="Quốc gia"
                value={crud.countryFilter}
                onChange={crud.setCountryFilter}
                options={crud.countryFilterOptions}
                allLabel="Tất cả quốc gia"
                minWidth="0"
              />
              <ListFilterSelect
                label="Nhà mạng"
                value={crud.carrierFilter}
                onChange={crud.setCarrierFilter}
                options={crud.carrierFilterOptions}
                allLabel="Tất cả nhà mạng"
                minWidth="0"
              />
              <div className="d-flex flex-column">
                <label className="form-label mb-1 small text-muted">Trạng thái</label>
                <ActiveFilterSelect value={crud.activeFilter} onChange={crud.setActiveFilter} />
              </div>

              <Button variant="primary" className="w-100 mt-1" onClick={crud.openCreate}>
                <LuPlus className="fs-sm me-1" />
                {productsLabels.addButton}
              </Button>
            </CardBody>
          </Card>
        </Col>

        <Col xs={12} lg={9} xl={10}>
          {crud.isLoading ? (
            <div className="text-center py-5">
              <Spinner animation="border" size="sm" className="me-2" />
              Đang tải sản phẩm...
            </div>
          ) : crud.items.length === 0 ? (
            <Card className="shadow-none border">
              <CardBody className="text-center text-muted py-5">{productsLabels.emptyMessage}</CardBody>
            </Card>
          ) : (
            <Row className="row-cols-1 row-cols-sm-2 row-cols-xl-3 row-cols-xxl-4 g-3">
              {crud.items.map((product) => (
                <Col key={product.id}>
                  <ProductCard
                    product={product}
                    countryFlagUrl={crud.countryFlagById.get(product.countryId)}
                    onClick={() => crud.openView(product)}
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
                itemsName={productsLabels.itemName}
                pageSize={crud.pageSize}
                pageSizeOptions={PRODUCT_PAGE_SIZE_OPTIONS}
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
        itemName={productsLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>
    </>
  )
}

export default ProductsCrudTable
