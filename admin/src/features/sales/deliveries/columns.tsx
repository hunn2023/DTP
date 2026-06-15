import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Dropdown } from 'react-bootstrap'
import { TbDotsVertical, TbEye, TbMailForward, TbPlayerPlay, TbCheck, TbX } from 'react-icons/tb'

import {
  DELIVERY_STATUS_LABELS,
  DELIVERY_TYPE_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/sales/shared/format'
import type { DeliveryRow } from '@/apis/deliveriesApi'

export type DeliveryTableHandlers = {
  onView: (row: DeliveryRow) => void
  onProcess: (row: DeliveryRow) => void
  onMarkDelivered: (row: DeliveryRow) => void
  onMarkFailed: (row: DeliveryRow) => void
  onResendEsimEmail: (row: DeliveryRow) => void
}

const helper = createColumnHelper<DeliveryRow>()

export function buildDeliveryColumns(handlers: DeliveryTableHandlers) {
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
    {
      id: 'actions',
      header: 'Thao tác',
      enableSorting: false,
      cell: ({ row }: { row: { original: DeliveryRow } }) => (
        <Dropdown align="end" onClick={(e) => e.stopPropagation()}>
          <Dropdown.Toggle variant="light" size="sm" className="btn-icon">
            <TbDotsVertical />
          </Dropdown.Toggle>
          <Dropdown.Menu>
            <Dropdown.Item onClick={() => handlers.onView(row.original)}>
              <TbEye className="me-2" /> Chi tiết
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onProcess(row.original)}>
              <TbPlayerPlay className="me-2" /> Xử lý giao hàng
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onMarkDelivered(row.original)}>
              <TbCheck className="me-2" /> Đánh dấu đã giao
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onMarkFailed(row.original)}>
              <TbX className="me-2" /> Đánh dấu lỗi
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onResendEsimEmail(row.original)}>
              <TbMailForward className="me-2" /> Gửi lại email eSIM
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      ),
    },
  ] as ColumnDef<DeliveryRow>[]
}
