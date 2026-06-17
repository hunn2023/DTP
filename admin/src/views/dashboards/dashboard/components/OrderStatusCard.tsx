import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { buildOrderStatusChartOptions } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'

import { OrderStatusChart } from './charts'

const OrderStatusCard = () => {
  const { ordersReport } = useDashboard()
  const getOptions = useMemo(
    () => () => buildOrderStatusChartOptions(ordersReport),
    [ordersReport],
  )

  return (
    <Col lg={4}>
      <Card className="h-100">
        <CardHeader className="border-dashed">
          <CardTitle as="h5" className="mb-0">
            Trạng thái đơn hàng
          </CardTitle>
        </CardHeader>
        <CardBody>
          <OrderStatusChart getOptions={getOptions} height={220} />
        </CardBody>
      </Card>
    </Col>
  )
}

export default OrderStatusCard
