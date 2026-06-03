import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { labels } from '@/modules/crud/entities/shared'
import { isActiveField, sortOrderField } from '@/modules/crud/entities/fieldHelpers'
import { readonlyCrudCapabilities, type EntityFieldDef } from '@/modules/crud/types'
import type { SettingsEntityBase } from '@/modules/crud/types'

const salesSub = 'Bán hàng'
const paySub = salesSub

const paymentProviderOptions = [{ value: '1', label: 'VNPay' }]

export type OrderRow = SettingsEntityBase & {
  orderNo: string
  customerId: number
  customerName: string
  totalAmount: number
  paymentStatus: string
  orderStatus: string
  createdAt: string
  paymentMethod: string
  expiredAt: string
  productName: string
  providerName: string
  processingStartedAt: string
  errorReason: string
  lastProviderResponse: string
  assignedStaff: string
}

const orderSeed: OrderRow[] = [
  {
    id: 1,
    orderNo: 'EZ202605310001',
    customerId: 1,
    customerName: 'Nguyễn Văn A',
    totalAmount: 99000,
    paymentStatus: 'Paid',
    orderStatus: 'Provisioning',
    createdAt: '2026-05-31 14:20',
    paymentMethod: 'VNPay',
    expiredAt: '',
    productName: 'eSIM Nhật 5GB 7 ngày',
    providerName: 'Airalo',
    processingStartedAt: '2026-05-31 14:22',
    errorReason: '',
    lastProviderResponse: '',
    assignedStaff: '',
    isActive: true,
  },
  {
    id: 2,
    orderNo: 'EZ202605310002',
    customerId: 2,
    customerName: 'Trần B',
    totalAmount: 92000,
    paymentStatus: 'PendingPayment',
    orderStatus: 'Pending',
    createdAt: '2026-05-31 15:00',
    paymentMethod: 'VNPay QR',
    expiredAt: '2026-05-31 16:00',
    productName: 'Thẻ Viettel 100k',
    providerName: '',
    processingStartedAt: '',
    errorReason: '',
    lastProviderResponse: '',
    assignedStaff: '',
    isActive: true,
  },
  {
    id: 3,
    orderNo: 'EZ202605310003',
    customerId: 3,
    customerName: 'Lê C',
    totalAmount: 120000,
    paymentStatus: 'Paid',
    orderStatus: 'Failed',
    createdAt: '2026-05-31 16:10',
    paymentMethod: 'Momo',
    expiredAt: '',
    productName: 'eSIM Nhật 5GB',
    providerName: 'Airalo',
    processingStartedAt: '2026-05-31 16:12',
    errorReason: 'Provider hết hàng',
    lastProviderResponse: '{"error":"out_of_stock"}',
    assignedStaff: 'staff@ezsim.vn',
    isActive: true,
  },
]

const orderCapabilities = { create: false, edit: false, view: true, delete: false, toggleActive: false }

const allOrderFields: EntityFieldDef<OrderRow>[] = [
  { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
  { name: 'customerId', label: 'ID khách hàng', type: 'number', table: true, form: false },
  { name: 'customerName', label: 'Khách hàng', type: 'text', table: { variant: 'primary' }, form: false },
  { name: 'totalAmount', label: 'Tổng tiền', type: 'number', table: true, form: false },
  { name: 'paymentStatus', label: 'Trạng thái thanh toán', type: 'text', table: true, form: false },
  { name: 'orderStatus', label: 'Trạng thái đơn', type: 'text', table: true, form: false },
  { name: 'createdAt', label: 'Ngày tạo', type: 'text', table: true, form: false },
  { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
]

const pendingOrderFields: EntityFieldDef<OrderRow>[] = [
  { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
  { name: 'customerName', label: 'Khách hàng', type: 'text', table: { variant: 'primary' }, form: false },
  { name: 'totalAmount', label: 'Số tiền', type: 'number', table: true, form: false },
  { name: 'paymentMethod', label: 'Phương thức TT', type: 'text', table: true, form: false },
  { name: 'expiredAt', label: 'Hết hạn lúc', type: 'text', table: true, form: false },
  { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
]

const processingOrderFields: EntityFieldDef<OrderRow>[] = [
  { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
  { name: 'productName', label: 'Sản phẩm', type: 'text', table: { variant: 'primary' }, form: false },
  { name: 'providerName', label: 'Nhà cung cấp', type: 'text', table: true, form: false },
  { name: 'processingStartedAt', label: 'Bắt đầu xử lý lúc', type: 'text', table: true, form: false },
  { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
]

const failedOrderFields: EntityFieldDef<OrderRow>[] = [
  { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
  { name: 'errorReason', label: 'Lý do lỗi', type: 'text', table: { variant: 'primary' }, form: false },
  { name: 'lastProviderResponse', label: 'Phản hồi provider', type: 'text', table: true, form: false },
  { name: 'assignedStaff', label: 'Nhân viên xử lý', type: 'text', table: true, form: false },
  { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
]

function orderEntity(
  path: string,
  title: string,
  description: string,
  fields: EntityFieldDef<OrderRow>[],
  filter: (rows: OrderRow[]) => OrderRow[],
) {
  return defineAdminEntity<OrderRow>({
    path,
    title,
    breadcrumbSubtitle: salesSub,
    description,
    entityName: 'đơn hàng',
    labels: labels('đơn', 'Tìm mã đơn, khách...', 'Thêm đơn'),
    capabilities: orderCapabilities,
    seedData: orderSeed,
    dataFilter: filter,
    fields,
  })
}

export const ordersAllEntity = orderEntity(
  '/orders/all',
  'Tất cả đơn hàng',
  'Xem, lọc, ghi chú — không CRUD tự do.',
  allOrderFields,
  (r) => r,
)
export const ordersPendingEntity = orderEntity(
  '/orders/pending',
  'Chờ thanh toán',
  'Đơn PendingPayment.',
  pendingOrderFields,
  (r) => r.filter((o) => o.paymentStatus === 'PendingPayment'),
)
export const ordersPaidEntity = orderEntity(
  '/orders/paid',
  'Đã thanh toán',
  'Đơn đã thanh toán.',
  allOrderFields,
  (r) => r.filter((o) => o.paymentStatus === 'Paid'),
)
export const ordersProcessingEntity = orderEntity(
  '/orders/processing',
  'Đang cấp hàng',
  'Provisioning — retry provider.',
  processingOrderFields,
  (r) => r.filter((o) => o.orderStatus === 'Provisioning'),
)
export const ordersDeliveredEntity = orderEntity(
  '/orders/delivered',
  'Đã giao',
  'Đơn đã giao digital.',
  allOrderFields,
  (r) => r.filter((o) => o.orderStatus === 'Delivered'),
)
export const ordersFailedEntity = orderEntity(
  '/orders/failed',
  'Đơn lỗi',
  'Retry, hoàn tiền, ghi chú.',
  failedOrderFields,
  (r) => r.filter((o) => o.orderStatus === 'Failed'),
)
export const ordersCancelledEntity = orderEntity(
  '/orders/cancelled',
  'Đã hủy',
  'Đơn đã hủy.',
  allOrderFields,
  (r) => r.filter((o) => o.orderStatus === 'Cancelled'),
)
export const ordersRefundedEntity = orderEntity(
  '/orders/refunded',
  'Hoàn tiền',
  'Đơn đã hoàn tiền.',
  allOrderFields,
  (r) => r.filter((o) => o.orderStatus === 'Refunded'),
)

export type PaymentTransaction = SettingsEntityBase & {
  orderId: number
  provider: string
  amount: number
  transactionCode: string
  status: string
  paidAt: string
}

export type PaymentProvider = SettingsEntityBase & {
  name: string
  code: string
  merchantId: string
  secretKey: string
}

export type PaymentMethod = SettingsEntityBase & {
  name: string
  providerId: number
  iconUrl: string
  feePercent: number
  sortOrder: number
}

export type PaymentCallback = SettingsEntityBase & {
  provider: string
  rawPayload: string
  signatureValid: boolean
  processStatus: string
  receivedAt: string
}

export type PaymentRefund = SettingsEntityBase & {
  orderId: number
  amount: number
  reason: string
  status: string
  approvedBy: string
  refundedAt: string
}

export type DigitalDelivery = SettingsEntityBase & {
  orderId: number
  deliveryType: string
  status: string
  sentAt: string
  errorMessage: string
}

export type EsimProfile = SettingsEntityBase & {
  orderItemId: number
  iccid: string
  qrCodeUrl: string
  activationCode: string
  status: string
}

export type DeliveredCode = SettingsEntityBase & {
  orderItemId: number
  serial: string
  pinCode: string
  expiredAt: string
  status: string
}

export type DeliveryLog = SettingsEntityBase & {
  deliveryId: number
  action: string
  request: string
  response: string
  errorMessage: string
  createdAt: string
}

export const paymentTransactionsEntity = defineAdminEntity<PaymentTransaction>({
  path: '/payments/transactions',
  title: 'Giao dịch thanh toán',
  breadcrumbSubtitle: paySub,
  description: 'Xem, lọc, đối soát.',
  entityName: 'giao dịch',
  labels: labels('giao dịch', 'Tìm mã GD, đơn...', 'Thêm GD'),
  capabilities: { ...readonlyCrudCapabilities, edit: false },
  seedData: [
    {
      id: 1,
      orderId: 1,
      provider: 'VNPay',
      amount: 99000,
      transactionCode: 'VNP123456',
      status: 'Success',
      paidAt: '2026-05-31 14:21',
      isActive: true,
    },
  ],
  fields: [
    { name: 'transactionCode', label: 'Mã giao dịch', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'orderId', label: 'ID đơn hàng', type: 'number', table: true, form: false },
    { name: 'provider', label: 'Cổng thanh toán', type: 'text', table: true, form: false },
    { name: 'amount', label: 'Số tiền', type: 'number', table: true, form: false },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: false },
    { name: 'paidAt', label: 'Thời gian thanh toán', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const paymentProvidersEntity = defineAdminEntity<PaymentProvider>({
  path: '/payments/providers',
  title: 'Cổng thanh toán',
  breadcrumbSubtitle: paySub,
  description: 'VNPay, Momo — SecretKey mã hóa.',
  entityName: 'cổng TT',
  labels: labels('cổng', 'Tìm tên, mã...', 'Thêm cổng'),
  seedData: [{ id: 1, name: 'VNPay', code: 'VNPAY', merchantId: 'MERCHANT_***', secretKey: '••••••', isActive: true }],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'code', label: 'Mã', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'merchantId', label: 'Merchant ID', type: 'text', form: { required: true } },
    { name: 'secretKey', label: 'Secret key', type: 'password', form: { required: true } },
    isActiveField<PaymentProvider>(),
  ],
})

export const paymentMethodsEntity = defineAdminEntity<PaymentMethod>({
  path: '/payments/methods',
  title: 'Phương thức thanh toán',
  breadcrumbSubtitle: paySub,
  description: 'Hiển thị checkout.',
  entityName: 'phương thức',
  labels: labels('phương thức', 'Tìm tên...', 'Thêm phương thức'),
  seedData: [{ id: 1, name: 'VNPay QR', providerId: 1, iconUrl: '/icons/vnpay.svg', feePercent: 0, sortOrder: 1, isActive: true }],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    {
      name: 'providerId',
      label: 'Cổng thanh toán',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: paymentProviderOptions },
    },
    { name: 'iconUrl', label: 'Icon URL', type: 'url', table: true, form: { col: 6 } },
    { name: 'feePercent', label: 'Phí (%)', type: 'number', table: true, form: { col: 6 } },
    sortOrderField<PaymentMethod>(),
    isActiveField<PaymentMethod>(),
  ],
})

export const paymentCallbacksEntity = defineAdminEntity<PaymentCallback>({
  path: '/payments/callbacks',
  title: 'Callback thanh toán',
  breadcrumbSubtitle: paySub,
  description: 'Raw webhook — idempotency.',
  entityName: 'callback',
  labels: labels('callback', 'Tìm provider...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      provider: 'VNPay',
      rawPayload: '{"vnp_Amount":"9900000"}',
      signatureValid: true,
      processStatus: 'Processed',
      receivedAt: '2026-05-31 14:21:05',
      isActive: true,
    },
  ],
  fields: [
    { name: 'receivedAt', label: 'Thời gian nhận', type: 'text', table: true, form: false },
    { name: 'provider', label: 'Cổng thanh toán', type: 'text', table: true, form: false },
    { name: 'rawPayload', label: 'Raw payload', type: 'textarea', form: false, table: false },
    { name: 'processStatus', label: 'Trạng thái xử lý', type: 'text', table: true, form: false },
    { name: 'signatureValid', label: 'Chữ ký hợp lệ', type: 'checkbox', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const paymentReconcileEntity = defineAdminEntity<PaymentTransaction>({
  path: '/payments/reconcile',
  title: 'Đối soát thanh toán',
  breadcrumbSubtitle: paySub,
  description: 'Đối soát với cổng thanh toán.',
  entityName: 'giao dịch',
  labels: labels('giao dịch', 'Tìm mã...', 'Đối soát'),
  capabilities: readonlyCrudCapabilities,
  seedData: paymentTransactionsEntity.seedData,
  fields: paymentTransactionsEntity.fields,
})

export const paymentRefundsEntity = defineAdminEntity<PaymentRefund>({
  path: '/payments/refunds',
  title: 'Hoàn tiền',
  breadcrumbSubtitle: paySub,
  description: 'Tạo / duyệt hoàn tiền.',
  entityName: 'hoàn tiền',
  labels: labels('yêu cầu', 'Tìm đơn...', 'Tạo hoàn tiền'),
  seedData: [
    {
      id: 1,
      orderId: 3,
      amount: 120000,
      reason: 'Provider hết hàng',
      status: 'Pending',
      approvedBy: '',
      refundedAt: '',
      isActive: true,
    },
  ],
  fields: [
    { name: 'orderId', label: 'ID đơn hàng', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'amount', label: 'Số tiền', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'reason', label: 'Lý do', type: 'textarea', form: { required: true } },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'approvedBy', label: 'Người duyệt', type: 'text', table: true, form: { col: 6 } },
    { name: 'refundedAt', label: 'Thời gian hoàn', type: 'text', table: true, form: { col: 6 } },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const deliveriesListEntity = defineAdminEntity<DigitalDelivery>({
  path: '/deliveries/list',
  title: 'Danh sách giao hàng',
  breadcrumbSubtitle: salesSub,
  description: 'Digital delivery: eSIM QR, mã thẻ.',
  entityName: 'giao hàng',
  labels: labels('giao hàng', 'Tìm đơn...', 'Thêm'),
  capabilities: { create: false, edit: false, view: true, delete: false, toggleActive: false },
  seedData: [
    {
      id: 1,
      orderId: 1,
      deliveryType: 'ESIM_QR',
      status: 'Sent',
      sentAt: '2026-05-31 14:25',
      errorMessage: '',
      isActive: true,
    },
  ],
  fields: [
    { name: 'orderId', label: 'ID đơn hàng', type: 'number', table: { variant: 'code' }, form: false },
    { name: 'deliveryType', label: 'Loại giao hàng', type: 'text', table: true, form: false },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: false },
    { name: 'sentAt', label: 'Thời gian gửi', type: 'text', table: true, form: false },
    { name: 'errorMessage', label: 'Lỗi', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const esimProfilesEntity = defineAdminEntity<EsimProfile>({
  path: '/deliveries/esim-profiles',
  title: 'Hồ sơ eSIM / QR code',
  breadcrumbSubtitle: salesSub,
  description: 'Dữ liệu nhạy cảm — mask theo quyền.',
  entityName: 'hồ sơ eSIM',
  labels: labels('hồ sơ', 'Tìm ICCID...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      orderItemId: 1,
      iccid: '8985***********',
      qrCodeUrl: 'https://cdn.example/qr/1.png',
      activationCode: 'LPA:***',
      status: 'Active',
      isActive: true,
    },
  ],
  fields: [
    { name: 'iccid', label: 'ICCID', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'orderItemId', label: 'ID item đơn', type: 'number', table: true, form: false },
    { name: 'qrCodeUrl', label: 'URL QR code', type: 'url', table: true, form: false },
    { name: 'activationCode', label: 'Mã kích hoạt', type: 'text', table: true, form: false },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const deliveredCodesEntity = defineAdminEntity<DeliveredCode>({
  path: '/deliveries/codes',
  title: 'Mã thẻ / PIN đã giao',
  breadcrumbSubtitle: salesSub,
  description: 'Serial / PIN — mask theo quyền.',
  entityName: 'mã đã giao',
  labels: labels('mã', 'Tìm serial...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      orderItemId: 2,
      serial: 'ABC123',
      pinCode: '****1234',
      expiredAt: '2027-05-31',
      status: 'Delivered',
      isActive: true,
    },
  ],
  fields: [
    { name: 'serial', label: 'Serial', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'pinCode', label: 'PIN', type: 'text', table: true, form: false },
    { name: 'orderItemId', label: 'ID item đơn', type: 'number', table: true, form: false },
    { name: 'expiredAt', label: 'Hết hạn', type: 'text', table: true, form: false },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const deliveryRetryEntity = defineAdminEntity<DigitalDelivery>({
  path: '/deliveries/retry',
  title: 'Retry giao hàng',
  breadcrumbSubtitle: salesSub,
  description: 'Đơn giao lỗi cần retry.',
  entityName: 'giao hàng',
  labels: labels('giao hàng', 'Tìm đơn lỗi...', 'Retry'),
  capabilities: { create: false, edit: false, view: true, delete: false, toggleActive: false },
  seedData: deliveriesListEntity.seedData.map((d) => ({ ...d, status: 'Failed', errorMessage: 'SMTP timeout' })),
  fields: deliveriesListEntity.fields,
})

export const deliveryLogsEntity = defineAdminEntity<DeliveryLog>({
  path: '/deliveries/logs',
  title: 'Nhật ký giao hàng',
  breadcrumbSubtitle: salesSub,
  description: 'Log vận hành — chỉ xem.',
  entityName: 'log',
  labels: labels('log', 'Tìm action...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      deliveryId: 1,
      action: 'SendEmail',
      request: '{"to":"a@gmail.com"}',
      response: '{"error":"SMTP timeout"}',
      errorMessage: 'SMTP timeout',
      createdAt: '2026-05-31 14:26',
      isActive: true,
    },
  ],
  fields: [
    { name: 'createdAt', label: 'Thời gian', type: 'text', table: true, form: false },
    { name: 'deliveryId', label: 'ID giao hàng', type: 'number', table: true, form: false },
    { name: 'action', label: 'Hành động', type: 'text', table: true, form: false },
    { name: 'request', label: 'Request', type: 'textarea', form: false, table: false },
    { name: 'response', label: 'Response', type: 'textarea', form: false, table: false },
    { name: 'errorMessage', label: 'Lỗi', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const salesEntities = [
  ordersAllEntity,
  ordersPendingEntity,
  ordersPaidEntity,
  ordersProcessingEntity,
  ordersDeliveredEntity,
  ordersFailedEntity,
  ordersCancelledEntity,
  ordersRefundedEntity,
  paymentTransactionsEntity,
  paymentProvidersEntity,
  paymentMethodsEntity,
  paymentCallbacksEntity,
  paymentReconcileEntity,
  paymentRefundsEntity,
  deliveriesListEntity,
  esimProfilesEntity,
  deliveredCodesEntity,
  deliveryRetryEntity,
  deliveryLogsEntity,
]
