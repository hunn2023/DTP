import { defineAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import { labels } from '@/views/admin-crud/entities/shared'
import { isActiveField, providerStatusMap, statusOptions } from '@/views/admin-crud/entities/fieldHelpers'
import { readonlyCrudCapabilities } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'

const sub = 'Nguồn hàng & Provider'

const providerOptions = [{ value: '1', label: 'Airalo (AIRALO)' }]

export type Provider = SettingsEntityBase & {
  name: string
  code: string
  providerType: string
  priority: number
  balance: string
  status: string
}

export type ProviderProduct = SettingsEntityBase & {
  providerId: number
  providerProductCode: string
  providerProductName: string
  costPrice: number
  currencyCode: string
  stockStatus: string
}

export type ProductMapping = SettingsEntityBase & {
  localProductType: string
  localProductId: number
  providerId: number
  providerProductId: number
  priority: number
  isBackup: boolean
}

export type ProviderApiConfig = SettingsEntityBase & {
  providerId: number
  baseUrl: string
  apiKey: string
  secretKey: string
  timeoutSeconds: number
  sandboxMode: boolean
}

export type ProviderStockRow = SettingsEntityBase & {
  providerId: number
  providerProductCode: string
  available: boolean
  costPrice: number
  checkedAt: string
}

export type ProviderLog = SettingsEntityBase & {
  providerId: number
  orderId: string
  endpoint: string
  requestBody: string
  responseBody: string
  statusCode: number
  errorMessage: string
  createdAt: string
}

export const providersListEntity = defineAdminEntity<Provider>({
  path: '/providers/list',
  title: 'Danh sách nhà cung cấp',
  breadcrumbSubtitle: sub,
  description: 'Provider là nguồn hàng/API — không phải carrier hiển thị.',
  entityName: 'provider',
  labels: labels('provider', 'Tìm tên, mã...', 'Thêm provider'),
  seedData: [
    {
      id: 1,
      name: 'Airalo',
      code: 'AIRALO',
      providerType: 'ESIM',
      priority: 1,
      balance: '1,250 USD',
      status: 'active',
      isActive: true,
    },
  ],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'code', label: 'Mã', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'providerType', label: 'Loại provider', type: 'text', table: true, form: { required: true } },
    { name: 'priority', label: 'Ưu tiên', type: 'number', table: true, form: { col: 6 } },
    { name: 'balance', label: 'Số dư', type: 'text', table: true, form: { col: 6 } },
    {
      name: 'status',
      label: 'Trạng thái',
      type: 'select',
      table: { variant: 'badge', badgeMap: providerStatusMap },
      form: { options: statusOptions(providerStatusMap) },
    },
    isActiveField<Provider>(),
  ],
})

export const providerProductsEntity = defineAdminEntity<ProviderProduct>({
  path: '/providers/products',
  title: 'Sản phẩm provider',
  breadcrumbSubtitle: sub,
  description: 'Sản phẩm thô đồng bộ từ API provider.',
  entityName: 'sản phẩm provider',
  labels: labels('sản phẩm', 'Tìm mã provider...', 'Thêm sản phẩm'),
  seedData: [
    {
      id: 1,
      providerId: 1,
      providerProductCode: 'JP_5GB_7D',
      providerProductName: 'Japan 5GB 7 Days',
      costPrice: 3.2,
      currencyCode: 'USD',
      stockStatus: 'in_stock',
      isActive: true,
    },
  ],
  fields: [
    {
      name: 'providerId',
      label: 'Nhà cung cấp',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: providerOptions },
    },
    { name: 'providerProductCode', label: 'Mã SP provider', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'providerProductName', label: 'Tên SP provider', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'costPrice', label: 'Giá vốn', type: 'number', table: true, form: { col: 6 } },
    { name: 'currencyCode', label: 'Tiền tệ', type: 'text', table: true, form: { col: 6 } },
    { name: 'stockStatus', label: 'Tình trạng kho', type: 'text', table: true, form: { required: true } },
    isActiveField<ProviderProduct>(),
  ],
})

export const providerMappingEntity = defineAdminEntity<ProductMapping>({
  path: '/providers/mapping',
  title: 'Mapping sản phẩm',
  breadcrumbSubtitle: sub,
  description: 'Map sản phẩm local ↔ provider.',
  entityName: 'mapping',
  labels: labels('mapping', 'Tìm LocalProductId...', 'Thêm mapping'),
  seedData: [
    {
      id: 1,
      localProductType: 'EsimPackage',
      localProductId: 1,
      providerId: 1,
      providerProductId: 1,
      priority: 1,
      isBackup: false,
      isActive: true,
    },
  ],
  fields: [
    { name: 'localProductType', label: 'Loại SP local', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'localProductId', label: 'ID SP local', type: 'number', table: true, form: { required: true, col: 6 } },
    {
      name: 'providerId',
      label: 'Nhà cung cấp',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: providerOptions },
    },
    { name: 'providerProductId', label: 'ID SP provider', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'priority', label: 'Ưu tiên', type: 'number', table: true, form: { col: 6 } },
    { name: 'isBackup', label: 'Dự phòng', type: 'checkbox', table: true, form: { col: 6 } },
    isActiveField<ProductMapping>(),
  ],
})

export const providerApiConfigsEntity = defineAdminEntity<ProviderApiConfig>({
  path: '/providers/api-configs',
  title: 'Cấu hình API',
  breadcrumbSubtitle: sub,
  description: 'Secret key mã hóa — UI mask, không show full.',
  entityName: 'cấu hình API',
  labels: labels('cấu hình', 'Tìm ProviderId...', 'Thêm cấu hình'),
  seedData: [
    {
      id: 1,
      providerId: 1,
      baseUrl: 'https://api.airalo.com',
      apiKey: '••••••••',
      secretKey: '••••••••',
      timeoutSeconds: 30,
      sandboxMode: false,
      isActive: true,
    },
  ],
  fields: [
    {
      name: 'providerId',
      label: 'Nhà cung cấp',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: providerOptions },
    },
    { name: 'baseUrl', label: 'Base URL', type: 'url', table: true, form: { required: true } },
    { name: 'apiKey', label: 'API Key', type: 'password', form: { required: true } },
    { name: 'secretKey', label: 'Secret Key', type: 'password', form: { required: true } },
    { name: 'timeoutSeconds', label: 'Timeout (giây)', type: 'number', table: true, form: { col: 6 } },
    { name: 'sandboxMode', label: 'Sandbox', type: 'checkbox', table: true, form: { col: 6 } },
    isActiveField<ProviderApiConfig>(),
  ],
})

export const providerStocksEntity = defineAdminEntity<ProviderStockRow>({
  path: '/providers/stocks',
  title: 'Kiểm tra tồn kho',
  breadcrumbSubtitle: sub,
  description: 'Gọi API provider realtime.',
  entityName: 'kết quả check',
  labels: labels('bản ghi', 'Tìm mã SP provider...', 'Check tồn kho'),
  capabilities: { ...readonlyCrudCapabilities, create: true },
  seedData: [
    {
      id: 1,
      providerId: 1,
      providerProductCode: 'JP_5GB_7D',
      available: true,
      costPrice: 3.2,
      checkedAt: '2026-06-01 10:30',
      isActive: true,
    },
  ],
  fields: [
    {
      name: 'providerId',
      label: 'Nhà cung cấp',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: providerOptions },
    },
    { name: 'providerProductCode', label: 'Mã SP provider', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'available', label: 'Còn hàng', type: 'checkbox', table: true },
    { name: 'costPrice', label: 'Giá vốn', type: 'number', table: true },
    { name: 'checkedAt', label: 'Thời điểm kiểm tra', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const providerLogsEntity = defineAdminEntity<ProviderLog>({
  path: '/providers/logs',
  title: 'Log provider',
  breadcrumbSubtitle: sub,
  description: 'Chỉ xem — không sửa/xóa log.',
  entityName: 'log',
  labels: labels('log', 'Tìm endpoint, lỗi...', 'Thêm log'),
  capabilities: readonlyCrudCapabilities,
  seedData: [
    {
      id: 1,
      providerId: 1,
      orderId: 'EZ202605310001',
      endpoint: 'POST /order',
      requestBody: '{"package":"JP_5GB_7D"}',
      responseBody: '{"error":"timeout"}',
      statusCode: 500,
      errorMessage: 'timeout',
      createdAt: '2026-06-01 09:15',
      isActive: true,
    },
  ],
  fields: [
    { name: 'createdAt', label: 'Thời gian', type: 'text', table: true, form: false },
    { name: 'providerId', label: 'ID nhà cung cấp', type: 'number', table: true, form: false },
    { name: 'orderId', label: 'ID đơn hàng', type: 'text', table: { variant: 'code' }, form: false },
    { name: 'endpoint', label: 'Endpoint', type: 'text', table: true, form: false },
    { name: 'requestBody', label: 'Request body', type: 'textarea', form: false, table: false },
    { name: 'responseBody', label: 'Response body', type: 'textarea', form: false, table: false },
    { name: 'statusCode', label: 'Mã trạng thái', type: 'number', table: true, form: false },
    { name: 'errorMessage', label: 'Lỗi', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const providerEntities = [
  providersListEntity,
  providerProductsEntity,
  providerMappingEntity,
  providerApiConfigsEntity,
  providerStocksEntity,
  providerLogsEntity,
]
