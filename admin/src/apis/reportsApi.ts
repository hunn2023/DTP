import { API_PATHS } from '@/shared/config/api'
import { readNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

import type {
  CustomersReport,
  DashboardReport,
  OrdersReport,
  PaymentsReport,
  ProductsReport,
  ProvidersReport,
  ReportDateRange,
  ReportFilters,
  ReportTimeSeriesPoint,
  ReportTopItem,
  SalesReport,
} from '@/features/reports/reportTypes'
import { toReportQueryParams } from '@/features/reports/reportDateRange'

type Raw = Record<string, unknown>

function normalizeTopItem(raw: Raw): ReportTopItem {
  const idRaw = raw.id ?? raw.Id
  return {
    id: idRaw == null ? null : String(idRaw),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    value: readNumber(raw, 'value', 'Value'),
    count: readNumber(raw, 'count', 'Count'),
  }
}

function normalizeTimeSeries(raw: Raw): ReportTimeSeriesPoint {
  return {
    label: readString(raw, 'label', 'Label'),
    date: readString(raw, 'date', 'Date'),
    value: readNumber(raw, 'value', 'Value'),
    count: readNumber(raw, 'count', 'Count'),
  }
}

function readTopItems(raw: Raw, camel: string, pascal: string): ReportTopItem[] {
  const items = (raw[camel] ?? raw[pascal] ?? []) as Raw[]
  return Array.isArray(items) ? items.map(normalizeTopItem) : []
}

function readTimeSeries(raw: Raw, camel: string, pascal: string): ReportTimeSeriesPoint[] {
  const items = (raw[camel] ?? raw[pascal] ?? []) as Raw[]
  return Array.isArray(items) ? items.map(normalizeTimeSeries) : []
}

function normalizeSalesReport(raw: Raw): SalesReport {
  return {
    fromDate: readString(raw, 'fromDate', 'FromDate'),
    toDate: readString(raw, 'toDate', 'ToDate'),
    totalRevenue: readNumber(raw, 'totalRevenue', 'TotalRevenue'),
    totalDiscount: readNumber(raw, 'totalDiscount', 'TotalDiscount'),
    totalRefund: readNumber(raw, 'totalRefund', 'TotalRefund'),
    netRevenue: readNumber(raw, 'netRevenue', 'NetRevenue'),
    totalOrders: readNumber(raw, 'totalOrders', 'TotalOrders'),
    paidOrders: readNumber(raw, 'paidOrders', 'PaidOrders'),
    cancelledOrders: readNumber(raw, 'cancelledOrders', 'CancelledOrders'),
    averageOrderValue: readNumber(raw, 'averageOrderValue', 'AverageOrderValue'),
    revenueByDate: readTimeSeries(raw, 'revenueByDate', 'RevenueByDate'),
    revenueByProduct: readTopItems(raw, 'revenueByProduct', 'RevenueByProduct'),
    revenueByProvider: readTopItems(raw, 'revenueByProvider', 'RevenueByProvider'),
  }
}

function normalizeOrdersReport(raw: Raw): OrdersReport {
  return {
    totalOrders: readNumber(raw, 'totalOrders', 'TotalOrders'),
    pendingOrders: readNumber(raw, 'pendingOrders', 'PendingOrders'),
    processingOrders: readNumber(raw, 'processingOrders', 'ProcessingOrders'),
    completedOrders: readNumber(raw, 'completedOrders', 'CompletedOrders'),
    cancelledOrders: readNumber(raw, 'cancelledOrders', 'CancelledOrders'),
    failedOrders: readNumber(raw, 'failedOrders', 'FailedOrders'),
    totalOrderAmount: readNumber(raw, 'totalOrderAmount', 'TotalOrderAmount'),
    averageOrderAmount: readNumber(raw, 'averageOrderAmount', 'AverageOrderAmount'),
    ordersByDate: readTimeSeries(raw, 'ordersByDate', 'OrdersByDate'),
    ordersByStatus: readTopItems(raw, 'ordersByStatus', 'OrdersByStatus'),
  }
}

function normalizeCustomersReport(raw: Raw): CustomersReport {
  return {
    totalCustomers: readNumber(raw, 'totalCustomers', 'TotalCustomers'),
    newCustomers: readNumber(raw, 'newCustomers', 'NewCustomers'),
    returningCustomers: readNumber(raw, 'returningCustomers', 'ReturningCustomers'),
    totalCustomerRevenue: readNumber(raw, 'totalCustomerRevenue', 'TotalCustomerRevenue'),
    averageRevenuePerCustomer: readNumber(raw, 'averageRevenuePerCustomer', 'AverageRevenuePerCustomer'),
    newCustomersByDate: readTimeSeries(raw, 'newCustomersByDate', 'NewCustomersByDate'),
    topCustomers: readTopItems(raw, 'topCustomers', 'TopCustomers'),
  }
}

function normalizeProvidersReport(raw: Raw): ProvidersReport {
  return {
    totalProviders: readNumber(raw, 'totalProviders', 'TotalProviders'),
    activeProviders: readNumber(raw, 'activeProviders', 'ActiveProviders'),
    inactiveProviders: readNumber(raw, 'inactiveProviders', 'InactiveProviders'),
    totalProviderRevenue: readNumber(raw, 'totalProviderRevenue', 'TotalProviderRevenue'),
    totalProviderOrders: readNumber(raw, 'totalProviderOrders', 'TotalProviderOrders'),
    revenueByProvider: readTopItems(raw, 'revenueByProvider', 'RevenueByProvider'),
    ordersByProvider: readTopItems(raw, 'ordersByProvider', 'OrdersByProvider'),
  }
}

function normalizeProductsReport(raw: Raw): ProductsReport {
  return {
    totalProducts: readNumber(raw, 'totalProducts', 'TotalProducts'),
    activeProducts: readNumber(raw, 'activeProducts', 'ActiveProducts'),
    inactiveProducts: readNumber(raw, 'inactiveProducts', 'InactiveProducts'),
    totalSoldQuantity: readNumber(raw, 'totalSoldQuantity', 'TotalSoldQuantity'),
    totalProductRevenue: readNumber(raw, 'totalProductRevenue', 'TotalProductRevenue'),
    topSellingProducts: readTopItems(raw, 'topSellingProducts', 'TopSellingProducts'),
    lowSellingProducts: readTopItems(raw, 'lowSellingProducts', 'LowSellingProducts'),
    revenueByCategory: readTopItems(raw, 'revenueByCategory', 'RevenueByCategory'),
  }
}

function normalizePaymentsReport(raw: Raw): PaymentsReport {
  return {
    totalPayments: readNumber(raw, 'totalPayments', 'TotalPayments'),
    successPayments: readNumber(raw, 'successPayments', 'SuccessPayments'),
    pendingPayments: readNumber(raw, 'pendingPayments', 'PendingPayments'),
    failedPayments: readNumber(raw, 'failedPayments', 'FailedPayments'),
    refundedPayments: readNumber(raw, 'refundedPayments', 'RefundedPayments'),
    totalPaidAmount: readNumber(raw, 'totalPaidAmount', 'TotalPaidAmount'),
    totalRefundedAmount: readNumber(raw, 'totalRefundedAmount', 'TotalRefundedAmount'),
    paymentsByDate: readTimeSeries(raw, 'paymentsByDate', 'PaymentsByDate'),
    paymentsByMethod: readTopItems(raw, 'paymentsByMethod', 'PaymentsByMethod'),
  }
}

function normalizeDashboardReport(raw: Raw): DashboardReport {
  return {
    totalRevenue: readNumber(raw, 'totalRevenue', 'TotalRevenue'),
    todayRevenue: readNumber(raw, 'todayRevenue', 'TodayRevenue'),
    monthRevenue: readNumber(raw, 'monthRevenue', 'MonthRevenue'),
    totalOrders: readNumber(raw, 'totalOrders', 'TotalOrders'),
    todayOrders: readNumber(raw, 'todayOrders', 'TodayOrders'),
    monthOrders: readNumber(raw, 'monthOrders', 'MonthOrders'),
    completedOrders: readNumber(raw, 'completedOrders', 'CompletedOrders'),
    pendingOrders: readNumber(raw, 'pendingOrders', 'PendingOrders'),
    cancelledOrders: readNumber(raw, 'cancelledOrders', 'CancelledOrders'),
    totalCustomers: readNumber(raw, 'totalCustomers', 'TotalCustomers'),
    newCustomersToday: readNumber(raw, 'newCustomersToday', 'NewCustomersToday'),
    newCustomersThisMonth: readNumber(raw, 'newCustomersThisMonth', 'NewCustomersThisMonth'),
    totalPaidAmount: readNumber(raw, 'totalPaidAmount', 'TotalPaidAmount'),
    totalRefundAmount: readNumber(raw, 'totalRefundAmount', 'TotalRefundAmount'),
    revenueChart: readTimeSeries(raw, 'revenueChart', 'RevenueChart'),
    orderChart: readTimeSeries(raw, 'orderChart', 'OrderChart'),
    topProducts: readTopItems(raw, 'topProducts', 'TopProducts'),
    topProviders: readTopItems(raw, 'topProviders', 'TopProviders'),
    topCountries: readTopItems(raw, 'topCountries', 'TopCountries'),
    topRegions: readTopItems(raw, 'topRegions', 'TopRegions'),
  }
}

async function fetchReport<T>(
  path: string,
  filters: ReportFilters,
  normalize: (raw: Raw) => T,
): Promise<T> {
  const raw = await httpGet<Raw>(path, { params: toReportQueryParams(filters) })
  return normalize(raw)
}

async function fetchReportByDateRange<T>(
  path: string,
  range: ReportDateRange,
  normalize: (raw: Raw) => T,
  extraParams?: Record<string, string | number>,
): Promise<T> {
  const raw = await httpGet<Raw>(path, {
    params: {
      fromDate: range.fromDate,
      toDate: range.toDate,
      ...extraParams,
    },
  })
  return normalize(raw)
}

export function fetchSalesReport(filters: ReportFilters): Promise<SalesReport> {
  return fetchReport(API_PATHS.adminReportsSales, filters, normalizeSalesReport)
}

export function fetchDashboardReport(range: ReportDateRange): Promise<DashboardReport> {
  return fetchReportByDateRange(API_PATHS.adminReportsDashboard, range, normalizeDashboardReport)
}

export function fetchOrdersReport(filters: ReportFilters): Promise<OrdersReport> {
  return fetchReport(API_PATHS.adminReportsOrders, filters, normalizeOrdersReport)
}

export function fetchCustomersReport(filters: ReportFilters): Promise<CustomersReport> {
  return fetchReport(API_PATHS.adminReportsCustomers, filters, normalizeCustomersReport)
}

export function fetchProvidersReport(filters: ReportFilters): Promise<ProvidersReport> {
  return fetchReport(API_PATHS.adminReportsProviders, filters, normalizeProvidersReport)
}

export function fetchProductsReport(filters: ReportFilters): Promise<ProductsReport> {
  return fetchReport(API_PATHS.adminReportsProducts, filters, normalizeProductsReport)
}

export function fetchPaymentsReport(filters: ReportFilters): Promise<PaymentsReport> {
  return fetchReport(API_PATHS.adminReportsPayments, filters, normalizePaymentsReport)
}
