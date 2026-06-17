export type ReportDateRange = {
  fromDate: string
  toDate: string
}

/** Khớp BE ReportDateGroupType: Day=1, Week=2, Month=3, Year=4 */
export type ReportGroupType = 1 | 2 | 3 | 4

export const REPORT_GROUP_TYPE_LABELS: Record<ReportGroupType, string> = {
  1: 'Theo ngày',
  2: 'Theo tuần',
  3: 'Theo tháng',
  4: 'Theo năm',
}

export type ReportFilters = ReportDateRange & {
  groupType: ReportGroupType
}

export type ReportTopItem = {
  id: string | null
  code: string
  name: string
  value: number
  count: number
}

export type ReportTimeSeriesPoint = {
  label: string
  date: string
  value: number
  count: number
}

export type SalesReport = {
  fromDate: string
  toDate: string
  totalRevenue: number
  totalDiscount: number
  totalRefund: number
  netRevenue: number
  totalOrders: number
  paidOrders: number
  cancelledOrders: number
  averageOrderValue: number
  revenueByDate: ReportTimeSeriesPoint[]
  revenueByProduct: ReportTopItem[]
  revenueByProvider: ReportTopItem[]
}

export type OrdersReport = {
  totalOrders: number
  pendingOrders: number
  processingOrders: number
  completedOrders: number
  cancelledOrders: number
  failedOrders: number
  totalOrderAmount: number
  averageOrderAmount: number
  ordersByDate: ReportTimeSeriesPoint[]
  ordersByStatus: ReportTopItem[]
}

export type CustomersReport = {
  totalCustomers: number
  newCustomers: number
  returningCustomers: number
  totalCustomerRevenue: number
  averageRevenuePerCustomer: number
  newCustomersByDate: ReportTimeSeriesPoint[]
  topCustomers: ReportTopItem[]
}

export type ProvidersReport = {
  totalProviders: number
  activeProviders: number
  inactiveProviders: number
  totalProviderRevenue: number
  totalProviderOrders: number
  revenueByProvider: ReportTopItem[]
  ordersByProvider: ReportTopItem[]
}

export type ProductsReport = {
  totalProducts: number
  activeProducts: number
  inactiveProducts: number
  totalSoldQuantity: number
  totalProductRevenue: number
  topSellingProducts: ReportTopItem[]
  lowSellingProducts: ReportTopItem[]
  revenueByCategory: ReportTopItem[]
}

export type PaymentsReport = {
  totalPayments: number
  successPayments: number
  pendingPayments: number
  failedPayments: number
  refundedPayments: number
  totalPaidAmount: number
  totalRefundedAmount: number
  paymentsByDate: ReportTimeSeriesPoint[]
  paymentsByMethod: ReportTopItem[]
}

export type DashboardReport = {
  totalRevenue: number
  todayRevenue: number
  monthRevenue: number
  totalOrders: number
  todayOrders: number
  monthOrders: number
  completedOrders: number
  pendingOrders: number
  cancelledOrders: number
  totalCustomers: number
  newCustomersToday: number
  newCustomersThisMonth: number
  totalPaidAmount: number
  totalRefundAmount: number
  revenueChart: ReportTimeSeriesPoint[]
  orderChart: ReportTimeSeriesPoint[]
  topProducts: ReportTopItem[]
  topProviders: ReportTopItem[]
  topCountries: ReportTopItem[]
  topRegions: ReportTopItem[]
}

export type ReportKpiVariant = 'primary' | 'success' | 'info' | 'warning' | 'danger' | 'secondary'

export type ReportKpi = {
  label: string
  value: string
  hint?: string
  variant?: ReportKpiVariant
}

export type ReportTableAccent = 'primary' | 'success' | 'info' | 'warning' | 'danger' | 'secondary'
