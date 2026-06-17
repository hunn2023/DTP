import { useMemo } from 'react'
import { Card, CardBody, CardHeader, CardTitle, Col } from 'react-bootstrap'

import { buildTopCountriesLegend } from '@/features/dashboard/dashboardWorldMap'
import { useDashboard } from '@/features/dashboard/DashboardContext'
import { formatReportMoney } from '@/features/reports/reportFormat'

import TopCountriesWorldMap from './TopCountriesWorldMap'

const TopCountriesCard = () => {
  const { dashboard } = useDashboard()
  const countries = dashboard?.topCountries ?? []
  const legend = useMemo(() => buildTopCountriesLegend(countries), [countries])

  return (
    <Col lg={4}>
      <Card className="h-100">
        <CardHeader className="border-dashed">
          <CardTitle as="h5" className="mb-0">
            Quốc gia bán chạy
          </CardTitle>
        </CardHeader>
        <CardBody>
          {countries.length === 0 ? (
            <div className="text-center text-muted py-5">Chưa có dữ liệu</div>
          ) : (
            <>
              <TopCountriesWorldMap items={countries} height={220} />
              <div className="dashboard-countries-legend mt-3">
                {legend.map((item, index) => (
                  <div key={`${item.label}-${index}`} className="dashboard-countries-legend__row">
                    <span className="fs-sm text-truncate">{item.label}</span>
                    <span className="report-value-money fs-sm text-nowrap">{formatReportMoney(item.value)}</span>
                  </div>
                ))}
              </div>
            </>
          )}
        </CardBody>
      </Card>
    </Col>
  )
}

export default TopCountriesCard
