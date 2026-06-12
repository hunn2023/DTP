import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { buildPhoneCardColumns } from '@/features/products/phone-cards/columns'
import { phoneCardsLabels } from '@/features/products/phone-cards/data'
import { PHONE_CARD_PAGE_SIZE_OPTIONS } from '@/apis/phoneCardsApi'
import { usePhoneCardsCrud } from '@/features/products/phone-cards/usePhoneCardsCrud'
import ActiveFilterSelect from '@/modules/crud/components/ActiveFilterSelect'
import ListFilterSelect from '@/modules/crud/components/ListFilterSelect'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const PhoneCardsCrudTable = () => {
  const crud = usePhoneCardsCrud({
    buildColumns: buildPhoneCardColumns,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${phoneCardsLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${phoneCardsLabels.itemName} này?`

  return (
    <Card>
      <CardHeader className="border-light flex-column align-items-stretch gap-2">
        <div className="d-flex justify-content-between flex-wrap gap-2">
          <div className="d-flex align-items-center gap-2 flex-wrap">
            <div className="app-search">
              <input
                type="search"
                className="form-control"
                placeholder={phoneCardsLabels.searchPlaceholder}
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
              disabled={crud.isLoadingLookups}>
              <LuPlus className="fs-sm me-1" />
              {crud.isLoadingLookups ? 'Đang tải...' : phoneCardsLabels.addButton}
            </Button>
          </div>
        </div>
        <div className="d-flex align-items-end gap-2 flex-wrap">
          <ListFilterSelect
            label="Nhà cung cấp"
            value={crud.providerFilter}
            onChange={crud.setProviderFilter}
            options={crud.providerFilterOptions}
            allLabel="Tất cả nhà cung cấp"
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
          Đang tải thẻ viễn thông...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={phoneCardsLabels.emptyMessage}
          onRowClick={crud.openView}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={phoneCardsLabels.itemName}
            pageSize={crud.pageSize}
            pageSizeOptions={PHONE_CARD_PAGE_SIZE_OPTIONS}
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
        itemName={phoneCardsLabels.itemName}
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

export default PhoneCardsCrudTable
