import { Link } from 'react-router'
import { Card, CardBody, CardHeader, CardTitle, Col, Table } from 'react-bootstrap'

import { useDashboard } from '@/features/dashboard/DashboardContext'
import { formatCurrency, formatDateTime, formatPaymentMethod } from '@/features/sales/shared/format'
import { OrderPaymentStatusBadge } from '@/features/sales/shared/OrderStatusBadges'

const RecentOrdersTable = () => {
  const { recentOrders } = useDashboard()

  return (
    <Col xxl={8}>
      <Card className="h-100">
        <CardHeader className="border-dashed d-flex align-items-center justify-content-between">
          <CardTitle as="h5" className="mb-0">
            Đơn hàng gần đây
          </CardTitle>
          <Link to="/orders/all" className="link-reset fw-semibold fs-sm">
            Xem tất cả
          </Link>
        </CardHeader>
        <CardBody className="p-0">
          {recentOrders.length === 0 ? (
            <div className="text-center text-muted py-5">Chưa có đơn hàng</div>
          ) : (
            <div className="table-responsive">
              <Table hover className="table-custom table-nowrap table-centered mb-0">
                <thead className="bg-light bg-opacity-25 thead-sm">
                  <tr className="text-uppercase fs-xxs">
                    <th className="text-muted">Mã đơn</th>
                    <th className="text-muted">Khách hàng</th>
                    <th className="text-muted">Thanh toán</th>
                    <th className="text-muted">Trạng thái</th>
                    <th className="text-muted">Ngày tạo</th>
                    <th className="text-muted text-end">Giá trị</th>
                  </tr>
                </thead>
                <tbody>
                  {recentOrders.map((order) => (
                    <tr key={order.id}>
                      <td>
                        <Link to="/orders/all" className="fw-semibold link-primary">
                          {order.orderCode || order.id}
                        </Link>
                      </td>
                      <td>
                        <div className="fw-medium">{order.customerName || '—'}</div>
                        {order.customerEmail && <div className="text-muted fs-xxs">{order.customerEmail}</div>}
                      </td>
                      <td>{formatPaymentMethod(order.paymentMethod)}</td>
                      <td>
                        <OrderPaymentStatusBadge paymentStatus={order.paymentStatus} />
                      </td>
                      <td>
                        <span className="text-muted fs-sm">{formatDateTime(order.createdAt)}</span>
                      </td>
                      <td className="text-end fw-semibold">{formatCurrency(order.totalAmount, order.currency)}</td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </div>
          )}
        </CardBody>
      </Card>
    </Col>
  )
}

export default RecentOrdersTable
