import { Button } from 'react-bootstrap'
import { TbCheck, TbCreditCard, TbX } from 'react-icons/tb'

import type { OrderDetail, OrderRow } from '@/apis/ordersApi'

export type OrderDetailActionHandlers = {
  onMarkPaid: (order: OrderRow) => void
  onComplete: (order: OrderRow) => void
  onCancel: (order: OrderRow) => void
}

function canMarkPaid(order: OrderDetail): boolean {
  return order.status !== 6 && order.paymentStatus !== 3 && order.paymentStatus !== 5
}

function canComplete(order: OrderDetail): boolean {
  return order.status !== 5 && order.status !== 6
}

function canCancel(order: OrderDetail): boolean {
  return order.status !== 5 && order.status !== 6
}

type OrderDetailActionsProps = {
  order: OrderDetail
  handlers: OrderDetailActionHandlers
  isSaving?: boolean
}

const OrderDetailActions = ({ order, handlers, isSaving = false }: OrderDetailActionsProps) => (
  <div className="d-flex flex-wrap gap-2">
    <Button
      variant="outline-primary"
      size="sm"
      disabled={isSaving || !canMarkPaid(order)}
      onClick={() => handlers.onMarkPaid(order)}>
      <TbCreditCard className="me-1" />
      Đánh dấu đã thanh toán
    </Button>
    <Button
      variant="outline-success"
      size="sm"
      disabled={isSaving || !canComplete(order)}
      onClick={() => handlers.onComplete(order)}>
      <TbCheck className="me-1" />
      Hoàn thành đơn
    </Button>
    <Button
      variant="outline-danger"
      size="sm"
      disabled={isSaving || !canCancel(order)}
      onClick={() => handlers.onCancel(order)}>
      <TbX className="me-1" />
      Hủy đơn
    </Button>
  </div>
)

export default OrderDetailActions
