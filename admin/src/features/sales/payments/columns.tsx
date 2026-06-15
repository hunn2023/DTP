import { type ColumnDef } from '@tanstack/react-table'
import { Dropdown } from 'react-bootstrap'
import { TbDotsVertical, TbEye } from 'react-icons/tb'

import type { OrderRow } from '@/apis/ordersApi'
import { buildOrderListColumns } from '@/features/sales/shared/orderListColumns'

export type PaymentTableHandlers = {
  onView: (row: OrderRow) => void
}

export function buildPaymentOrderColumns(handlers: PaymentTableHandlers): ColumnDef<OrderRow>[] {
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
              <TbEye className="me-2" /> Chi tiết thanh toán
            </Dropdown.Item>
          </Dropdown.Menu>
        </Dropdown>
      ),
    },
  ]
}
