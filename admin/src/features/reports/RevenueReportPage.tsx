import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchSalesReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import {
  timeSeriesMoneyColumns,
  timeSeriesRowKey,
  topItemMoneyColumns,
  topItemRowKey,
} from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchSalesReport>>): ReportKpi[] {
  return [
    { label: 'Tổng doanh thu', value: formatReportMoney(data.totalRevenue), variant: 'primary' },
    { label: 'Thực nhận', value: formatReportMoney(data.netRevenue), variant: 'success', hint: 'Sau giảm giá & hoàn tiền' },
    { label: 'Tổng đơn', value: formatReportCount(data.totalOrders), variant: 'info' },
    { label: 'Đơn đã thanh toán', value: formatReportCount(data.paidOrders), variant: 'warning' },
    { label: 'Giá trị TB / đơn', value: formatReportMoney(data.averageOrderValue), variant: 'secondary' },
    { label: 'Hoàn tiền', value: formatReportMoney(data.totalRefund), variant: 'danger' },
  ]
}

const RevenueReportPage = () => {
  const fetcher = useCallback((range: Parameters<typeof fetchSalesReport>[0]) => fetchSalesReport(range), [])
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Doanh thu"
      description="Báo cáo doanh thu theo khoảng thời gian — dữ liệu từ API admin/reports/sales."
      filters={report.filters}
      onFiltersChange={report.setFilters}
      onApplyFilters={report.applyFilters}
      isLoading={report.isLoading}
      error={report.error}>
      {report.data && (
        <>
          <ReportKpiGrid items={buildKpis(report.data)} />
          <Row className="g-3">
            <Col lg={12}>
              <ReportDataTable
                title="Doanh thu theo thời gian"
                accent="primary"
                columns={timeSeriesMoneyColumns}
                rows={report.data.revenueByDate}
                rowKey={timeSeriesRowKey}
              />
            </Col>
            <Col lg={6}>
              <ReportDataTable
                title="Theo sản phẩm"
                accent="success"
                columns={topItemMoneyColumns}
                rows={report.data.revenueByProduct}
                rowKey={topItemRowKey}
              />
            </Col>
            <Col lg={6}>
              <ReportDataTable
                title="Theo provider"
                accent="info"
                columns={topItemMoneyColumns}
                rows={report.data.revenueByProvider}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default RevenueReportPage
