import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { OrderRow } from '@/apis/ordersApi'
import {
  formatCurrency,
  formatDateTime,
  formatPaymentMethod,
} from '@/features/sales/shared/format'
import {
  OrderPaymentStatusBadge,
  OrderStatusBadge,
} from '@/features/sales/shared/OrderStatusBadges'

const helper = createColumnHelper<OrderRow>()

export function buildOrderListColumns(): ColumnDef<OrderRow>[] {
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
          <div className="text-muted fs-xxs">
            {row.original.customerEmail || row.original.customerPhone}
          </div>
        </div>
      ),
    }),
    helper.accessor('totalAmount', {
      header: 'Tổng tiền',
      cell: ({ row }) => formatCurrency(row.original.totalAmount, row.original.currency),
    }),
    helper.accessor('paymentStatus', {
      header: 'TT thanh toán',
      cell: ({ getValue }) => <OrderPaymentStatusBadge paymentStatus={getValue()} />,
    }),
    helper.accessor('status', {
      header: 'Trạng thái đơn',
      cell: ({ getValue }) => <OrderStatusBadge status={getValue()} />,
    }),
    helper.accessor('paymentMethod', {
      header: 'Phương thức TT',
      cell: ({ getValue }) => formatPaymentMethod(getValue()) || '—',
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
