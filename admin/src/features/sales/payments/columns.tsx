import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import { formatCurrency, formatDateTime } from '@/features/sales/shared/format'
import type { PaymentRow } from '@/apis/paymentsApi'

const helper = createColumnHelper<PaymentRow>()

export function buildPaymentColumns() {
  return [
    helper.accessor('orderCode', {
      header: 'Mã đơn',
      cell: ({ getValue }) => <code>{getValue()}</code>,
    }),
    helper.accessor('providerTransactionId', {
      header: 'Mã giao dịch',
      cell: ({ getValue }) => <code>{getValue() || '—'}</code>,
    }),
    helper.accessor('provider', { header: 'Cổng thanh toán' }),
    helper.accessor('method', { header: 'Phương thức' }),
    helper.accessor('amount', {
      header: 'Số tiền',
      cell: ({ row }) => formatCurrency(row.original.amount, row.original.currency),
    }),
    helper.accessor('status', { header: 'Trạng thái' }),
    helper.accessor('paidAt', {
      header: 'Thanh toán lúc',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
    helper.accessor('createdAt', {
      header: 'Tạo lúc',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
  ] as ColumnDef<PaymentRow>[]
}
