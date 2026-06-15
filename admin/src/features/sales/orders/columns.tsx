import { type ColumnDef } from '@tanstack/react-table'
import { Dropdown } from 'react-bootstrap'
import { TbCheck, TbDotsVertical, TbEye, TbCreditCard, TbX } from 'react-icons/tb'

import type { OrderRow } from '@/apis/ordersApi'
import { buildOrderListColumns } from '@/features/sales/shared/orderListColumns'

export type OrderTableHandlers = {
  onView: (row: OrderRow) => void
  onMarkPaid: (row: OrderRow) => void
  onComplete: (row: OrderRow) => void
  onCancel: (row: OrderRow) => void
}

export function buildOrderColumns(handlers: OrderTableHandlers): ColumnDef<OrderRow>[] {
  return [
    ...buildOrderListColumns(),
    {
      id: 'actions',
      header: 'Thao tác',
      enableSorting: false,
      cell: ({ row }: { row: { original: OrderRow } }) => (
        <Dropdown align="end" onClick={(e) => e.stopPropagation()}>
          <Dropdown.Toggle variant="light" size="sm" className="btn-icon">
            <TbDotsVertical />
          </Dropdown.Toggle>
          <Dropdown.Menu>
            <Dropdown.Item onClick={() => handlers.onView(row.original)}>
              <TbEye className="me-2" /> Chi tiết
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onMarkPaid(row.original)}>
              <TbCreditCard className="me-2" /> Đánh dấu đã thanh toán
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onComplete(row.original)}>
              <TbCheck className="me-2" /> Hoàn thành đơn
            </Dropdown.Item>
            <Dropdown.Item onClick={() => handlers.onCancel(row.original)}>
              <TbX className="me-2" /> Hủy đơn
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      ),
    },
  ]
}
