import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchProvidersReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import { topItemMoneyColumns, topItemRowKey } from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchProvidersReport>>): ReportKpi[] {
  return [
    { label: 'Tổng provider', value: formatReportCount(data.totalProviders), variant: 'primary' },
    { label: 'Đang hoạt động', value: formatReportCount(data.activeProviders), variant: 'success' },
    { label: 'Ngưng hoạt động', value: formatReportCount(data.inactiveProviders), variant: 'secondary' },
    { label: 'Doanh thu', value: formatReportMoney(data.totalProviderRevenue), variant: 'info' },
    { label: 'Tổng đơn', value: formatReportCount(data.totalProviderOrders), variant: 'warning' },
  ]
}

const ProvidersReportPage = () => {
  const fetcher = useCallback(
    (range: Parameters<typeof fetchProvidersReport>[0]) => fetchProvidersReport(range),
    [],
  )
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Provider"
      description="Doanh thu và đơn hàng theo nhà cung cấp — API admin/reports/providers."
      filters={report.filters}
      onFiltersChange={report.setFilters}
      onApplyFilters={report.applyFilters}
      isLoading={report.isLoading}
      error={report.error}
      showGroupType={false}>
      {report.data && (
        <>
          <ReportKpiGrid items={buildKpis(report.data)} />
          <Row className="g-3">
            <Col lg={6}>
              <ReportDataTable
                title="Doanh thu theo provider"
                accent="success"
                columns={topItemMoneyColumns}
                rows={report.data.revenueByProvider}
                rowKey={topItemRowKey}
              />
            </Col>
            <Col lg={6}>
              <ReportDataTable
                title="Đơn theo provider"
                accent="primary"
                columns={topItemMoneyColumns}
                rows={report.data.ordersByProvider}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default ProvidersReportPage
