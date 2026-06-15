import { Card, CardBody, CardHeader, CardTitle, ProgressBar } from 'react-bootstrap'

import { bestSellingPackages } from '../data'

const BestSellingPackages = () => (
  <Card className="mb-3">
    <CardHeader className="border-dashed">
      <CardTitle as="h5" className="mb-0">
        Gói eSIM bán chạy
      </CardTitle>
    </CardHeader>
    <CardBody>
      {bestSellingPackages.map((item) => (
        <div key={item.id} className="mb-3">
          <div className="d-flex justify-content-between align-items-center mb-1">
            <span className="fw-medium fs-sm">{item.name}</span>
            <span className="text-muted fs-xs">{item.sold.toLocaleString('vi-VN')}</span>
          </div>
          <ProgressBar now={item.percent} variant="primary" style={{ height: 6 }} aria-label={item.name} />
        </div>
      ))}
    </CardBody>
  </Card>
)

export default BestSellingPackages
