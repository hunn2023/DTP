import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  enumLabel,
  formatCurrency,
  formatDateTime,
  ORDER_PAYMENT_STATUS_LABELS,
  ORDER_STATUS_LABELS,
} from '@/features/sales/shared/format'
import type { OrderRow } from '@/apis/ordersApi'

const helper = createColumnHelper<OrderRow>()

export function buildOrderColumns() {
  return [
    helper.accessor('orderCode', {
      header: 'Mã đơn',
      cell: ({ getValue }) => <code>{getValue()}</code>,
    }),
    helper.accessor('customerName', {
      header: 'Khách hàng',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.customerName || '—'}</div>
          <div className="text-muted fs-xxs">{row.original.customerEmail || row.original.customerPhone}</div>
        </div>
      ),
    }),
    helper.accessor('totalAmount', {
      header: 'Tổng tiền',
      cell: ({ row }) => formatCurrency(row.original.totalAmount, row.original.currency),
    }),
    helper.accessor('paymentStatus', {
      header: 'TT thanh toán',
      cell: ({ getValue }) => enumLabel(getValue(), ORDER_PAYMENT_STATUS_LABELS),
    }),
    helper.accessor('status', {
      header: 'Trạng thái đơn',
      cell: ({ getValue }) => enumLabel(getValue(), ORDER_STATUS_LABELS),
    }),
    helper.accessor('paymentMethod', {
      header: 'Phương thức TT',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('createdAt', {
      header: 'Ngày tạo',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
    helper.accessor('paidAt', {
      header: 'Thanh toán lúc',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
  ] as ColumnDef<OrderRow>[]
}
