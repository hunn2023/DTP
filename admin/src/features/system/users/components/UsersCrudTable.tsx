import { Button, Card, CardFooter, CardHeader, Spinner } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import TablePagination from '@/components/table/TablePagination'
import { buildUserColumns } from '@/features/system/users/columns'
import UserCreateModal from '@/features/system/users/components/UserCreateModal'
import { useUsersCrud } from '@/features/system/users/useUsersCrud'
import CheckboxPickerModal from '@/features/system/shared/CheckboxPickerModal'
import EntityFormModal from '@/modules/crud/form/EntityFormModal'

const PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

const UsersCrudTable = () => {
  const crud = useUsersCrud({ buildColumns: buildUserColumns })

  return (
    <Card>
      <CardHeader className="border-light justify-content-between">
        <div className="app-search">
          <input
            type="search"
            className="form-control"
            placeholder="Tìm email, tên, SĐT..."
            value={crud.globalFilter}
            onChange={(e) => crud.setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>
        <Button variant="primary" size="sm" className="text-nowrap" onClick={() => void crud.openCreate()}>
          <LuPlus className="fs-sm me-1" />
          Thêm tài khoản
        </Button>
      </CardHeader>

      {crud.isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải tài khoản...
        </div>
      ) : (
        <DataTable table={crud.table} emptyMessage="Chưa có tài khoản" onRowClick={crud.openView} />
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

      {crud.formMode === 'create' && crud.formValues && (
        <UserCreateModal
          show
          initialValues={crud.formValues}
          roleOptions={crud.createRoleOptions}
          rolesLoading={crud.createRolesLoading}
          isSaving={crud.isSaving}
          onHide={crud.closeFormModal}
          onSubmit={(values) => void crud.saveForm(values)}
        />
      )}

      {crud.formMode && crud.formMode !== 'create' && crud.formValues && (
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

      {crud.assignUser && (
        <CheckboxPickerModal
          show
          title={`Gán vai trò — ${crud.assignUser.fullName || crud.assignUser.email}`}
          options={crud.assignOptions}
          selectedIds={crud.assignSelectedIds}
          isLoading={crud.assignLoading}
          isSaving={crud.assignSaving}
          onHide={crud.closeAssignModal}
          onSave={(ids) => void crud.saveAssignRoles(ids)}
        />
      )}
    </Card>
  )
}

export default UsersCrudTable
