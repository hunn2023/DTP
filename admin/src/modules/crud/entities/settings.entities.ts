import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { buildBrandColumns } from '@/features/master-data/brands/columns'
import { brandsData, brandsLabels } from '@/features/master-data/brands/data'
import { brandFormConfig } from '@/features/master-data/brands/formConfig'
import { buildCarrierColumns } from '@/features/master-data/carriers/columns'
import { carriersData, carriersLabels } from '@/features/master-data/carriers/data'
import { carrierFormConfig } from '@/features/master-data/carriers/formConfig'
import { buildCategoryColumns } from '@/features/master-data/categories/columns'
import { categoriesData, categoriesLabels } from '@/features/master-data/categories/data'
import { categoryFormConfig } from '@/features/master-data/categories/formConfig'
import { buildCountryColumns } from '@/features/master-data/countries/columns'
import { countriesData, countriesLabels } from '@/features/master-data/countries/data'
import { countryFormConfig } from '@/features/master-data/countries/formConfig'
import { buildDenominationColumns } from '@/features/master-data/denominations/columns'
import { denominationsData, denominationsLabels } from '@/features/master-data/denominations/data'
import { denominationFormConfig } from '@/features/master-data/denominations/formConfig'
import { buildSupportedDeviceColumns } from '@/features/master-data/supported-devices/columns'
import { supportedDevicesData, supportedDevicesLabels } from '@/features/master-data/supported-devices/data'
import { supportedDeviceFormConfig } from '@/features/master-data/supported-devices/formConfig'
import { buildTagColumns } from '@/features/master-data/tags/columns'
import { tagsData, tagsLabels } from '@/features/master-data/tags/data'
import { tagFormConfig } from '@/features/master-data/tags/formConfig'
import type {
  Brand,
  Carrier,
  Category,
  Country,
  Denomination,
  SupportedDevice,
  Tag,
} from '@/features/master-data/types'

const sub = 'Cấu hình hệ thống'

export const settingsCategoriesEntity = defineAdminEntity<Category>({
  path: '/settings/categories',
  title: 'Danh mục',
  breadcrumbSubtitle: sub,
  description:
    'Phân loại sản phẩm chính: eSIM, thẻ game, thẻ viễn thông, data. Dùng cho Products / EsimPackages / GameCards.',
  entityName: 'danh mục',
  labels: categoriesLabels,
  seedData: categoriesData,
  fields: [],
  buildColumns: buildCategoryColumns,
  formConfig: categoryFormConfig,
})

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

export const settingsCountriesEntity = defineAdminEntity<Country>({
  path: '/settings/countries',
  title: 'Quốc gia',
  breadcrumbSubtitle: sub,
  description:
    'Danh mục quốc gia cho eSIM và nhà mạng. RegionCode dùng enum (ASIA, EU, ...) — không cần bảng Regions riêng ở MVP.',
  entityName: 'quốc gia',
  labels: countriesLabels,
  seedData: countriesData,
  fields: [],
  buildColumns: buildCountryColumns,
  formConfig: countryFormConfig,
})

export const settingsCarriersEntity = defineAdminEntity<Carrier>({
  path: '/settings/carriers',
  title: 'Nhà mạng',
  breadcrumbSubtitle: sub,
  description: 'Hiển thị, lọc và SEO — không phải provider API. Liên kết CountryId.',
  entityName: 'nhà mạng',
  labels: carriersLabels,
  seedData: carriersData,
  fields: [],
  buildColumns: buildCarrierColumns,
  formConfig: carrierFormConfig,
})

export const settingsDenominationsEntity = defineAdminEntity<Denomination>({
  path: '/settings/denominations',
  title: 'Mệnh giá',
  breadcrumbSubtitle: sub,
  description: 'Dùng cho thẻ game và thẻ viễn thông (TelecomCards / GameCards).',
  entityName: 'mệnh giá',
  labels: denominationsLabels,
  seedData: denominationsData,
  fields: [],
  buildColumns: buildDenominationColumns,
  formConfig: denominationFormConfig,
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
  settingsCategoriesEntity,
  settingsBrandsEntity,
  settingsTagsEntity,
  settingsCountriesEntity,
  settingsCarriersEntity,
  settingsDenominationsEntity,
  settingsSupportedDevicesEntity,
]
