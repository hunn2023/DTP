import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { OrderStatusChart } from './charts'

const OrderStatusCard = () => (
  <Col xxl={3} lg={6}>
    <Card className="h-100">
      <CardHeader className="border-dashed">
        <CardTitle as="h5" className="mb-0">
          Trạng thái đơn hàng
        </CardTitle>
      </CardHeader>
      <CardBody>
        <OrderStatusChart />
      </CardBody>
    </Card>
  </Col>
)

export default OrderStatusCard
