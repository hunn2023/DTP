import { Card, CardBody, CardHeader, CardTitle } from 'react-bootstrap'

import { PaymentMethodsChart } from './charts'

const PaymentMethodsCard = () => (
  <Card className="mb-3">
    <CardHeader className="border-dashed">
      <CardTitle as="h5" className="mb-0">
        Phương thức thanh toán
      </CardTitle>
    </CardHeader>
    <CardBody>
      <PaymentMethodsChart />
    </CardBody>
  </Card>
)

export default PaymentMethodsCard
