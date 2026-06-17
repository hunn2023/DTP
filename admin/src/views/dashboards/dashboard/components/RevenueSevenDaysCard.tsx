import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { buildRevenueChartOptions } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'

import { RevenueSevenDaysChart } from './charts'

const RevenueSevenDaysCard = () => {
  const { dashboard } = useDashboard()
  const getOptions = useMemo(
    () => () => buildRevenueChartOptions(dashboard?.revenueChart ?? []),
    [dashboard?.revenueChart],
  )

  return (
    <Col lg={6}>
      <Card className="h-100">
        <CardHeader className="border-dashed">
          <CardTitle as="h5" className="mb-0">
            Doanh thu theo ngày
          </CardTitle>
        </CardHeader>
        <CardBody>
          <RevenueSevenDaysChart getOptions={getOptions} />
        </CardBody>
      </Card>
    </Col>
  )
}

export default RevenueSevenDaysCard
