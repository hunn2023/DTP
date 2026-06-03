import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/modules/crud/components/tableColumns'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { Category } from '@/features/master-data/types'

const helper = createColumnHelper<Category>()

export function buildCategoryColumns(handlers: EntityTableHandlers<Category>) {
  return [
    createSelectColumn<Category>(),
    createIdColumn<Category>(),
    helper.accessor('name', {
      header: 'Tên danh mục',
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
      header: 'Mô tả',
      cell: ({ getValue }) => <span className="text-muted">{getValue()}</span>,
    }),
    createSortOrderColumn<Category>(),
    createIsActiveColumn<Category>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Category>[]
}
