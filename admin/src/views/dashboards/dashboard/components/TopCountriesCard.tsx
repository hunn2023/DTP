import { Card, CardBody, CardHeader, CardTitle, Col, Form } from 'react-bootstrap'

import { TopCountriesBarChart } from './charts'

const TopCountriesCard = () => (
  <Col xxl={4} lg={6}>
    <Card className="h-100">
      <CardHeader className="border-dashed d-flex align-items-center justify-content-between">
        <CardTitle as="h5" className="mb-0">
          Quốc gia bán chạy
        </CardTitle>
        <Form.Select size="sm" style={{ width: 'auto' }} defaultValue="7d" aria-label="Khoảng thời gian">
          <option value="7d">7 ngày qua</option>
        </Form.Select>
      </CardHeader>
      <CardBody>
        <TopCountriesBarChart />
      </CardBody>
    </Card>
  </Col>
)

export default TopCountriesCard
