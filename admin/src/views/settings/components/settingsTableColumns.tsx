import { createColumnHelper, type Row, type Table } from '@tanstack/react-table'

import type { CrudCapabilities } from '@/views/admin-crud/types'
import { defaultCrudCapabilities } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'
import { Button } from 'react-bootstrap'
import { TbEdit, TbEye, TbEyeOff, TbTrash } from 'react-icons/tb'

type ActionHandlers<T extends SettingsEntityBase> = {
  onToggleActive: (row: T) => void
  onDeleteRequest: (rowId: string) => void
  onEdit: (row: T) => void
}

export function createIdColumn<T extends SettingsEntityBase>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.id, {
    id: 'id',
    header: 'ID',
    cell: ({ getValue }) => <span className="text-muted fw-medium">{getValue()}</span>,
    size: 72,
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

export function createIsActiveColumn<T extends SettingsEntityBase>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.isActive, {
    id: 'isActive',
    header: 'Active',
    filterFn: 'equals',
    cell: ({ getValue }) => {
      const active = getValue()
      return (
        <span className={`badge ${active ? 'badge-soft-success' : 'badge-soft-secondary'} fs-xxs`}>
          {active ? 'Active' : 'Hidden'}
        </span>
      )
    },
  })
}

export function createSortOrderColumn<T extends { sortOrder: number }>() {
  const helper = createColumnHelper<T>()
  return helper.accessor((row) => row.sortOrder, {
    id: 'sortOrder',
    header: 'Sort order',
    cell: ({ getValue }) => <span className="text-muted">{getValue()}</span>,
  })
}

export function createActionsColumn<T extends SettingsEntityBase>(
  handlers: ActionHandlers<T>,
  capabilities: CrudCapabilities = defaultCrudCapabilities,
) {
  const caps = { ...defaultCrudCapabilities, ...capabilities }
  const showToggle = caps.toggleActive !== false

  return {
    id: 'actions',
    header: 'Actions',
    enableSorting: false,
    cell: ({ row }: { row: Row<T> }) => (
      <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
        {caps.edit && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Edit"
            onClick={() => handlers.onEdit(row.original)}>
            <TbEdit className="fs-lg" />
          </Button>
        )}
        {showToggle && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title={row.original.isActive ? 'Hide' : 'Show'}
            onClick={() => handlers.onToggleActive(row.original)}>
            {row.original.isActive ? <TbEyeOff className="fs-lg" /> : <TbEye className="fs-lg" />}
          </Button>
        )}
        {caps.delete && (
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Delete"
            onClick={() => handlers.onDeleteRequest(String(row.original.id))}>
            <TbTrash className="fs-lg" />
          </Button>
        )}
      </div>
    ),
  }
}
