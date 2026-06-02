import { defineAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import { labels } from '@/views/admin-crud/entities/shared'
import type { SettingsEntityBase } from '@/views/settings/types'

const sub = 'Báo cáo'

export type RevenueReportRow = SettingsEntityBase & {
  date: string
  revenue: number
  paidOrders: number
  refundAmount: number
  netRevenue: number
}

export type ProviderReportRow = SettingsEntityBase & {
  providerName: string
  successRate: number
  failedCount: number
  revenue: number
}

export const revenueReportEntity = defineAdminEntity<RevenueReportRow>({
  path: '/reports/revenue',
  title: 'Doanh thu',
  breadcrumbSubtitle: sub,
  description: 'Lọc theo ngày, export Excel — không CRUD.',
  entityName: 'dòng báo cáo',
  labels: labels('dòng', 'Tìm ngày...', 'Export'),
  seedData: [
    {
      id: 1,
      date: '2026-06-01',
      revenue: 10000000,
      paidOrders: 85,
      refundAmount: 500000,
      netRevenue: 9500000,
      isActive: true,
    },
  ],
  fields: [
    { name: 'date', label: 'Ngày', type: 'text', table: { variant: 'primary' }, form: false },
    { name: 'revenue', label: 'Doanh thu', type: 'number', table: true, form: false },
    { name: 'paidOrders', label: 'Đơn paid', type: 'number', table: true, form: false },
    { name: 'refundAmount', label: 'Hoàn', type: 'number', table: true, form: false },
    { name: 'netRevenue', label: 'Thực nhận', type: 'number', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const ordersReportEntity = defineAdminEntity<RevenueReportRow>({
  path: '/reports/orders',
  title: 'Đơn hàng',
  breadcrumbSubtitle: sub,
  description: 'Thống kê đơn theo thời gian.',
  entityName: 'dòng',
  labels: labels('dòng', 'Tìm...', 'Export'),
  seedData: revenueReportEntity.seedData,
  fields: revenueReportEntity.fields,
})

export const productsReportEntity = defineAdminEntity<SettingsEntityBase & { productCode: string; soldQty: number }>({
  path: '/reports/products',
  title: 'Sản phẩm bán chạy',
  breadcrumbSubtitle: sub,
  description: 'Top sản phẩm.',
  entityName: 'sản phẩm',
  labels: labels('SP', 'Tìm mã...', 'Export'),
  seedData: [{ id: 1, productCode: 'JP_5GB_7D', soldQty: 120, isActive: true }],
  fields: [
    { name: 'productCode', label: 'Mã SP', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'soldQty', label: 'Đã bán', type: 'number', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const customersReportEntity = defineAdminEntity<SettingsEntityBase & { date: string; newCustomers: number }>({
  path: '/reports/customers',
  title: 'Khách hàng',
  breadcrumbSubtitle: sub,
  description: 'Khách mới theo ngày.',
  entityName: 'dòng',
  labels: labels('dòng', 'Tìm...', 'Export'),
  seedData: [{ id: 1, date: '2026-06-01', newCustomers: 42, isActive: true }],
  fields: [
    { name: 'date', label: 'Ngày', type: 'text', table: { variant: 'primary' }, form: false },
    { name: 'newCustomers', label: 'Khách mới', type: 'number', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const paymentsReportEntity = defineAdminEntity<RevenueReportRow>({
  path: '/reports/payments',
  title: 'Thanh toán',
  breadcrumbSubtitle: sub,
  description: 'Đối soát thanh toán.',
  entityName: 'dòng',
  labels: labels('dòng', 'Tìm...', 'Export'),
  seedData: revenueReportEntity.seedData,
  fields: revenueReportEntity.fields,
})

export const providersReportEntity = defineAdminEntity<ProviderReportRow>({
  path: '/reports/providers',
  title: 'Provider',
  breadcrumbSubtitle: sub,
  description: 'Tỷ lệ lỗi, doanh thu theo provider — Phase 2.',
  entityName: 'provider',
  labels: labels('provider', 'Tìm...', 'Export'),
  seedData: [
    { id: 1, providerName: 'Airalo', successRate: 98, failedCount: 4, revenue: 5000000, isActive: true },
  ],
  fields: [
    { name: 'providerName', label: 'Nhà cung cấp', type: 'text', table: { variant: 'primary' }, form: false },
    { name: 'successRate', label: 'Tỷ lệ thành công (%)', type: 'number', table: true, form: false },
    { name: 'failedCount', label: 'Lỗi', type: 'number', table: true, form: false },
    { name: 'revenue', label: 'Doanh thu', type: 'number', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const deliveryFailedReportEntity = defineAdminEntity<SettingsEntityBase & { orderNo: string; error: string }>({
  path: '/reports/delivery-failed',
  title: 'Lỗi cấp hàng',
  breadcrumbSubtitle: sub,
  description: 'Đơn lỗi giao digital.',
  entityName: 'đơn',
  labels: labels('đơn', 'Tìm mã...', 'Export'),
  seedData: [{ id: 1, orderNo: 'EZ202605310003', error: 'Provider timeout', isActive: true }],
  fields: [
    { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'error', label: 'Lỗi', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const refundsReportEntity = defineAdminEntity<RevenueReportRow>({
  path: '/reports/refunds',
  title: 'Hoàn tiền',
  breadcrumbSubtitle: sub,
  description: 'Thống kê hoàn tiền.',
  entityName: 'dòng',
  labels: labels('dòng', 'Tìm...', 'Export'),
  seedData: revenueReportEntity.seedData.map((r) => ({ ...r, revenue: r.refundAmount })),
  fields: revenueReportEntity.fields,
})

export const reportEntities = [
  revenueReportEntity,
  ordersReportEntity,
  productsReportEntity,
  customersReportEntity,
  paymentsReportEntity,
  providersReportEntity,
  deliveryFailedReportEntity,
  refundsReportEntity,
]
