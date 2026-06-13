import { Suspense } from 'react'
import { Card, CardBody, CardHeader, CardTitle } from 'react-bootstrap'

import { regionShares } from '../data'
import { TopRegionsMap } from './charts'

const TopRegionsCard = () => (
  <Card>
    <CardHeader className="border-dashed">
      <CardTitle as="h5" className="mb-0">
        Khu vực mua nhiều
      </CardTitle>
    </CardHeader>
    <CardBody>
      <Suspense>
        <TopRegionsMap />
      </Suspense>
      <div className="mt-3">
        {regionShares.map((region) => (
          <div key={region.id} className="d-flex justify-content-between align-items-center mb-2">
            <span className="fs-sm">{region.name}</span>
            <span className="fw-semibold fs-sm">{region.percent.toLocaleString('vi-VN')}%</span>
          </div>
        ))}
      </div>
    </CardBody>
  </Card>
)

export default TopRegionsCard
