import { createColumnHelper, type Row, type Table } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEdit, TbEye, TbEyeOff, TbTrash } from 'react-icons/tb'

import type { CrudCapabilities, TableRowBase } from '@/modules/crud/types'
import { defaultCrudCapabilities } from '@/modules/crud/types'

export type ActionHandlers<T extends TableRowBase> = {
  onToggleActive: (row: T) => void
  onDeleteRequest: (rowId: string) => void
  onEdit: (row: T) => void
}

export type TableColumnMeta = {
  /** Ẩn cột trên UI bảng (vẫn giữ trong column def để getRowId / logic khác). */
  hideInTable?: boolean
}

export function isHiddenTableColumn(meta: unknown): boolean {
  return Boolean((meta as TableColumnMeta | undefined)?.hideInTable)
}

export function createIdColumn<T extends TableRowBase>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.id, {
    id: 'id',
    header: 'ID',
    cell: ({ getValue }) => <span className="text-muted fw-medium">{getValue()}</span>,
    size: 72,
    meta: { hideInTable: true },
  })
}

export function createSelectColumn<T>() {
  return {
    id: 'select',
    header: ({ table }: { table: Table<T> }) => (
      <input
        type="checkbox"
        className="form-check-input form-check-input-light fs-14"
        checked={table.getIsAllRowsSelected()}
        onChange={table.getToggleAllRowsSelectedHandler()}
      />
    ),
    cell: ({ row }: { row: Row<T> }) => (
      <input
        type="checkbox"
        className="form-check-input form-check-input-light fs-14"
        checked={row.getIsSelected()}
        onChange={row.getToggleSelectedHandler()}
        onClick={(e) => e.stopPropagation()}
      />
    ),
    enableSorting: false,
    enableColumnFilter: false,
  }
}

export function createIsActiveColumn<T extends TableRowBase>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.isActive, {
    id: 'isActive',
    header: 'Hiển thị',
    filterFn: 'equals',
    cell: ({ getValue }) => {
      const active = getValue()
      return (
        <span className={`badge ${active ? 'badge-soft-success' : 'badge-soft-secondary'} fs-xxs`}>
          {active ? 'Đang hiển thị' : 'Đang ẩn'}
        </span>
      )
    },
  })
}

export function createSortOrderColumn<T extends { sortOrder: number }>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.sortOrder, {
    id: 'sortOrder',
    header: 'Thứ tự',
    cell: ({ getValue }) => <span className="text-muted">{getValue()}</span>,
  })
}

export function createActionsColumn<T extends TableRowBase>(
  handlers: ActionHandlers<T>,
  capabilities: CrudCapabilities = defaultCrudCapabilities,
) {
  const caps = { ...defaultCrudCapabilities, ...capabilities }
  const showToggle = caps.toggleActive !== false

  return {
    id: 'actions',
    header: 'Thao tác',
    enableSorting: false,
    cell: ({ row }: { row: Row<T> }) => (
      <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
        {caps.edit && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Sửa"
            onClick={() => handlers.onEdit(row.original)}>
            <TbEdit className="fs-lg" />
          </Button>
        )}
        {showToggle && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title={row.original.isActive ? 'Ẩn' : 'Hiện'}
            onClick={() => handlers.onToggleActive(row.original)}>
            {row.original.isActive ? <TbEyeOff className="fs-lg" /> : <TbEye className="fs-lg" />}
          </Button>
        )}
        {caps.delete && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Xóa"
            onClick={() => handlers.onDeleteRequest(String(row.original.id))}>
            <TbTrash className="fs-lg" />
          </Button>
        )}
      </div>
    ),
  }
}
