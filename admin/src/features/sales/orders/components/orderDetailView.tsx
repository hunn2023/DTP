import { Card, CardBody, CardHeader, Col, Row, Table } from 'react-bootstrap'

import type { OrderDetail, OrderHistory, OrderItem } from '@/apis/ordersApi'
import {
  enumLabel,
  formatCurrency,
  formatDateTime,
  formatPaymentMethod,
  resolveCurrencyUnit,
  ORDER_ITEM_TYPE_LABELS,
  ORDER_STATUS_LABELS,
} from '@/features/sales/shared/format'
import { OrderDetailStatusBadges } from '@/features/sales/shared/OrderStatusBadges'

function InfoField({ label, value }: { label: string; value: string }) {
  return (
    <div className="mb-2">
      <div className="text-muted fs-xxs text-uppercase mb-1">{label}</div>
      <div className="fw-medium">{value || '—'}</div>
    </div>
  )
}

function OrderDetailHeader({ order }: { order: OrderDetail }) {
  return (
    <div className="d-flex flex-wrap align-items-start justify-content-between gap-2 mb-3">
      <div>
        <div className="text-muted fs-sm mb-1">Mã đơn hàng</div>
        <code className="fs-md">{order.orderCode}</code>
      </div>
      <OrderDetailStatusBadges status={order.status} paymentStatus={order.paymentStatus} />
    </div>
  )
}

function OrderCustomerCard({ order }: { order: OrderDetail }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Khách hàng</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Họ tên" value={order.customerName} />
        <InfoField label="Email" value={order.customerEmail} />
        <InfoField label="Số điện thoại" value={order.customerPhone} />
        <InfoField label="Customer ID" value={order.customerId} />
      </CardBody>
    </Card>
  )
}

function OrderPaymentCard({ order }: { order: OrderDetail }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Thanh toán</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Phương thức" value={formatPaymentMethod(order.paymentMethod)} />
        <InfoField label="Mã giao dịch" value={order.paymentTransactionId} />
        <InfoField label="Hết hạn thanh toán" value={formatDateTime(order.paymentExpiredAt)} />
        <InfoField label="Thanh toán lúc" value={formatDateTime(order.paidAt)} />
      </CardBody>
    </Card>
  )
}

function OrderTimelineCard({ order }: { order: OrderDetail }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Thời gian & ghi chú</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Ngày tạo" value={formatDateTime(order.createdAt)} />
        <InfoField label="Cập nhật lúc" value={formatDateTime(order.updatedAt)} />
        <InfoField label="Hủy lúc" value={formatDateTime(order.cancelledAt)} />
        <InfoField label="Ghi chú đơn" value={order.note} />
        <InfoField label="Lý do hủy" value={order.cancelReason} />
      </CardBody>
    </Card>
  )
}

function OrderSummaryCard({ order }: { order: OrderDetail }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Tổng tiền</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Tiền tệ" value={resolveCurrencyUnit(order.currency)} />
        <div className="d-flex justify-content-between mb-2">
          <span className="text-muted">Tạm tính</span>
          <span>{formatCurrency(order.subTotal, order.currency)}</span>
        </div>
        <div className="d-flex justify-content-between mb-2">
          <span className="text-muted">Giảm giá</span>
          <span className="text-danger">
            {order.discountAmount > 0
              ? `−${formatCurrency(order.discountAmount, order.currency)}`
              : formatCurrency(0, order.currency)}
          </span>
        </div>
        <div className="d-flex justify-content-between pt-2 border-top fw-semibold fs-md">
          <span>Tổng cộng</span>
          <span className="text-primary">{formatCurrency(order.totalAmount, order.currency)}</span>
        </div>
      </CardBody>
    </Card>
  )
}

function OrderItemsTable({ items, currency }: { items: OrderItem[]; currency: string }) {
  if (items.length === 0) {
    return <div className="text-muted text-center py-3">Không có sản phẩm trong đơn</div>
  }

  return (
    <div className="table-responsive">
      <Table hover size="sm" className="mb-0 align-middle">
        <thead className="table-light">
          <tr>
            <th>Sản phẩm</th>
            <th>Loại</th>
            <th className="text-center">SL</th>
            <th className="text-end">Đơn giá</th>
            <th className="text-end">Thành tiền</th>
          </tr>
        </thead>
        <tbody>
          {items.map((item) => (
            <tr key={item.id}>
              <td>
                <div className="fw-medium">{item.productName}</div>
                {item.variantName ? (
                  <div className="text-muted fs-xs">{item.variantName}</div>
                ) : null}
                {item.sku ? <div className="text-muted fs-xs">SKU: {item.sku}</div> : null}
              </td>
              <td>{enumLabel(item.itemType, ORDER_ITEM_TYPE_LABELS)}</td>
              <td className="text-center">{item.quantity}</td>
              <td className="text-end">{formatCurrency(item.unitPrice, item.currency || currency)}</td>
              <td className="text-end fw-medium">
                {formatCurrency(item.totalPrice, item.currency || currency)}
              </td>
            </tr>
          ))}
        </tbody>
      </Table>
    </div>
  )
}

function sortHistories(histories: OrderHistory[]): OrderHistory[] {
  return [...histories].sort(
    (a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime(),
  )
}

function OrderHistoryList({ histories }: { histories: OrderHistory[] }) {
  const sorted = sortHistories(histories)

  if (sorted.length === 0) {
    return <div className="text-muted text-center py-3">Chưa có lịch sử thay đổi</div>
  }

  return (
    <div className="timeline-list">
      {sorted.map((history, index) => (
        <div
          key={history.id}
          className={`d-flex gap-3 pb-3 ${index < sorted.length - 1 ? 'border-bottom mb-3' : ''}`}>
          <div className="flex-shrink-0 mt-1">
            <span className="badge bg-light text-dark border">
              {enumLabel(history.toStatus, ORDER_STATUS_LABELS)}
            </span>
          </div>
          <div className="flex-grow-1">
            <div className="d-flex flex-wrap justify-content-between gap-1 mb-1">
              <span className="fs-sm fw-medium">
                {enumLabel(history.fromStatus, ORDER_STATUS_LABELS)}
                {' → '}
                {enumLabel(history.toStatus, ORDER_STATUS_LABELS)}
              </span>
              <span className="text-muted fs-xs">{formatDateTime(history.createdAt)}</span>
            </div>
            {history.note ? <div className="text-muted fs-sm">{history.note}</div> : null}
            {history.changedBy ? (
              <div className="text-muted fs-xs mt-1">Bởi: {history.changedBy}</div>
            ) : null}
          </div>
        </div>
      ))}
    </div>
  )
}

export function OrderDetailContent({ order }: { order: OrderDetail }) {
  return (
    <>
      <OrderDetailHeader order={order} />

      <Row className="g-3 mb-3">
        <Col md={6} xl={3}>
          <OrderCustomerCard order={order} />
        </Col>
        <Col md={6} xl={3}>
          <OrderPaymentCard order={order} />
        </Col>
        <Col md={6} xl={3}>
          <OrderTimelineCard order={order} />
        </Col>
        <Col md={6} xl={3}>
          <OrderSummaryCard order={order} />
        </Col>
      </Row>

      <Card className="mb-3">
        <CardHeader className="py-2 bg-light bg-opacity-50 d-flex justify-content-between align-items-center">
          <span className="fw-semibold fs-sm">Sản phẩm ({order.items.length})</span>
        </CardHeader>
        <CardBody className="p-0">
          <OrderItemsTable items={order.items} currency={order.currency} />
        </CardBody>
      </Card>

      <Card>
        <CardHeader className="py-2 bg-light bg-opacity-50">
          <span className="fw-semibold fs-sm">Lịch sử đơn hàng ({order.histories.length})</span>
        </CardHeader>
        <CardBody className="py-3">
          <OrderHistoryList histories={order.histories} />
        </CardBody>
      </Card>
    </>
  )
}
