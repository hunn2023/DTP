import { Suspense } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col, Form } from 'react-bootstrap'

import { RevenueSevenDaysChart } from './charts'

const RevenueSevenDaysCard = () => (
  <Col xxl={5} lg={12}>
    <Card className="h-100">
      <CardHeader className="border-dashed d-flex align-items-center justify-content-between">
        <CardTitle as="h5" className="mb-0">
          Doanh thu 7 ngày
        </CardTitle>
        <Form.Select size="sm" style={{ width: 'auto' }} defaultValue="7d" aria-label="Khoảng thời gian">
          <option value="7d">7 ngày qua</option>
        </Form.Select>
      </CardHeader>
      <CardBody>
        <Suspense>
          <RevenueSevenDaysChart />
        </Suspense>
      </CardBody>
    </Card>
  </Col>
)

export default RevenueSevenDaysCard
