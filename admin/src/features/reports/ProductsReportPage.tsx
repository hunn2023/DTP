import { Col, Row } from 'react-bootstrap'
import { useCallback } from 'react'

import { fetchProductsReport } from '@/apis/reportsApi'
import ReportDataTable from '@/features/reports/components/ReportDataTable'
import ReportKpiGrid from '@/features/reports/components/ReportKpiGrid'
import ReportPageShell from '@/features/reports/components/ReportPageShell'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'
import { topItemMoneyColumns, topItemRowKey } from '@/features/reports/reportTableColumns'
import type { ReportKpi } from '@/features/reports/reportTypes'
import { useReportPage } from '@/features/reports/useReportPage'

function buildKpis(data: Awaited<ReturnType<typeof fetchProductsReport>>): ReportKpi[] {
  return [
    { label: 'Tổng sản phẩm', value: formatReportCount(data.totalProducts), variant: 'primary' },
    { label: 'Đang bán', value: formatReportCount(data.activeProducts), variant: 'success' },
    { label: 'Ngưng bán', value: formatReportCount(data.inactiveProducts), variant: 'secondary' },
    { label: 'Đã bán (SL)', value: formatReportCount(data.totalSoldQuantity), variant: 'info' },
    { label: 'Doanh thu SP', value: formatReportMoney(data.totalProductRevenue), variant: 'warning' },
  ]
}

const ProductsReportPage = () => {
  const fetcher = useCallback(
    (range: Parameters<typeof fetchProductsReport>[0]) => fetchProductsReport(range),
    [],
  )
  const report = useReportPage(fetcher)

  return (
    <ReportPageShell
      title="Sản phẩm bán chạy"
      description="Top sản phẩm và doanh thu theo danh mục — API admin/reports/products."
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
                title="Bán chạy nhất"
                accent="success"
                columns={topItemMoneyColumns}
                rows={report.data.topSellingProducts}
                rowKey={topItemRowKey}
              />
            </Col>
            <Col lg={6}>
              <ReportDataTable
                title="Bán chậm"
                accent="warning"
                columns={topItemMoneyColumns}
                rows={report.data.lowSellingProducts}
                rowKey={topItemRowKey}
              />
            </Col>
            <Col lg={12}>
              <ReportDataTable
                title="Doanh thu theo danh mục"
                accent="info"
                columns={topItemMoneyColumns}
                rows={report.data.revenueByCategory}
                rowKey={topItemRowKey}
              />
            </Col>
          </Row>
        </>
      )}
    </ReportPageShell>
  )
}

export default ProductsReportPage
