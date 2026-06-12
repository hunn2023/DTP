import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  DELIVERY_STATUS_LABELS,
  DELIVERY_TYPE_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/sales/shared/format'
import type { DeliveryRow } from '@/apis/deliveriesApi'

const helper = createColumnHelper<DeliveryRow>()

export function buildDeliveryColumns() {
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
          <div className="text-muted fs-xxs">{row.original.customerEmail}</div>
        </div>
      ),
    }),
    helper.accessor('deliveryType', {
      header: 'Loại giao hàng',
      cell: ({ getValue }) => enumLabel(getValue(), DELIVERY_TYPE_LABELS),
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ getValue }) => enumLabel(getValue(), DELIVERY_STATUS_LABELS),
    }),
    helper.accessor('attemptCount', { header: 'Lần thử' }),
    helper.accessor('sentAt', {
      header: 'Giao lúc',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
    helper.accessor('lastError', {
      header: 'Lỗi',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('createdAt', {
      header: 'Tạo lúc',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
  ] as ColumnDef<DeliveryRow>[]
}
