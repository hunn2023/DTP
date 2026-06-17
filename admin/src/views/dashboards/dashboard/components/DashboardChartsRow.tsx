import { Row } from 'react-bootstrap'

import OrderStatusCard from './OrderStatusCard'
import PaymentMethodsCard from './PaymentMethodsCard'
import RevenueSevenDaysCard from './RevenueSevenDaysCard'
import TopCountriesCard from './TopCountriesCard'
import TopProvidersCard from './TopProvidersCard'

const DashboardChartsRow = () => (
  <>
    <Row className="g-3 mb-3">
      <RevenueSevenDaysCard />
      <TopProvidersCard />
    </Row>
    <Row className="g-3 mb-3">
      <OrderStatusCard />
      <TopCountriesCard />
      <PaymentMethodsCard />
    </Row>
  </>
)

export default DashboardChartsRow
