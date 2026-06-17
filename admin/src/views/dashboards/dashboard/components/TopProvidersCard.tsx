import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { buildTopItemsBarOptions } from '@/features/dashboard/chartOptions'
import { useDashboard } from '@/features/dashboard/DashboardContext'

import { TopItemsBarChart } from './charts'

const TopProvidersCard = () => {
  const { dashboard } = useDashboard()
  const getOptions = useMemo(
    () => () => buildTopItemsBarOptions(dashboard?.topProviders ?? []),
    [dashboard?.topProviders],
  )

  return (
    <Col lg={6}>
      <Card className="h-100">
        <CardHeader className="border-dashed">
          <CardTitle as="h5" className="mb-0">
            Nhà cung cấp hàng đầu
          </CardTitle>
        </CardHeader>
        <CardBody>
          <TopItemsBarChart getOptions={getOptions} />
        </CardBody>
      </Card>
    </Col>
  )
}

export default TopProvidersCard
