import { defineAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import { labels } from '@/views/admin-crud/entities/shared'
import { readonlyCrudCapabilities } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'

const mktSub = 'Marketing'

export type Promotion = SettingsEntityBase & {
  name: string
  discountType: string
  discountValue: number
  startDate: string
  endDate: string
  status: string
}

export type Coupon = SettingsEntityBase & {
  code: string
  discountType: string
  discountValue: number
  maxUsage: number
  usedCount: number
  expiredAt: string
}

export type EmailTemplate = SettingsEntityBase & {
  code: string
  subject: string
  bodyHtml: string
  variables: string
}

export type EmailQueueItem = SettingsEntityBase & {
  templateCode: string
  toEmail: string
  status: string
  retryCount: number
  errorMessage: string
  scheduledAt: string
}

export const promotionsEntity = defineAdminEntity<Promotion>({
  path: '/marketing/promotions',
  title: 'Khuyến mãi',
  breadcrumbSubtitle: mktSub,
  description: 'Phase 2 — sau MVP.',
  entityName: 'khuyến mãi',
  labels: labels('CTKM', 'Tìm tên...', 'Thêm CTKM'),
  seedData: [
    {
      id: 1,
      name: 'Sale hè eSIM Nhật',
      discountType: 'Percent',
      discountValue: 10,
      startDate: '2026-06-01',
      endDate: '2026-06-30',
      status: 'Active',
      isActive: true,
    },
  ],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'discountType', label: 'Loại giảm', type: 'text', form: { col: 6 } },
    { name: 'discountValue', label: 'Giá trị', type: 'number', form: { col: 6 } },
    { name: 'startDate', label: 'Từ', type: 'text', form: { col: 6 } },
    { name: 'endDate', label: 'Đến', type: 'text', form: { col: 6 } },
    { name: 'status', label: 'TT', type: 'text', table: true },
    { name: 'isActive', label: 'Bật', type: 'checkbox', form: { col: 12 } },
  ],
})

export const couponsEntity = defineAdminEntity<Coupon>({
  path: '/marketing/coupons',
  title: 'Mã giảm giá',
  breadcrumbSubtitle: mktSub,
  description: 'Phase 2.',
  entityName: 'coupon',
  labels: labels('coupon', 'Tìm mã...', 'Tạo coupon'),
  seedData: [
    {
      id: 1,
      code: 'WELCOME10',
      discountType: 'Percent',
      discountValue: 10,
      maxUsage: 1000,
      usedCount: 120,
      expiredAt: '2026-12-31',
      isActive: true,
    },
  ],
  fields: [
    { name: 'code', label: 'Mã', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'discountType', label: 'Loại', type: 'text', form: { col: 6 } },
    { name: 'discountValue', label: 'Giá trị', type: 'number', form: { col: 6 } },
    { name: 'maxUsage', label: 'Max lượt', type: 'number', form: { col: 6 } },
    { name: 'usedCount', label: 'Đã dùng', type: 'number', table: true, form: false },
    { name: 'expiredAt', label: 'Hết hạn', type: 'text', table: true, form: { col: 6 } },
    { name: 'isActive', label: 'Bật', type: 'checkbox', form: { col: 12 } },
  ],
})

export const flashSaleEntity = defineAdminEntity<Promotion>({
  path: '/marketing/flash-sale',
  title: 'Flash sale',
  breadcrumbSubtitle: mktSub,
  description: 'Chương trình flash sale.',
  entityName: 'flash sale',
  labels: labels('chương trình', 'Tìm...', 'Thêm'),
  seedData: promotionsEntity.seedData,
  fields: promotionsEntity.fields,
})

export const featuredProductsEntity = defineAdminEntity<SettingsEntityBase & { productCode: string; sortOrder: number }>({
  path: '/marketing/featured-products',
  title: 'Sản phẩm nổi bật',
  breadcrumbSubtitle: mktSub,
  description: 'Gắn SP hiển thị trang chủ.',
  entityName: 'sản phẩm',
  labels: labels('sản phẩm', 'Tìm mã SP...', 'Thêm'),
  seedData: [{ id: 1, productCode: 'JP_5GB_7D', sortOrder: 1, isActive: true }],
  fields: [
    { name: 'productCode', label: 'Mã SP', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', form: { col: 6 } },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', form: { col: 12 } },
  ],
})

export const bestSellerProductsEntity = defineAdminEntity<SettingsEntityBase & { productCode: string; soldCount: number }>({
  path: '/marketing/best-seller-products',
  title: 'Sản phẩm bán chạy',
  breadcrumbSubtitle: mktSub,
  description: 'Top bán chạy (manual hoặc auto).',
  entityName: 'sản phẩm',
  labels: labels('sản phẩm', 'Tìm mã...', 'Thêm'),
  seedData: [{ id: 1, productCode: 'JP_5GB_7D', soldCount: 520, isActive: true }],
  fields: [
    { name: 'productCode', label: 'Mã SP', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'soldCount', label: 'Đã bán', type: 'number', table: true, form: { col: 6 } },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', form: { col: 12 } },
  ],
})

export const emailTemplatesEntity = defineAdminEntity<EmailTemplate>({
  path: '/marketing/email-templates',
  title: 'Mẫu email',
  breadcrumbSubtitle: mktSub,
  description: 'Template gửi QR/PIN — MVP cần SEND_ESIM_QR.',
  entityName: 'mẫu email',
  labels: labels('mẫu', 'Tìm code...', 'Thêm mẫu'),
  seedData: [
    {
      id: 1,
      code: 'SEND_ESIM_QR',
      subject: 'Mã eSIM của bạn',
      bodyHtml: '<p>Xin chào {{name}}, QR: {{qrUrl}}</p>',
      variables: 'name,qrUrl',
      isActive: true,
    },
  ],
  fields: [
    { name: 'code', label: 'Code', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'subject', label: 'Subject', type: 'text', form: { required: true } },
    { name: 'bodyHtml', label: 'BodyHtml', type: 'textarea', form: { required: true } },
    { name: 'variables', label: 'Variables', type: 'text', form: { placeholder: 'name,qrUrl' } },
    { name: 'isActive', label: 'IsActive', type: 'checkbox', form: { col: 12 } },
  ],
})

export const emailQueueEntity = defineAdminEntity<EmailQueueItem>({
  path: '/marketing/email-queue',
  title: 'Hàng đợi email',
  breadcrumbSubtitle: mktSub,
  description: 'Retry / xem lỗi — không sửa nội dung tùy tiện.',
  entityName: 'email',
  labels: labels('email', 'Tìm email...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      templateCode: 'SEND_ESIM_QR',
      toEmail: 'a@gmail.com',
      status: 'Failed',
      retryCount: 2,
      errorMessage: 'SMTP timeout',
      scheduledAt: '2026-05-31 14:25',
      isActive: true,
    },
  ],
  fields: [
    { name: 'toEmail', label: 'ToEmail', type: 'text', table: { variant: 'primary' }, form: false },
    { name: 'templateCode', label: 'TemplateCode', type: 'text', table: true, form: false },
    { name: 'status', label: 'Status', type: 'text', table: true, form: false },
    { name: 'retryCount', label: 'RetryCount', type: 'number', table: true, form: false },
    { name: 'scheduledAt', label: 'ScheduledAt', type: 'text', table: true, form: false },
    { name: 'errorMessage', label: 'ErrorMessage', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

const sysSub = 'Quản trị hệ thống'

export type AdminUser = SettingsEntityBase & {
  fullName: string
  email: string
  phone: string
  status: string
  lastLoginAt: string
}

export type Role = SettingsEntityBase & {
  name: string
  code: string
  description: string
}

export type Permission = SettingsEntityBase & {
  code: string
  name: string
  module: string
  action: string
}

export type AuditLog = SettingsEntityBase & {
  userId: number
  userEmail: string
  entityName: string
  entityId: number
  action: string
  oldValue: string
  newValue: string
  createdAt: string
}

export type SystemSetting = SettingsEntityBase & {
  key: string
  value: string
  group: string
  description: string
  isEncrypted: boolean
}

export const adminUsersEntity = defineAdminEntity<AdminUser>({
  path: '/system/admin-users',
  title: 'Tài khoản quản trị',
  breadcrumbSubtitle: sysSub,
  description: 'Không lưu password plain text.',
  entityName: 'tài khoản',
  labels: labels('tài khoản', 'Tìm email...', 'Thêm admin'),
  seedData: [
    {
      id: 1,
      fullName: 'CSKH Staff',
      email: 'staff@ezsim.vn',
      phone: '0900000000',
      status: 'Active',
      lastLoginAt: '2026-06-01 08:00',
      isActive: true,
    },
  ],
  fields: [
    { name: 'fullName', label: 'Họ tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'email', label: 'Email', type: 'text', table: true, form: { required: true } },
    { name: 'phone', label: 'SĐT', type: 'text', form: { col: 6 } },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: { col: 6 } },
    { name: 'lastLoginAt', label: 'Đăng nhập cuối', type: 'text', table: true, form: false },
    { name: 'isActive', label: 'Hoạt động', type: 'checkbox', form: { col: 12 } },
  ],
})

export const rolesEntity = defineAdminEntity<Role>({
  path: '/system/roles',
  title: 'Vai trò',
  breadcrumbSubtitle: sysSub,
  description: 'MVP có thể role cố định.',
  entityName: 'vai trò',
  labels: labels('vai trò', 'Tìm tên...', 'Thêm vai trò'),
  seedData: [
    { id: 1, name: 'SuperAdmin', code: 'SUPER_ADMIN', description: 'Full quyền', isActive: true },
    { id: 2, name: 'CSKH', code: 'CSKH', description: 'Hỗ trợ đơn', isActive: true },
  ],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'code', label: 'Mã', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'description', label: 'Mô tả', type: 'textarea' },
    { name: 'isActive', label: 'Bật', type: 'checkbox', form: { col: 12 } },
  ],
})

export const permissionsEntity = defineAdminEntity<Permission>({
  path: '/system/permissions',
  title: 'Phân quyền',
  breadcrumbSubtitle: sysSub,
  description: 'Phase 2 khi nhiều nhân viên.',
  entityName: 'quyền',
  labels: labels('quyền', 'Tìm code...', 'Thêm quyền'),
  seedData: [
    { id: 1, code: 'orders.view', name: 'Xem đơn', module: 'orders', action: 'view', isActive: true },
    { id: 2, code: 'orders.refund', name: 'Hoàn tiền', module: 'orders', action: 'refund', isActive: true },
  ],
  fields: [
    { name: 'code', label: 'Code', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'module', label: 'Module', type: 'text', table: true, form: { col: 6 } },
    { name: 'action', label: 'Action', type: 'text', table: true, form: { col: 6 } },
    { name: 'isActive', label: 'Bật', type: 'checkbox', form: { col: 12 } },
  ],
})

export const loginHistoryEntity = defineAdminEntity<AuditLog>({
  path: '/system/login-history',
  title: 'Lịch sử đăng nhập',
  breadcrumbSubtitle: sysSub,
  description: 'Log đăng nhập admin.',
  entityName: 'bản ghi',
  labels: labels('bản ghi', 'Tìm user...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      userId: 1,
      userEmail: 'staff@ezsim.vn',
      entityName: 'Auth',
      entityId: 0,
      action: 'LoginSuccess',
      oldValue: '',
      newValue: '',
      createdAt: '2026-06-01 08:00',
      isActive: true,
    },
  ],
  fields: [
    { name: 'createdAt', label: 'CreatedAt', type: 'text', table: true, form: false },
    { name: 'userId', label: 'UserId', type: 'number', table: true, form: false },
    { name: 'userEmail', label: 'User', type: 'text', table: { variant: 'primary' }, form: false },
    { name: 'entityName', label: 'EntityName', type: 'text', table: true, form: false },
    { name: 'entityId', label: 'EntityId', type: 'number', table: true, form: false },
    { name: 'action', label: 'Action', type: 'text', table: true, form: false },
    { name: 'oldValue', label: 'OldValue', type: 'text', table: false, form: false },
    { name: 'newValue', label: 'NewValue', type: 'text', table: false, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const auditLogsEntity = defineAdminEntity<AuditLog>({
  path: '/system/audit-logs',
  title: 'Nhật ký thao tác',
  breadcrumbSubtitle: sysSub,
  description: 'Không sửa/xóa audit log.',
  entityName: 'log',
  labels: labels('log', 'Tìm user, entity...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      userId: 1,
      userEmail: 'staff@ezsim.vn',
      entityName: 'EsimPackage',
      entityId: 1,
      action: 'Update',
      oldValue: '{"salePrice":120000}',
      newValue: '{"salePrice":99000}',
      createdAt: '2026-06-01 09:00',
      isActive: true,
    },
  ],
  fields: [
    { name: 'createdAt', label: 'CreatedAt', type: 'text', table: true, form: false },
    { name: 'userId', label: 'UserId', type: 'number', table: true, form: false },
    { name: 'userEmail', label: 'User', type: 'text', table: true, form: false },
    { name: 'entityName', label: 'EntityName', type: 'text', table: true, form: false },
    { name: 'entityId', label: 'EntityId', type: 'number', table: true, form: false },
    { name: 'action', label: 'Action', type: 'text', table: true, form: false },
    { name: 'oldValue', label: 'OldValue', type: 'text', table: true, form: false },
    { name: 'newValue', label: 'NewValue', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const systemSettingsEntity = defineAdminEntity<SystemSetting>({
  path: '/system/settings',
  title: 'Cấu hình hệ thống',
  breadcrumbSubtitle: sysSub,
  description: 'Key-value — secret mã hóa.',
  entityName: 'cấu hình',
  labels: labels('cấu hình', 'Tìm key...', 'Thêm key'),
  seedData: [
    {
      id: 1,
      key: 'SUPPORT_PHONE',
      value: '19001881',
      group: 'Contact',
      description: 'Hotline hỗ trợ',
      isEncrypted: false,
      isActive: true,
    },
  ],
  fields: [
    { name: 'key', label: 'Key', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'value', label: 'Value', type: 'text', table: true, form: { required: true } },
    { name: 'group', label: 'Group', type: 'text', table: true, form: { col: 6 } },
    { name: 'description', label: 'Description', type: 'textarea' },
    { name: 'isEncrypted', label: 'IsEncrypted', type: 'checkbox', table: true, form: { col: 6 } },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const marketingEntities = [
  promotionsEntity,
  couponsEntity,
  flashSaleEntity,
  featuredProductsEntity,
  bestSellerProductsEntity,
  emailTemplatesEntity,
  emailQueueEntity,
]

export const systemEntities = [
  adminUsersEntity,
  rolesEntity,
  permissionsEntity,
  loginHistoryEntity,
  auditLogsEntity,
  systemSettingsEntity,
]
