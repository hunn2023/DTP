import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, ProgressBar } from 'react-bootstrap'

import { buildTopProductShares } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'

const BestSellingPackages = () => {
  const { dashboard } = useDashboard()
  const items = useMemo(
    () => buildTopProductShares(dashboard?.topProducts ?? []),
    [dashboard?.topProducts],
  )

  return (
    <Card className="mb-3">
      <CardHeader className="border-dashed">
        <CardTitle as="h5" className="mb-0">
          Gói eSIM bán chạy
        </CardTitle>
      </CardHeader>
      <CardBody>
        {items.length === 0 ? (
          <div className="text-center text-muted py-3">Chưa có dữ liệu</div>
        ) : (
          items.map((item, index) => (
            <div key={`${item.name}-${index}`} className="mb-3">
              <div className="d-flex justify-content-between align-items-center mb-1">
                <span className="fw-medium fs-sm">{item.name}</span>
                <span className="text-muted fs-xs">{item.sold.toLocaleString('vi-VN')}</span>
              </div>
              <ProgressBar now={item.percent} variant="primary" style={{ height: 6 }} aria-label={item.name} />
            </div>
          ))
        )}
      </CardBody>
    </Card>
  )
}

export default BestSellingPackages
