import { lazy, Suspense } from 'react'
import { Alert, Col, Container, Row } from 'react-bootstrap'

import Loader from '@/components/Loader'
import PageMetaData from '@/components/PageMetaData'
import { DashboardProvider } from '@/features/dashboard/DashboardContext'
import { useDashboardPage } from '@/features/dashboard/useDashboardPage'

import DashboardHeader from './components/DashboardHeader'
import RecentOrdersTable from './components/RecentOrdersTable'
import StatCards from './components/StatCards'
import './dashboard.scss'

const DashboardChartsRow = lazy(() => import('./components/DashboardChartsRow'))
const DashboardSideWidgets = lazy(() => import('./components/DashboardSideWidgets'))

const DashboardContent = () => {
  const dashboardState = useDashboardPage()

  return (
    <DashboardProvider value={dashboardState}>
      <DashboardHeader />
      {dashboardState.error && (
        <Alert variant="danger" className="mb-3 border-0 shadow-sm">
          {dashboardState.error}
        </Alert>
      )}
      {dashboardState.isLoading ? (
        <Loader height="480px" />
      ) : (
        <>
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
        </>
      )}
    </DashboardProvider>
  )
}

const Page = () => (
  <>
    <PageMetaData title="Dashboard" />
    <Container fluid className="dashboard-page">
      <DashboardContent />
    </Container>
  </>
)

export default Page
