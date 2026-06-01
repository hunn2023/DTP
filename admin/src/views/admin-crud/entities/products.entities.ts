import { defineAdminEntity } from '@/views/admin-crud/schema/defineEntity'
import { labels } from '@/views/admin-crud/entities/shared'
import { isActiveField, sortOrderField, statusField } from '@/views/admin-crud/entities/fieldHelpers'
import { carriersData } from '@/views/settings/carriers/data'
import { countriesData } from '@/views/settings/countries/data'
import { denominationsData } from '@/views/settings/denominations/data'
import { activeTagOptions } from '@/views/settings/tags/tagOptions'
import type { EntityFieldDef } from '@/views/admin-crud/types'
import type { SettingsEntityBase } from '@/views/settings/types'

const sub = 'Sản phẩm bán'

const countryOptions = countriesData.map((c) => ({ value: String(c.id), label: `${c.flagEmoji} ${c.name}` }))
const carrierOptions = carriersData.map((c) => ({ value: String(c.id), label: `${c.name} (${c.countryName})` }))
const denominationOptions = denominationsData.map((d) => ({
  value: String(d.id),
  label: d.displayName,
}))

export type EsimPackage = SettingsEntityBase & {
  name: string
  code: string
  countryId: number
  carrierId: number
  dataAmount: string
  dataUnit: string
  validityDays: number
  isUnlimited: boolean
  support5G: boolean
  supportHotspot: boolean
  hasPhoneNumber: boolean
  tagIds: number[]
  status: string
}

export type ProductPrice = SettingsEntityBase & {
  productType: string
  productId: number
  currencyCode: string
  originalPrice: number
  salePrice: number
  startDate: string
  endDate: string
}

export type CardProductPrice = SettingsEntityBase & {
  productType: string
  productId: number
  originalPrice: number
  salePrice: number
  discountRate: number
  startDate: string
  endDate: string
}

export type FaqItem = SettingsEntityBase & {
  question: string
  answer: string
  targetType: string
  targetId: number
  sortOrder: number
}

export type TelecomCard = SettingsEntityBase & {
  name: string
  carrierBrand: string
  denominationId: number
  faceValue: number
  salePrice: number
  discountRate: number
  maxQuantity: number
  status: string
}

export type GameCard = SettingsEntityBase & {
  name: string
  publisherBrand: string
  denominationId: number
  faceValue: number
  salePrice: number
  discountRate: number
  maxQuantity: number
  status: string
}

export type DataPackage = SettingsEntityBase & {
  name: string
  carrierId: number
  dataAmount: string
  validityDays: number
  price: number
  activationMethod: string
  status: string
}

const priceFields: EntityFieldDef<ProductPrice>[] = [
  { name: 'productType', label: 'Loại SP', type: 'text', table: true, form: { required: true, col: 6 } },
  { name: 'productId', label: 'ProductId', type: 'number', table: true, form: { required: true, parseAsNumber: true, col: 6 } },
  { name: 'currencyCode', label: 'Tiền tệ', type: 'text', table: true, form: { required: true, col: 6 } },
  { name: 'originalPrice', label: 'Giá gốc', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'salePrice', label: 'Giá bán', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'startDate', label: 'Từ ngày', type: 'text', table: true, form: { col: 6 } },
  { name: 'endDate', label: 'Đến ngày', type: 'text', table: true, form: { col: 6 } },
  isActiveField<ProductPrice>(),
]

export const esimPackagesEntity = defineAdminEntity<EsimPackage>({
  path: '/products/esim/packages',
  title: 'Gói eSIM',
  breadcrumbSubtitle: sub,
  description: 'Quản lý gói eSIM du lịch. Liên kết Country, Carrier, giá bán và mapping provider.',
  entityName: 'gói eSIM',
  labels: labels('gói eSIM', 'Tìm tên, mã, quốc gia...', 'Thêm gói eSIM'),
  seedData: [
    {
      id: 1,
      name: 'eSIM Nhật Bản 5GB 7 ngày',
      code: 'JP_5GB_7D',
      countryId: 1,
      carrierId: 1,
      dataAmount: '5',
      dataUnit: 'GB',
      validityDays: 7,
      isUnlimited: false,
      support5G: true,
      supportHotspot: true,
      hasPhoneNumber: false,
      tagIds: [1, 3],
      status: 'active',
      isActive: true,
    },
  ],
  fields: [
    { name: 'name', label: 'Name', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'code', label: 'Code', type: 'text', table: { variant: 'code' }, form: { required: true } },
    {
      name: 'countryId',
      label: 'CountryId',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: countryOptions },
    },
    {
      name: 'carrierId',
      label: 'CarrierId',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: carrierOptions },
    },
    { name: 'dataAmount', label: 'DataAmount', type: 'text', table: true, form: { required: true, col: 6 } },
    {
      name: 'dataUnit',
      label: 'DataUnit',
      type: 'select',
      table: true,
      form: {
        col: 6,
        options: [
          { value: 'GB', label: 'GB' },
          { value: 'MB', label: 'MB' },
        ],
      },
    },
    { name: 'validityDays', label: 'ValidityDays', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'isUnlimited', label: 'IsUnlimited', type: 'checkbox', table: true, form: { col: 6 } },
    { name: 'support5G', label: 'Support5G', type: 'checkbox', table: true, form: { col: 6 } },
    { name: 'supportHotspot', label: 'SupportHotspot', type: 'checkbox', table: true, form: { col: 6 } },
    { name: 'hasPhoneNumber', label: 'HasPhoneNumber', type: 'checkbox', table: true, form: { col: 6 } },
    {
      name: 'tagIds',
      type: 'multiselect',
      table: true,
      form: { col: 12, options: activeTagOptions },
    },
    statusField<EsimPackage>(),
    isActiveField<EsimPackage>(),
  ],
})

export const esimPricesEntity = defineAdminEntity<ProductPrice>({
  path: '/products/esim/prices',
  title: 'Giá bán eSIM',
  breadcrumbSubtitle: sub,
  description: 'Bảng giá theo sản phẩm (ProductPrices).',
  entityName: 'bản ghi giá',
  labels: labels('bản ghi giá', 'Tìm ProductId...', 'Thêm giá'),
  seedData: [
    {
      id: 1,
      productType: 'EsimPackage',
      productId: 1,
      currencyCode: 'VND',
      originalPrice: 120000,
      salePrice: 99000,
      startDate: '2026-01-01',
      endDate: '2026-12-31',
      isActive: true,
    },
  ],
  fields: priceFields as EntityFieldDef<ProductPrice>[],
})

export const esimFaqEntity = defineAdminEntity<FaqItem>({
  path: '/products/esim/faq',
  title: 'FAQ eSIM',
  breadcrumbSubtitle: sub,
  description: 'FAQ gắn EsimPackages / Countries.',
  entityName: 'FAQ',
  labels: labels('FAQ', 'Tìm câu hỏi...', 'Thêm FAQ'),
  seedData: [
    {
      id: 1,
      question: 'Tôi nên cài eSIM trước hay sau khi đến Nhật?',
      answer: 'Nên cài trước khi bay, kích hoạt khi đến nơi.',
      targetType: 'Country',
      targetId: 1,
      sortOrder: 1,
      isActive: true,
    },
  ],
  fields: [
    { name: 'question', label: 'Question', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'answer', label: 'Answer', type: 'textarea', form: { required: true } },
    { name: 'targetType', label: 'TargetType', type: 'text', table: true, form: { col: 6 } },
    { name: 'targetId', label: 'TargetId', type: 'number', table: true, form: { col: 6 } },
    sortOrderField<FaqItem>(),
    isActiveField<FaqItem>(),
  ],
})

const sharedCardProductFields = [
  { name: 'name', label: 'Name', type: 'text', table: { variant: 'primary' }, form: { required: true } },
  {
    name: 'denominationId',
    label: 'DenominationId',
    type: 'select',
    table: true,
    form: { required: true, parseAsNumber: true, options: denominationOptions },
  },
  { name: 'faceValue', label: 'FaceValue', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'salePrice', label: 'SalePrice', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'discountRate', label: 'DiscountRate', type: 'number', table: true, form: { col: 6 } },
  { name: 'maxQuantity', label: 'MaxQuantity', type: 'number', table: true, form: { col: 6 } },
] as const

const telecomCardFields: EntityFieldDef<TelecomCard>[] = [
  ...sharedCardProductFields,
  { name: 'carrierBrand', label: 'CarrierBrand', type: 'text', table: true, form: { required: true } },
  statusField<TelecomCard>(),
  isActiveField<TelecomCard>(),
]

const gameCardFields: EntityFieldDef<GameCard>[] = [
  ...sharedCardProductFields,
  { name: 'publisherBrand', label: 'PublisherBrand', type: 'text', table: true, form: { required: true } },
  statusField<GameCard>(),
  isActiveField<GameCard>(),
]

export const telecomCardsEntity = defineAdminEntity<TelecomCard>({
  path: '/products/cards-data/telecom-cards',
  title: 'Thẻ viễn thông',
  breadcrumbSubtitle: sub,
  description: 'TelecomCards — digital code sau thanh toán.',
  entityName: 'thẻ viễn thông',
  labels: labels('thẻ', 'Tìm tên...', 'Thêm thẻ'),
  seedData: [
    {
      id: 1,
      name: 'Thẻ Viettel 100.000đ',
      carrierBrand: 'Viettel',
      denominationId: 4,
      faceValue: 100000,
      salePrice: 92000,
      discountRate: 8,
      maxQuantity: 10,
      status: 'active',
      isActive: true,
    },
  ],
  fields: telecomCardFields,
})

export const gameCardsEntity = defineAdminEntity<GameCard>({
  path: '/products/cards-data/game-cards',
  title: 'Thẻ game',
  breadcrumbSubtitle: sub,
  description: 'GameCards — digital code sau thanh toán.',
  entityName: 'thẻ game',
  labels: labels('thẻ game', 'Tìm tên...', 'Thêm thẻ game'),
  seedData: [
    {
      id: 1,
      name: 'Garena 100.000đ',
      publisherBrand: 'Garena',
      denominationId: 4,
      faceValue: 100000,
      salePrice: 90000,
      discountRate: 10,
      maxQuantity: 5,
      status: 'active',
      isActive: true,
    },
  ],
  fields: gameCardFields,
})

export const dataPackagesEntity = defineAdminEntity<DataPackage>({
  path: '/products/cards-data/data-packages',
  title: 'Data 4G/5G',
  breadcrumbSubtitle: sub,
  description: 'DataPackages — gói data trong nước.',
  entityName: 'gói data',
  labels: labels('gói data', 'Tìm tên...', 'Thêm gói data'),
  seedData: [
    {
      id: 1,
      name: 'Viettel 5GB/ngày',
      carrierId: 4,
      dataAmount: '5GB/ngày',
      validityDays: 7,
      price: 150000,
      activationMethod: 'USSD',
      status: 'active',
      isActive: true,
    },
  ],
  fields: [
    { name: 'name', label: 'Name', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    {
      name: 'carrierId',
      label: 'CarrierId',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: carrierOptions },
    },
    { name: 'dataAmount', label: 'DataAmount', type: 'text', table: true, form: { required: true } },
    { name: 'validityDays', label: 'ValidityDays', type: 'number', table: true, form: { col: 6 } },
    { name: 'price', label: 'Price', type: 'number', table: true, form: { col: 6 } },
    { name: 'activationMethod', label: 'ActivationMethod', type: 'text', table: true, form: { required: true } },
    statusField<DataPackage>(),
    isActiveField<DataPackage>(),
  ],
})

export const cardPricesEntity = defineAdminEntity<CardProductPrice>({
  path: '/products/cards-data/prices',
  title: 'Giá bán / Chiết khấu',
  breadcrumbSubtitle: sub,
  description: 'ProductPrices cho thẻ & data.',
  entityName: 'bản ghi giá',
  labels: labels('bản ghi giá', 'Tìm ProductId...', 'Thêm giá'),
  seedData: [
    {
      id: 1,
      productType: 'TelecomCard',
      productId: 1,
      originalPrice: 100000,
      salePrice: 92000,
      discountRate: 8,
      startDate: '2026-01-01',
      endDate: '',
      isActive: true,
    },
  ],
  fields: [
    { name: 'productType', label: 'ProductType', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'productId', label: 'ProductId', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'originalPrice', label: 'OriginalPrice', type: 'number', table: true, form: { col: 6 } },
    { name: 'salePrice', label: 'SalePrice', type: 'number', table: true, form: { col: 6 } },
    { name: 'discountRate', label: 'DiscountRate', type: 'number', table: true, form: { col: 6 } },
    { name: 'startDate', label: 'StartDate', type: 'text', table: true, form: { col: 6 } },
    { name: 'endDate', label: 'EndDate', type: 'text', table: true, form: { col: 6 } },
    isActiveField<CardProductPrice>(),
  ],
})

export const productEntities = [
  esimPackagesEntity,
  esimPricesEntity,
  esimFaqEntity,
  telecomCardsEntity,
  gameCardsEntity,
  dataPackagesEntity,
  cardPricesEntity,
]
