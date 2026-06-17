import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, ProgressBar } from 'react-bootstrap'

import { buildTopItemShares } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'
import { formatReportMoney } from '@/features/reports/reportFormat'

const TopRegionsCard = () => {
  const { dashboard } = useDashboard()
  const items = useMemo(
    () => buildTopItemShares(dashboard?.topRegions ?? []),
    [dashboard?.topRegions],
  )

  return (
    <Card className="mb-3">
      <CardHeader className="border-dashed">
        <CardTitle as="h5" className="mb-0">
          Khu vực mua nhiều
        </CardTitle>
      </CardHeader>
      <CardBody>
        {items.length === 0 ? (
          <div className="text-center text-muted py-3">Chưa có dữ liệu</div>
        ) : (
          items.map((item, index) => (
            <div key={`${item.name}-${index}`} className="mb-3">
              <div className="d-flex justify-content-between align-items-start gap-2 mb-1">
                <span className="fs-sm">{item.name}</span>
                <div className="text-end flex-shrink-0">
                  <div className="fw-semibold fs-sm">{item.percent.toLocaleString('vi-VN')}%</div>
                  <div className="text-muted fs-xxs">{formatReportMoney(item.value)}</div>
                </div>
              </div>
              <ProgressBar now={item.percent} variant="info" style={{ height: 6 }} aria-label={item.name} />
            </div>
          ))
        )}
      </CardBody>
    </Card>
  )
}

export default TopRegionsCard
