import { Link } from 'react-router'
import { Badge, Card, CardBody, CardHeader, CardTitle, Col, Table } from 'react-bootstrap'

import { recentOrders } from '../data'

const RecentOrdersTable = () => (
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
        <div className="table-responsive">
          <Table hover className="table-custom table-nowrap table-centered mb-0">
            <thead className="bg-light bg-opacity-25 thead-sm">
              <tr className="text-uppercase fs-xxs">
                <th className="text-muted">Mã đơn</th>
                <th className="text-muted">Khách hàng</th>
                <th className="text-muted">Gói eSIM</th>
                <th className="text-muted">Quốc gia</th>
                <th className="text-muted">Thanh toán</th>
                <th className="text-muted">Trạng thái</th>
                <th className="text-muted text-end">Giá trị</th>
              </tr>
            </thead>
            <tbody>
              {recentOrders.map((order) => (
                <tr key={order.id}>
                  <td>
                    <Link to="/orders/all" className="fw-semibold link-primary">
                      {order.id}
                    </Link>
                  </td>
                  <td>{order.customer}</td>
                  <td>{order.packageName}</td>
                  <td>
                    <div className="d-flex align-items-center gap-2">
                      <img src={order.flag} alt="" width={20} height={14} className="rounded" style={{ objectFit: 'cover' }} />
                      <span>{order.country}</span>
                    </div>
                  </td>
                  <td>{order.payment}</td>
                  <td>
                    <Badge bg={order.statusVariant} className={`badge-soft-${order.statusVariant}`}>
                      {order.status}
                    </Badge>
                  </td>
                  <td className="text-end fw-semibold">{order.amount}</td>
                </tr>
              ))}
            </tbody>
          </Table>
        </div>
      </CardBody>
    </Card>
  </Col>
)

export default RecentOrdersTable
