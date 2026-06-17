import type { OrderRow } from '@/apis/ordersApi'
import { fetchOrdersPage } from '@/apis/ordersApi'
import {
  fetchDashboardReport,
  fetchOrdersReport,
  fetchPaymentsReport,
} from '@/apis/reportsApi'
import {
  getDefaultDashboardRange,
  isValidDateRange,
  toDashboardFilters,
} from '@/features/dashboard/dashboardDateRange'
import type { DashboardReport, OrdersReport, PaymentsReport, ReportDateRange } from '@/features/reports/reportTypes'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import { useCallback, useEffect, useState } from 'react'

type DashboardPageState = {
  filters: ReportDateRange
  appliedFilters: ReportDateRange
  setFilters: (next: ReportDateRange) => void
  applyFilters: () => void
  isLoading: boolean
  error: string | null
  dashboard: DashboardReport | null
  ordersReport: OrdersReport | null
  paymentsReport: PaymentsReport | null
  recentOrders: OrderRow[]
  reload: () => void
}

export function useDashboardPage(): DashboardPageState {
  const [filters, setFilters] = useState<ReportDateRange>(() => getDefaultDashboardRange())
  const [appliedFilters, setAppliedFilters] = useState<ReportDateRange>(() => getDefaultDashboardRange())
  const [dashboard, setDashboard] = useState<DashboardReport | null>(null)
  const [ordersReport, setOrdersReport] = useState<OrdersReport | null>(null)
  const [paymentsReport, setPaymentsReport] = useState<PaymentsReport | null>(null)
  const [recentOrders, setRecentOrders] = useState<OrderRow[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const load = useCallback(async () => {
    if (!isValidDateRange(appliedFilters)) {
      setError('Ngày bắt đầu không được lớn hơn ngày kết thúc')
      setIsLoading(false)
      return
    }

    setIsLoading(true)
    setError(null)
    const reportFilters = toDashboardFilters(appliedFilters)

    try {
      const [dashboardData, ordersData, paymentsData, ordersPage] = await Promise.all([
        fetchDashboardReport(appliedFilters),
        fetchOrdersReport(reportFilters),
        fetchPaymentsReport(reportFilters),
        fetchOrdersPage(1, 6),
      ])

      setDashboard(dashboardData)
      setOrdersReport(ordersData)
      setPaymentsReport(paymentsData)
      setRecentOrders(ordersPage.items)
    } catch (err) {
      setError(getErrorMessage(err, 'Không tải được dữ liệu dashboard'))
      setDashboard(null)
      setOrdersReport(null)
      setPaymentsReport(null)
      setRecentOrders([])
    } finally {
      setIsLoading(false)
    }
  }, [appliedFilters])

  useEffect(() => {
    void load()
  }, [load])

  const applyFilters = useCallback(() => {
    setAppliedFilters(filters)
  }, [filters])

  return {
    filters,
    appliedFilters,
    setFilters,
    applyFilters,
    isLoading,
    error,
    dashboard,
    ordersReport,
    paymentsReport,
    recentOrders,
    reload: load,
  }
}
