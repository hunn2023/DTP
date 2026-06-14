import { lazy, Suspense } from 'react'
import { Col, Container, Row } from 'react-bootstrap'

import Loader from '@/components/Loader'
import PageMetaData from '@/components/PageMetaData'

import DashboardHeader from './components/DashboardHeader'
import RecentOrdersTable from './components/RecentOrdersTable'
import StatCards from './components/StatCards'

const DashboardChartsRow = lazy(() => import('./components/DashboardChartsRow'))
const DashboardSideWidgets = lazy(() => import('./components/DashboardSideWidgets'))

const Page = () => (
  <>
    <PageMetaData title="Dashboard" />
    <Container fluid>
      <DashboardHeader />
      <StatCards />

      <Suspense fallback={<Loader height="320px" />}>
        <DashboardChartsRow />
      </Suspense>

      <Row className="g-3">
        <RecentOrdersTable />
        <Col xxl={4}>
          <Suspense fallback={<Loader height="520px" />}>
            <DashboardSideWidgets />
          </Suspense>
        </Col>
      </Row>
    </Container>
  </>
)

export default Page
