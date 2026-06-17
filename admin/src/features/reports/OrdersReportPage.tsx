import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchOrdersReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import {
  orderStatusColumns,
  timeSeriesMoneyColumns,
  timeSeriesRowKey,
  topItemRowKey,
} from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchOrdersReport>>): ReportKpi[] {
  return [
    { label: 'Tổng đơn', value: formatReportCount(data.totalOrders), variant: 'primary' },
    { label: 'Đang xử lý', value: formatReportCount(data.processingOrders), variant: 'warning' },
    { label: 'Hoàn thành', value: formatReportCount(data.completedOrders), variant: 'success' },
    { label: 'Đã hủy', value: formatReportCount(data.cancelledOrders), variant: 'danger' },
    { label: 'Tổng giá trị', value: formatReportMoney(data.totalOrderAmount), variant: 'info' },
    { label: 'Giá trị TB / đơn', value: formatReportMoney(data.averageOrderAmount), variant: 'secondary' },
  ]
}

const OrdersReportPage = () => {
  const fetcher = useCallback((range: Parameters<typeof fetchOrdersReport>[0]) => fetchOrdersReport(range), [])
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Đơn hàng"
      description="Thống kê đơn hàng theo thời gian — API admin/reports/orders."
      filters={report.filters}
      onFiltersChange={report.setFilters}
      onApplyFilters={report.applyFilters}
      isLoading={report.isLoading}
      error={report.error}>
      {report.data && (
        <>
          <ReportKpiGrid items={buildKpis(report.data)} />
          <Row className="g-3">
            <Col lg={7}>
              <ReportDataTable
                title="Đơn theo thời gian"
                accent="primary"
                columns={timeSeriesMoneyColumns}
                rows={report.data.ordersByDate}
                rowKey={timeSeriesRowKey}
              />
            </Col>
            <Col lg={5}>
              <ReportDataTable
                title="Theo trạng thái"
                accent="warning"
                columns={orderStatusColumns}
                rows={report.data.ordersByStatus}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default OrdersReportPage
