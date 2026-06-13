import { Col, Container, Row } from 'react-bootstrap'

import PageMetaData from '@/components/PageMetaData'

import BestSellingPackages from './components/BestSellingPackages'
import DashboardHeader from './components/DashboardHeader'
import OrderStatusCard from './components/OrderStatusCard'
import PaymentMethodsCard from './components/PaymentMethodsCard'
import RecentOrdersTable from './components/RecentOrdersTable'
import RevenueSevenDaysCard from './components/RevenueSevenDaysCard'
import StatCards from './components/StatCards'
import TopCountriesCard from './components/TopCountriesCard'
import TopRegionsCard from './components/TopRegionsCard'

const Page = () => (
  <>
    <PageMetaData title="Dashboard" />
    <Container fluid>
      <DashboardHeader />
      <StatCards />

      <Row className="g-3 mb-3">
        <RevenueSevenDaysCard />
        <OrderStatusCard />
        <TopCountriesCard />
      </Row>

      <Row className="g-3">
        <RecentOrdersTable />
        <Col xxl={4}>
          <BestSellingPackages />
          <PaymentMethodsCard />
          <TopRegionsCard />
        </Col>
      </Row>
    </Container>
  </>
)

export default Page
