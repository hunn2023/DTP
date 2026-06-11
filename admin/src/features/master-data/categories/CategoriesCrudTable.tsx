import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { CATEGORY_PAGE_SIZE_OPTIONS } from '@/features/master-data/categories/categories.api'
import { buildCategoryColumns } from '@/features/master-data/categories/columns'
import { categoriesLabels } from '@/features/master-data/categories/data'
import { categoryFormConfig } from '@/features/master-data/categories/formConfig'
import { useCategoriesCrud } from '@/features/master-data/categories/useCategoriesCrud'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const CategoriesCrudTable = () => {
  const crud = useCategoriesCrud({
    buildColumns: buildCategoryColumns,
    formConfig: categoryFormConfig,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${categoriesLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${categoriesLabels.itemName} này?`

  const statusColumn = crud.table.getColumn('isActive')

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <div className="app-search">
            <input
              type="search"
              className="form-control"
              placeholder={categoriesLabels.searchPlaceholder}
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
              <option value="true">Hoạt động</option>
              <option value="false">Ngưng hoạt động</option>
            </select>
          )}
          <Button variant="primary" size="sm" className="text-nowrap" onClick={crud.openCreate}>
            <LuPlus className="fs-sm me-1" />
            {categoriesLabels.addButton}
          </Button>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải danh mục...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={categoriesLabels.emptyMessage}
          onRowClick={crud.openView}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={categoriesLabels.itemName}
            pageSize={crud.pageSize}
            pageSizeOptions={CATEGORY_PAGE_SIZE_OPTIONS}
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
        itemName={categoriesLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>

      {crud.formMode && crud.formValues && (
        <EntityFormModal
          show
          mode={crud.formMode}
          entityName={categoryFormConfig.entityName}
          fields={categoryFormConfig.fields}
          viewFields={categoryFormConfig.viewFields}
          initialValues={crud.formValues}
          onHide={crud.closeFormModal}
          onSubmit={(values) => void crud.saveForm(values)}
        />
      )}
    </Card>
  )
}

export default CategoriesCrudTable
