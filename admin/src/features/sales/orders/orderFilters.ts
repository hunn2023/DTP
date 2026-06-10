import type { OrdersQueryFilters } from '@/features/sales/orders/orders.api'

export type OrderPageConfig = {
  path: string
  title: string
  description: string
  filters: OrdersQueryFilters
  searchPlaceholder: string
}

const salesSub = 'Bán hàng'

export const ORDER_PAGE_CONFIGS: OrderPageConfig[] = [
  {
    path: '/orders/all',
    title: 'Tất cả đơn hàng',
    description: 'Xem, lọc, ghi chú — không CRUD tự do.',
    filters: {},
    searchPlaceholder: 'Tìm mã đơn, khách...',
  },
  {
    path: '/orders/pending',
    title: 'Chờ thanh toán',
    description: 'Đơn chờ thanh toán.',
    filters: { status: 2 },
    searchPlaceholder: 'Tìm mã đơn chờ thanh toán...',
  },
  {
    path: '/orders/paid',
    title: 'Đã thanh toán',
    description: 'Đơn đã thanh toán.',
    filters: { paymentStatus: 3 },
    searchPlaceholder: 'Tìm đơn đã thanh toán...',
  },
  {
    path: '/orders/processing',
    title: 'Đang cấp hàng',
    description: 'Đơn đang xử lý / cấp hàng.',
    filters: { status: 4 },
    searchPlaceholder: 'Tìm đơn đang xử lý...',
  },
  {
    path: '/orders/delivered',
    title: 'Đã giao',
    description: 'Đơn đã hoàn thành.',
    filters: { status: 5 },
    searchPlaceholder: 'Tìm đơn đã giao...',
  },
  {
    path: '/orders/failed',
    title: 'Đơn lỗi',
    description: 'Retry, hoàn tiền, ghi chú.',
    filters: { status: 7 },
    searchPlaceholder: 'Tìm đơn lỗi...',
  },
  {
    path: '/orders/cancelled',
    title: 'Đã hủy',
    description: 'Đơn đã hủy.',
    filters: { status: 6 },
    searchPlaceholder: 'Tìm đơn đã hủy...',
  },
  {
    path: '/orders/refunded',
    title: 'Hoàn tiền',
    description: 'Đơn đã hoàn tiền.',
    filters: { paymentStatus: 5 },
    searchPlaceholder: 'Tìm đơn hoàn tiền...',
  },
]

export const ORDER_BREADCRUMB_SUBTITLE = salesSub
