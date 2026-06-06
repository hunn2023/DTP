import type { ColumnDef } from '@tanstack/react-table'
import { Button, Card, CardFooter, CardHeader } from 'react-bootstrap'
import { LuPlus, LuSearch } from 'react-icons/lu'

import DataTable from '@/components/table/DataTable'
import DeleteConfirmationModal from '@/components/table/DeleteConfirmationModal'
import TablePagination from '@/components/table/TablePagination'

const DEFAULT_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const
import EntityFormModal from '@/modules/crud/form/EntityFormModal'
import type { EntityFormConfig } from '@/modules/crud/form/types'
import { useEntityCrud, type EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { CrudCapabilities, EntityCrudLabels, CrudEntityBase } from '@/modules/crud/types'
import { defaultCrudCapabilities } from '@/modules/crud/types'

type EntityCrudTableProps<T extends CrudEntityBase> = {
  initialData: T[]
  buildColumns: (handlers: EntityTableHandlers<T>) => ColumnDef<T>[]
  formConfig: EntityFormConfig<T>
  labels: EntityCrudLabels
  pageSize?: number
  capabilities?: CrudCapabilities
}

type ToolbarProps<T extends CrudEntityBase> = {
  labels: EntityCrudLabels
  globalFilter: string
  setGlobalFilter: (value: string) => void
  selectedCount: number
  onBulkDelete: () => void
  onAdd: () => void
  table: ReturnType<typeof useEntityCrud<T>>['table']
}

function EntityTableToolbar<T extends CrudEntityBase>({
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

const EntityCrudTable = <T extends CrudEntityBase>({
  initialData,
  buildColumns,
  formConfig,
  labels,
  pageSize,
  capabilities: capabilitiesProp,
}: EntityCrudTableProps<T>) => {
  const capabilities = { ...defaultCrudCapabilities, ...capabilitiesProp }
  const crud = useEntityCrud({ initialData, buildColumns, formConfig, pageSize })

  const deleteMessage =
    labels.deleteConfirm ??
    (crud.pendingDeleteCount > 1
      ? `Bạn có chắc muốn xóa ${crud.pendingDeleteCount} ${labels.itemName} đã chọn?`
      : `Bạn có chắc muốn xóa ${labels.itemName} này?`)

  return (
    <Card>
      <EntityTableToolbar
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
            pageSize={crud.table.getState().pagination.pageSize}
            pageSizeOptions={DEFAULT_PAGE_SIZE_OPTIONS}
            onPageSizeChange={crud.table.setPageSize}
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
        <EntityFormModal
          show
          mode={crud.formMode}
          entityName={formConfig.entityName}
          fields={formConfig.fields}
          viewFields={formConfig.viewFields}
          initialValues={crud.formValues}
          onHide={crud.closeFormModal}
          onSubmit={crud.saveForm}
        />
      )}
    </Card>
  )
}

export default EntityCrudTable
