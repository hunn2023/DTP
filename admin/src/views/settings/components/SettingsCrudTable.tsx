import type { ColumnDef } from '@tanstack/react-table'
import { Button, Card, CardFooter, CardHeader } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'
import SettingsEntityFormModal from '@/views/settings/form/SettingsEntityFormModal'
import type { SettingsFormConfig } from '@/views/settings/form/types'
import {
  useSettingsCrudTable,
  type SettingsTableHandlers,
} from '@/views/settings/hooks/useSettingsCrudTable'
import type { CrudCapabilities } from '@/views/admin-crud/types'
import { defaultCrudCapabilities } from '@/views/admin-crud/types'
import type { SettingsCrudLabels, SettingsEntityBase } from '@/views/settings/types'

type SettingsCrudTableProps<T extends SettingsEntityBase> = {
  initialData: T[]
  buildColumns: (handlers: SettingsTableHandlers<T>) => ColumnDef<T>[]
  formConfig: SettingsFormConfig<T>
  labels: SettingsCrudLabels
  pageSize?: number
  capabilities?: CrudCapabilities
}

type ToolbarProps<T extends SettingsEntityBase> = {
  labels: SettingsCrudLabels
  globalFilter: string
  setGlobalFilter: (value: string) => void
  selectedCount: number
  onBulkDelete: () => void
  onAdd: () => void
  table: ReturnType<typeof useSettingsCrudTable<T>>['table']
}

function SettingsTableToolbar<T extends SettingsEntityBase>({
  labels,
  globalFilter,
  setGlobalFilter,
  selectedCount,
  onBulkDelete,
  onAdd,
  table,
  showAdd,
}: ToolbarProps<T> & { showAdd: boolean }) {
  const statusColumn = table.getColumn('isActive')

  return (
    <CardHeader className="border-light justify-content-between">
      <div className="d-flex align-items-center gap-2 flex-wrap">
        <div className="app-search">
          <input
            type="search"
            className="form-control"
            placeholder={labels.searchPlaceholder}
            value={globalFilter}
            onChange={(e) => setGlobalFilter(e.target.value)}
          />
          <LuSearch className="app-search-icon text-muted" />
        </div>
        {selectedCount > 0 && (
          <Button variant="danger" size="sm" onClick={onBulkDelete}>
            Xóa ({selectedCount})
          </Button>
        )}
      </div>
      <div className="card-action d-flex flex-nowrap align-items-center gap-2">
        <select
          className="form-select form-select-sm w-auto"
          aria-label="Số dòng mỗi trang"
          value={table.getState().pagination.pageSize}
          onChange={(e) => table.setPageSize(Number(e.target.value))}>
          {[5, 10, 15, 20].map((size) => (
            <option key={size} value={size}>
              {size}
            </option>
          ))}
        </select>
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
            <option value="true">Đang hiển thị</option>
            <option value="false">Đang ẩn</option>
          </select>
        )}
        {showAdd && (
          <Button variant="primary" size="sm" className="text-nowrap" onClick={onAdd}>
            <LuPlus className="fs-sm me-1" />
            {labels.addButton}
          </Button>
        )}
      </div>
    </CardHeader>
  )
}

const SettingsCrudTable = <T extends SettingsEntityBase>({
  initialData,
  buildColumns,
  formConfig,
  labels,
  pageSize,
  capabilities: capabilitiesProp,
}: SettingsCrudTableProps<T>) => {
  const capabilities = { ...defaultCrudCapabilities, ...capabilitiesProp }
  const crud = useSettingsCrudTable({ initialData, buildColumns, formConfig, pageSize })

  const deleteMessage =
    labels.deleteConfirm ??
    (crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${labels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${labels.itemName} này?`)

  return (
    <Card>
      <SettingsTableToolbar
        labels={labels}
        globalFilter={crud.globalFilter}
        setGlobalFilter={crud.setGlobalFilter}
        selectedCount={crud.selectedCount}
        onBulkDelete={crud.requestBulkDelete}
        onAdd={crud.openCreate}
        table={crud.table}
        showAdd={capabilities.create !== false}
      />
      <DataTable
        table={crud.table}
        emptyMessage={labels.emptyMessage}
        onRowClick={capabilities.view !== false ? crud.openView : undefined}
      />
      {crud.table.getRowModel().rows.length > 0 && (
        <CardFooter className="border-0">
          <TablePagination
            totalItems={crud.paginationInfo.total}
            start={crud.paginationInfo.start}
            end={crud.paginationInfo.end}
            itemsName={labels.itemName}
            showInfo
            previousPage={crud.table.previousPage}
            canPreviousPage={crud.table.getCanPreviousPage()}
            pageCount={crud.table.getPageCount()}
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
        onConfirm={crud.confirmDelete}
        selectedCount={crud.pendingDeleteCount}
        itemName={labels.itemName}
        modalTitle={labels.deleteTitle ?? 'Xác nhận xóa'}
        confirmButtonText="Xóa"
        cancelButtonText="Hủy">
        {deleteMessage}
      </DeleteConfirmationModal>
      {crud.formMode && crud.formValues && (
        <SettingsEntityFormModal
          show
          mode={crud.formMode}
          entityName={formConfig.entityName}
          fields={formConfig.fields}
          initialValues={crud.formValues}
          onHide={crud.closeFormModal}
          onSubmit={crud.saveForm}
        />
      )}
    </Card>
  )
}

export default SettingsCrudTable
