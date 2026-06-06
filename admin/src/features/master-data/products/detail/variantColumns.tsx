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
    helper.accessor('price', { header: 'Giá' }),
    helper.accessor('dataAmount', {
      header: 'Data',
      cell: ({ row }) => {
        if (row.original.isUnlimited) return 'Unlimited'
        const amount = row.original.dataAmount
        if (amount == null) return <span className="text-muted">—</span>
        return `${amount} ${row.original.dataUnit || ''}`.trim()
      },
    }),
    helper.accessor('durationDays', {
      header: 'Ngày',
      cell: ({ getValue }) => getValue() ?? <span className="text-muted">—</span>,
    }),
    createIsActiveColumn<ProductVariant>(),
    createActionsColumn(handlers),
  ] as ColumnDef<ProductVariant>[]
}
