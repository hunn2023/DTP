import { Button, Card, CardBody, Col, Row, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import ProductCard from './ProductCard'
import './products-grid.scss'
import { buildProductColumns } from '@/features/master-data/products/columns'
import { productsLabels } from '@/features/master-data/products/data'
import { PRODUCT_PAGE_SIZE_OPTIONS } from '@/apis/productsApi'
import { useProductsCrud } from '@/features/master-data/products/useProductsCrud'
import ActiveFilterSelect from '@/modules/crud/components/ActiveFilterSelect'
import ApiFilterSearchSelect from '@/modules/crud/components/ApiFilterSearchSelect'
import {
  resolveCarrierSelectOption,
  searchCarrierSelectOptions,
} from '@/features/master-data/carriers/carrierSearchSelect'
import {
  resolveCategorySelectOption,
  searchCategorySelectOptions,
} from '@/features/master-data/categories/categorySearchSelect'
import {
  resolveCountrySelectOption,
  searchCountrySelectOptions,
} from '@/features/master-data/countries/countrySearchSelect'

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

              <ApiFilterSearchSelect
                label="Danh mục"
                value={crud.categoryFilter}
                onChange={crud.setCategoryFilter}
                allLabel="Tất cả danh mục"
                loadOptions={searchCategorySelectOptions}
                resolveValue={resolveCategorySelectOption}
                noOptionsMessage="Không tìm thấy danh mục"
              />
              <ApiFilterSearchSelect
                label="Quốc gia"
                value={crud.countryFilter}
                onChange={crud.setCountryFilter}
                allLabel="Tất cả quốc gia"
                loadOptions={searchCountrySelectOptions}
                resolveValue={resolveCountrySelectOption}
                noOptionsMessage="Không tìm thấy quốc gia"
              />
              <ApiFilterSearchSelect
                label="Nhà mạng"
                value={crud.carrierFilter}
                onChange={crud.setCarrierFilter}
                allLabel="Tất cả nhà mạng"
                loadOptions={searchCarrierSelectOptions}
                resolveValue={resolveCarrierSelectOption}
                noOptionsMessage="Không tìm thấy nhà mạng"
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
