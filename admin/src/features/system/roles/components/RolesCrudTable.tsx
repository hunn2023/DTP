import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'
import { buildRoleColumns } from '@/features/system/roles/columns'
import { useRolesCrud } from '@/features/system/roles/useRolesCrud'
import CheckboxPickerModal from '@/features/system/shared/CheckboxPickerModal'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

const RolesCrudTable = () => {
  const crud = useRolesCrud({ buildColumns: buildRoleColumns })
  const statusColumn = crud.table.getColumn('isActive')

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="app-search">
          <input
            type="search"
            className="form-control"
            placeholder="Tìm tên, mã role..."
            value={crud.globalFilter}
            onChange={(e) => crud.setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
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
            Thêm vai trò
          </Button>
        </div>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải vai trò...
        </div>
      ) : (
        <DataTable table={crud.table} emptyMessage="Chưa có vai trò" onRowClick={crud.openView} />
      )}

      {!crud.isLoading && crud.paginationInfo.total > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            pageSize={crud.pageSize}
            pageSizeOptions={PAGE_SIZE_OPTIONS}
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

      {crud.assignRole && (
        <CheckboxPickerModal
          show
          title={`Gán quyền — ${crud.assignRole.name}`}
          groupedOptions={crud.assignGroupedOptions}
          selectedIds={crud.assignSelectedIds}
          isLoading={crud.assignLoading}
          isSaving={crud.assignSaving}
          onHide={crud.closeAssignModal}
          onSave={(ids) => void crud.saveAssignPermissions(ids)}
        />
      )}
    </Card>
  )
}

export default RolesCrudTable
