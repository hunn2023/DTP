import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/modules/crud/components/tableColumns'
import type { ActionHandlers } from '@/modules/crud/components/tableColumns'
import type { Category } from '@/features/master-data/types'

export type CategoryTableHandlers = ActionHandlers<Category>

const helper = createColumnHelper<Category>()

export function buildCategoryColumns(handlers: CategoryTableHandlers) {
  return [
    createSelectColumn<Category>(),
    createIdColumn<Category>(),
    helper.accessor('name', {
      header: 'Tên danh mục',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            /{row.original.slug}
            {row.original.code ? ` · ${row.original.code}` : ''}
          </div>
        </div>
      ),
    }),
    helper.accessor('description', {
      header: 'Mô tả',
      cell: ({ getValue }) => <span className="text-muted">{getValue() || '—'}</span>,
    }),
    createSortOrderColumn<Category>(),
    createIsActiveColumn<Category>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Category>[]
}
