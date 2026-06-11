import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { labels } from '@/modules/crud/entities/shared'
import { readonlyCrudCapabilities } from '@/modules/crud/types'
import type { SettingsEntityBase } from '@/modules/crud/types'

const sub = 'Khách hàng'

export type CustomerProfile = SettingsEntityBase & {
  fullName: string
  email: string
  phone: string
  status: string
  createdAt: string
}

export type CustomerOrderView = SettingsEntityBase & {
  customerId: number
  orderNo: string
  productName: string
  amount: number
  status: string
  createdAt: string
}

export type CustomerNote = SettingsEntityBase & {
  customerId: number
  note: string
  createdBy: string
  createdAt: string
}

export const customersListEntity = defineAdminEntity<CustomerProfile>({
  path: '/customers/list',
  title: 'Danh sách khách hàng',
  breadcrumbSubtitle: sub,
  description: 'MVP hỗ trợ khách guest + tài khoản.',
  entityName: 'khách hàng',
  labels: labels('khách', 'Tìm tên, email, SĐT...', 'Thêm khách'),
  seedData: [
    {
      id: 1,
      fullName: 'Nguyễn Văn A',
      email: 'a@gmail.com',
      phone: '0901234567',
      status: 'Active',
      createdAt: '2026-01-15',
      isActive: true,
    },
  ],
  fields: [
    { name: 'fullName', label: 'Họ tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'email', label: 'Email', type: 'text', table: true, form: { required: true } },
    { name: 'phone', label: 'SĐT', type: 'text', table: true, form: { required: true } },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: { required: true } },
    { name: 'createdAt', label: 'Ngày tạo', type: 'text', table: true, form: false },
    { name: 'isActive', label: 'Hoạt động', type: 'checkbox', form: { col: 12 } },
  ],
})

export const customersOrdersEntity = defineAdminEntity<CustomerOrderView>({
  path: '/customers/orders',
  title: 'Lịch sử mua hàng',
  breadcrumbSubtitle: sub,
  description: 'View tổng hợp đơn theo khách — không CRUD.',
  entityName: 'đơn',
  labels: labels('đơn', 'Tìm khách, mã đơn...', 'Thêm'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      customerId: 1,
      orderNo: 'EZ202605310001',
      productName: 'eSIM Nhật 5GB',
      amount: 99000,
      status: 'Paid',
      createdAt: '2026-05-31',
      isActive: true,
    },
  ],
  fields: [
    { name: 'orderNo', label: 'Mã đơn', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'customerId', label: 'ID khách hàng', type: 'number', table: { variant: 'primary' }, form: false },
    { name: 'productName', label: 'Sản phẩm', type: 'text', table: true, form: false },
    { name: 'amount', label: 'Số tiền', type: 'number', table: true, form: false },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: false },
    { name: 'createdAt', label: 'Ngày tạo', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const customersNotesEntity = defineAdminEntity<CustomerNote>({
  path: '/customers/notes',
  title: 'Ghi chú khách hàng',
  breadcrumbSubtitle: sub,
  description: 'CSKH — Phase 2.',
  entityName: 'ghi chú',
  labels: labels('ghi chú', 'Tìm khách...', 'Thêm ghi chú'),
  seedData: [
    {
      id: 1,
      customerId: 1,
      note: 'Khách VIP, hỗ trợ nhanh',
      createdBy: 'staff@ezsim.vn',
      createdAt: '2026-05-30',
      isActive: true,
    },
  ],
  fields: [
    { name: 'customerId', label: 'ID khách hàng', type: 'number', table: { variant: 'primary' }, form: { required: true } },
    { name: 'note', label: 'Ghi chú', type: 'textarea', form: { required: true } },
    { name: 'createdBy', label: 'Người tạo', type: 'text', form: { required: true } },
    { name: 'createdAt', label: 'Ngày tạo', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const customersBlockedEntity = defineAdminEntity<CustomerProfile>({
  path: '/customers/blocked',
  title: 'Khách hàng bị khóa',
  breadcrumbSubtitle: sub,
  description: 'Danh sách khách bị khóa / chặn.',
  entityName: 'khách',
  labels: labels('khách', 'Tìm...', 'Thêm'),
  seedData: customersListEntity.seedData.map((c) => ({ ...c, status: 'Blocked', isActive: false })),
  fields: customersListEntity.fields,
})

export const customerEntities = [
  customersListEntity,
  customersOrdersEntity,
  customersNotesEntity,
  customersBlockedEntity,
]
