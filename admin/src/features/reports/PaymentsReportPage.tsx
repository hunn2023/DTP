import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchPaymentsReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import {
  paymentMethodColumns,
  timeSeriesMoneyColumns,
  timeSeriesRowKey,
  topItemRowKey,
} from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchPaymentsReport>>): ReportKpi[] {
  return [
    { label: 'Tổng giao dịch', value: formatReportCount(data.totalPayments), variant: 'primary' },
    { label: 'Thành công', value: formatReportCount(data.successPayments), variant: 'success' },
    { label: 'Đang chờ', value: formatReportCount(data.pendingPayments), variant: 'warning' },
    { label: 'Thất bại', value: formatReportCount(data.failedPayments), variant: 'danger' },
    { label: 'Đã hoàn tiền', value: formatReportCount(data.refundedPayments), variant: 'info' },
    { label: 'Tổng đã thu', value: formatReportMoney(data.totalPaidAmount), variant: 'success' },
    { label: 'Tổng hoàn', value: formatReportMoney(data.totalRefundedAmount), variant: 'secondary' },
  ]
}

const PaymentsReportPage = () => {
  const fetcher = useCallback(
    (range: Parameters<typeof fetchPaymentsReport>[0]) => fetchPaymentsReport(range),
    [],
  )
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Thanh toán"
      description="Đối soát thanh toán — API admin/reports/payments."
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
                title="Giao dịch theo thời gian"
                accent="primary"
                columns={timeSeriesMoneyColumns}
                rows={report.data.paymentsByDate}
                rowKey={timeSeriesRowKey}
              />
            </Col>
            <Col lg={5}>
              <ReportDataTable
                title="Theo phương thức"
                accent="info"
                columns={paymentMethodColumns}
                rows={report.data.paymentsByMethod}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default PaymentsReportPage
