import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import { formatCurrency, formatPaymentMethod } from '@/features/sales/shared/format'
import type { PaymentProvider } from '@/features/master-data/payment-providers/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type PaymentProviderTableHandlers = ActionHandlers<PaymentProvider>

const helper = createColumnHelper<PaymentProvider>()
const editOnlyCapabilities = { create: false, delete: false, edit: true, toggleActive: true }

function formatAmountLimit(amount: number | null, currency: string): string {
  if (amount == null) return '—'
  return formatCurrency(amount, currency)
}

export function buildPaymentProviderColumns(handlers: PaymentProviderTableHandlers) {
  return [
    createIdColumn<PaymentProvider>(),
    helper.accessor('name', {
      header: 'Cổng thanh toán',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          {row.original.logoUrl ? (
            <img
              src={row.original.logoUrl}
              alt=""
              width={28}
              height={28}
              className="rounded bg-light p-1"
            />
          ) : null}
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">
              <code>{row.original.code}</code>
            </div>
          </div>
        </div>
      ),
    }),
    helper.accessor('paymentMethod', {
      header: 'Phương thức',
      cell: ({ getValue }) => formatPaymentMethod(getValue()),
    }),
    helper.accessor('currency', {
      header: 'Tiền tệ',
      cell: ({ getValue }) => <span className="text-muted">{getValue()}</span>,
    }),
    helper.accessor('minAmount', {
      header: 'Tối thiểu',
      cell: ({ row, getValue }) => formatAmountLimit(getValue(), row.original.currency),
    }),
    helper.accessor('maxAmount', {
      header: 'Tối đa',
      cell: ({ row, getValue }) => formatAmountLimit(getValue(), row.original.currency),
    }),
    createSortOrderColumn<PaymentProvider>(),
    helper.accessor('isDefault', {
      header: 'Mặc định',
      cell: ({ getValue }) =>
        getValue() ? (
          <span className="badge badge-soft-success fs-xxs">Mặc định</span>
        ) : (
          <span className="text-muted">—</span>
        ),
    }),
    createIsActiveColumn<PaymentProvider>(),
    createActionsColumn(handlers, editOnlyCapabilities),
  ] as ColumnDef<PaymentProvider>[]
}
