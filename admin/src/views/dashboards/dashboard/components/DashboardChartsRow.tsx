import { Row } from 'react-bootstrap'

import OrderStatusCard from './OrderStatusCard'
import RevenueSevenDaysCard from './RevenueSevenDaysCard'
import TopCountriesCard from './TopCountriesCard'

const DashboardChartsRow = () => (
  <Row className="g-3 mb-3">
    <RevenueSevenDaysCard />
    <OrderStatusCard />
    <TopCountriesCard />
  </Row>
)

export default DashboardChartsRow
