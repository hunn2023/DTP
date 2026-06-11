import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ProductVariant } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type VariantTableHandlers = ActionHandlers<ProductVariant>

const helper = createColumnHelper<ProductVariant>()

export function buildVariantColumns(handlers: VariantTableHandlers) {
  return [
    createSelectColumn<ProductVariant>(),
    createIdColumn<ProductVariant>(),
    helper.accessor('name', { header: 'Tên', cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span> }),
    helper.accessor('sku', {
      header: 'SKU',
      cell: ({ getValue }) => {
        const sku = getValue()
        return sku ? <code>{sku}</code> : <span className="text-muted">—</span>
      },
    }),
    helper.accessor('shortName', {
      header: 'Tên ngắn',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    helper.accessor('sortOrder', { header: 'TT' }),
    createIsActiveColumn<ProductVariant>(),
    createActionsColumn(handlers),
  ] as ColumnDef<ProductVariant>[]
}
