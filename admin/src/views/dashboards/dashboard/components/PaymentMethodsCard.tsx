import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { buildPaymentMethodsChartOptions } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'

import { PaymentMethodsChart } from './charts'

const PaymentMethodsCard = () => {
  const { paymentsReport } = useDashboard()
  const getOptions = useMemo(
    () => () => buildPaymentMethodsChartOptions(paymentsReport),
    [paymentsReport],
  )

  return (
    <Col lg={4}>
      <Card className="h-100">
        <CardHeader className="border-dashed">
          <CardTitle as="h5" className="mb-0">
            Phương thức thanh toán
          </CardTitle>
        </CardHeader>
        <CardBody>
          <PaymentMethodsChart getOptions={getOptions} />
        </CardBody>
      </Card>
    </Col>
  )
}

export default PaymentMethodsCard
