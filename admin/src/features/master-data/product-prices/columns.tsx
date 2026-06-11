import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ProductPriceRow } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type PriceTableHandlers = ActionHandlers<ProductPriceRow>

const helper = createColumnHelper<ProductPriceRow>()

export function buildPriceColumns(handlers: PriceTableHandlers) {
  return [
    createSelectColumn<ProductPriceRow>(),
    createIdColumn<ProductPriceRow>(),
    helper.accessor('productName', {
      header: 'Sản phẩm',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.productName || row.original.productId}</div>
          {row.original.productVariantName && (
            <div className="text-muted fs-xxs">{row.original.productVariantName}</div>
          )}
        </div>
      ),
    }),
    helper.accessor('currency', { header: 'Tiền tệ' }),
    helper.accessor('originalPrice', { header: 'Giá gốc' }),
    helper.accessor('salePrice', { header: 'Giá bán' }),
    helper.accessor('costPrice', { header: 'Giá vốn' }),
    helper.accessor('startDate', {
      header: 'Từ',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    helper.accessor('endDate', {
      header: 'Đến',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    createIsActiveColumn<ProductPriceRow>(),
    createActionsColumn(handlers),
  ] as ColumnDef<ProductPriceRow>[]
}
