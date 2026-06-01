import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { Category } from '@/views/settings/types'

const helper = createColumnHelper<Category>()

export function buildCategoryColumns(handlers: SettingsTableHandlers<Category>) {
  return [
    createSelectColumn<Category>(),
    createIdColumn<Category>(),
    helper.accessor('name', {
      header: 'Category name',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <span className="fs-4 lh-1">{row.original.icon}</span>
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">/{row.original.slug}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('description', {
      header: 'Description',
      cell: ({ getValue }) => <span className="text-muted">{getValue()}</span>,
    }),
    createSortOrderColumn<Category>(),
    createIsActiveColumn<Category>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Category>[]
}
