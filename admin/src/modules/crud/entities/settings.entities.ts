import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { buildBrandColumns } from '@/features/master-data/brands/columns'
import { brandsData, brandsLabels } from '@/features/master-data/brands/data'
import { brandFormConfig } from '@/features/master-data/brands/formConfig'
import { buildSupportedDeviceColumns } from '@/features/master-data/supported-devices/columns'
import { supportedDevicesData, supportedDevicesLabels } from '@/features/master-data/supported-devices/data'
import { supportedDeviceFormConfig } from '@/features/master-data/supported-devices/formConfig'
import { buildTagColumns } from '@/features/master-data/tags/columns'
import { tagsData, tagsLabels } from '@/features/master-data/tags/data'
import { tagFormConfig } from '@/features/master-data/tags/formConfig'
import type {
  Brand,
  SupportedDevice,
  Tag,
} from '@/features/master-data/types'

const sub = 'Cấu hình hệ thống'

export const settingsBrandsEntity = defineAdminEntity<Brand>({
  path: '/settings/brands',
  title: 'Brand / Nhà phát hành',
  breadcrumbSubtitle: sub,
  description: 'Nhà phát hành, nhà cung cấp thương hiệu. Products và Carriers có thể tham chiếu BrandId.',
  entityName: 'brand',
  labels: brandsLabels,
  seedData: brandsData,
  fields: [],
  buildColumns: buildBrandColumns,
  formConfig: brandFormConfig,
})

export const settingsTagsEntity = defineAdminEntity<Tag>({
  path: '/settings/tags',
  title: 'Tag',
  breadcrumbSubtitle: sub,
  description: 'Nhãn gắn sản phẩm qua bảng ProductTags. Có thể hardcode ở MVP nếu chưa cần CRUD đầy đủ.',
  entityName: 'tag',
  labels: tagsLabels,
  seedData: tagsData,
  fields: [],
  buildColumns: buildTagColumns,
  formConfig: tagFormConfig,
})

export const settingsSupportedDevicesEntity = defineAdminEntity<SupportedDevice>({
  path: '/settings/supported-devices',
  title: 'Thiết bị hỗ trợ',
  breadcrumbSubtitle: sub,
  description: 'Danh sách thiết bị hỗ trợ eSIM. Phase 2: Import Excel. MVP có thể dùng trang hướng dẫn tĩnh.',
  entityName: 'thiết bị',
  labels: supportedDevicesLabels,
  seedData: supportedDevicesData,
  fields: [],
  buildColumns: buildSupportedDeviceColumns,
  formConfig: supportedDeviceFormConfig,
})

export const settingsEntities = [
  settingsBrandsEntity,
  settingsTagsEntity,
  settingsSupportedDevicesEntity,
]
