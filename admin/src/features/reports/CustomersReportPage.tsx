import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchCustomersReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import {
  timeSeriesCountColumns,
  timeSeriesRowKey,
  topItemMoneyColumns,
  topItemRowKey,
} from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchCustomersReport>>): ReportKpi[] {
  return [
    { label: 'Tổng khách hàng', value: formatReportCount(data.totalCustomers), variant: 'primary' },
    { label: 'Khách mới', value: formatReportCount(data.newCustomers), variant: 'success' },
    { label: 'Khách quay lại', value: formatReportCount(data.returningCustomers), variant: 'info' },
    { label: 'Tổng doanh thu', value: formatReportMoney(data.totalCustomerRevenue), variant: 'warning' },
    { label: 'TB / khách', value: formatReportMoney(data.averageRevenuePerCustomer), variant: 'secondary' },
  ]
}

const CustomersReportPage = () => {
  const fetcher = useCallback(
    (range: Parameters<typeof fetchCustomersReport>[0]) => fetchCustomersReport(range),
    [],
  )
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Khách hàng"
      description="Thống kê khách hàng — API admin/reports/customers."
      filters={report.filters}
      onFiltersChange={report.setFilters}
      onApplyFilters={report.applyFilters}
      isLoading={report.isLoading}
      error={report.error}>
      {report.data && (
        <>
          <ReportKpiGrid items={buildKpis(report.data)} />
          <Row className="g-3">
            <Col lg={6}>
              <ReportDataTable
                title="Khách mới theo thời gian"
                accent="success"
                columns={timeSeriesCountColumns}
                rows={report.data.newCustomersByDate}
                rowKey={timeSeriesRowKey}
              />
            </Col>
            <Col lg={6}>
              <ReportDataTable
                title="Top khách hàng"
                accent="primary"
                columns={topItemMoneyColumns}
                rows={report.data.topCustomers}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default CustomersReportPage
