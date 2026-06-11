import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'
import type { PhoneCard } from '@/features/products/phone-cards/types'

export type PhoneCardTableHandlers = ActionHandlers<PhoneCard>

const helper = createColumnHelper<PhoneCard>()

function formatMoney(value: number, currency: string): string {
  return `${value.toLocaleString('vi-VN')} ${currency}`
}

export function buildPhoneCardColumns(handlers: PhoneCardTableHandlers) {
  return [
    createSelectColumn<PhoneCard>(),
    createIdColumn<PhoneCard>(),
    helper.accessor('name', {
      header: 'Thẻ viễn thông',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            <code>{row.original.slug}</code>
          </div>
        </div>
      ),
    }),
    helper.accessor('providerName', {
      header: 'Nhà cung cấp',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    helper.accessor('productVariantName', {
      header: 'Biến thể SP',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    helper.accessor('currency', { header: 'Tiền tệ' }),
    helper.display({
      id: 'faceValue',
      header: 'Mệnh giá',
      cell: ({ row }) => formatMoney(row.original.faceValue, row.original.currency),
    }),
    helper.display({
      id: 'price',
      header: 'Giá bán',
      cell: ({ row }) => <span className="fw-medium">{formatMoney(row.original.price, row.original.currency)}</span>,
    }),
    createSortOrderColumn<PhoneCard>(),
    createIsActiveColumn<PhoneCard>(),
    createActionsColumn(handlers),
  ] as ColumnDef<PhoneCard>[]
}
