import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import { COUNTRY_PAGE_SIZE_OPTIONS } from '@/apis/countriesApi'
import { buildCountryColumns } from '@/features/master-data/countries/columns'
import { countriesLabels } from '@/features/master-data/countries/data'
import { countryFormConfig } from '@/features/master-data/countries/formConfig'
import CountryFormModal from './CountryFormModal'
import { useCountriesCrud } from '@/features/master-data/countries/useCountriesCrud'
import '../countries.scss'

const CountriesCrudTable = () => {
  const crud = useCountriesCrud({
    buildColumns: buildCountryColumns,
    formConfig: countryFormConfig,
  })

  const deleteMessage =
    crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${countriesLabels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${countriesLabels.itemName} này?`

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <div className="app-search">
            <input
              type="search"
              className="form-control"
              placeholder={countriesLabels.searchPlaceholder}
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
          <Button variant="primary" size="sm" className="text-nowrap" onClick={crud.openCreate}>
            <LuPlus className="fs-sm me-1" />
            {countriesLabels.addButton}
          </Button>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải quốc gia...
        </div>
      ) : (
        <DataTable
          table={crud.table}
          emptyMessage={countriesLabels.emptyMessage}
          onRowClick={crud.openEdit}
        />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={countriesLabels.itemName}
            pageSize={crud.pageSize}
            pageSizeOptions={COUNTRY_PAGE_SIZE_OPTIONS}
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
        itemName={countriesLabels.itemName}
        modalTitle="Xác nhận xóa"
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>

      {crud.formMode && crud.formValues && (
        <CountryFormModal
          show
          mode={crud.formMode}
          initialValues={crud.formValues}
          activeTab={crud.formTab}
          countryIdForFlag={crud.countryIdForFlag}
          isSaving={crud.isSaving}
          onHide={crud.closeFormModal}
          onTabChange={crud.setFormTab}
          onContinueCreate={(values) => void crud.continueCreate(values)}
          onSaveChanges={(input) => void crud.saveCountryChanges(input)}
          onSaveCreateFlag={(file) => void crud.saveCreateFlag(file)}
        />
      )}
    </Card>
  )
}

export default CountriesCrudTable
