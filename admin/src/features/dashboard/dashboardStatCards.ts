import type { IconType } from 'react-icons'
import { TbChartBar, TbPercentage, TbShoppingCart, TbUsers } from 'react-icons/tb'

import type { DashboardReport, PaymentsReport } from '@/features/reports/reportTypes'
import { formatReportCount, formatReportMoney } from '@/features/reports/reportFormat'

export type DashboardStatMetric = {
  label: string
  value: string
}

export type DashboardStatCard = {
  id: number
  title: string
  headline: string
  headlineHint: string
  metrics: DashboardStatMetric[]
  icon: IconType
  iconBg: string
}

function formatSuccessRate(payments: PaymentsReport | null): string {
  if (!payments || payments.totalPayments <= 0) return '—'
  const rate = (payments.successPayments / payments.totalPayments) * 100
  return `${rate.toLocaleString('vi-VN', { maximumFractionDigits: 1 })}%`
}

function moneyMetrics(dashboard: DashboardReport): DashboardStatMetric[] {
  return [
    { label: 'Hôm nay', value: formatReportMoney(dashboard.todayRevenue) },
    { label: 'Tháng này', value: formatReportMoney(dashboard.monthRevenue) },
    { label: 'Đã thanh toán', value: formatReportMoney(dashboard.totalPaidAmount) },
    { label: 'Hoàn tiền', value: formatReportMoney(dashboard.totalRefundAmount) },
  ]
}

function orderMetrics(dashboard: DashboardReport): DashboardStatMetric[] {
  return [
    { label: 'Hôm nay', value: formatReportCount(dashboard.todayOrders) },
    { label: 'Tháng này', value: formatReportCount(dashboard.monthOrders) },
    { label: 'Hoàn thành', value: formatReportCount(dashboard.completedOrders) },
    { label: 'Chờ xử lý', value: formatReportCount(dashboard.pendingOrders) },
    { label: 'Đã hủy', value: formatReportCount(dashboard.cancelledOrders) },
  ]
}

function customerMetrics(dashboard: DashboardReport): DashboardStatMetric[] {
  return [
    { label: 'Mới hôm nay', value: formatReportCount(dashboard.newCustomersToday) },
    { label: 'Mới tháng này', value: formatReportCount(dashboard.newCustomersThisMonth) },
  ]
}

export function buildStatCards(
  dashboard: DashboardReport | null,
  payments: PaymentsReport | null,
): DashboardStatCard[] {
  return [
    {
      id: 1,
      title: 'Doanh thu',
      headline: dashboard ? formatReportMoney(dashboard.totalRevenue) : '—',
      headlineHint: 'Tổng doanh thu',
      metrics: dashboard ? moneyMetrics(dashboard) : [],
      icon: TbChartBar,
      iconBg: 'primary',
    },
    {
      id: 2,
      title: 'Đơn hàng',
      headline: dashboard ? `${formatReportCount(dashboard.totalOrders)} Đơn hàng` : '—',
      headlineHint: 'Tổng số đơn',
      metrics: dashboard ? orderMetrics(dashboard) : [],
      icon: TbShoppingCart,
      iconBg: 'success',
    },
    {
      id: 3,
      title: 'Khách hàng',
      headline: dashboard ? `${formatReportCount(dashboard.totalCustomers)} Khách hàng` : '—',
      headlineHint: 'Tổng số lượng',
      metrics: dashboard ? customerMetrics(dashboard) : [],
      icon: TbUsers,
      iconBg: 'info',
    },
    {
      id: 4,
      title: 'Thanh toán',
      headline: formatSuccessRate(payments),
      headlineHint: 'Tỷ lệ thành công',
      metrics: payments
        ? [
            {
              label: 'Giao dịch',
              value: `${formatReportCount(payments.successPayments)} / ${formatReportCount(payments.totalPayments)}`,
            },
          ]
        : [],
      icon: TbPercentage,
      iconBg: 'secondary',
    },
  ]
}
