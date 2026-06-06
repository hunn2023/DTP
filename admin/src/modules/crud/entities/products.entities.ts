import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { labels } from '@/modules/crud/entities/shared'
import { isActiveField, sortOrderField, statusField } from '@/modules/crud/entities/fieldHelpers'
import { carriersData } from '@/features/master-data/carriers/data'
import { denominationsData } from '@/features/master-data/denominations/data'
import type { EntityFieldDef } from '@/modules/crud/types'
import type { SettingsEntityBase } from '@/modules/crud/types'

const sub = 'Sản phẩm bán'

const carrierOptions = carriersData.map((c) => ({ value: String(c.id), label: c.name }))
const denominationOptions = denominationsData.map((d) => ({
  value: String(d.id),
  label: d.displayName,
}))

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
  { name: 'productId', label: 'ID sản phẩm', type: 'number', table: true, form: { required: true, parseAsNumber: true, col: 6 } },
  { name: 'currencyCode', label: 'Tiền tệ', type: 'text', table: true, form: { required: true, col: 6 } },
  { name: 'originalPrice', label: 'Giá gốc', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'salePrice', label: 'Giá bán', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'startDate', label: 'Từ ngày', type: 'text', table: true, form: { col: 6 } },
  { name: 'endDate', label: 'Đến ngày', type: 'text', table: true, form: { col: 6 } },
  isActiveField<ProductPrice>(),
]

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
    { name: 'question', label: 'Câu hỏi', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'answer', label: 'Trả lời', type: 'textarea', form: { required: true } },
    { name: 'targetType', label: 'Loại đối tượng', type: 'text', table: true, form: { col: 6 } },
    { name: 'targetId', label: 'ID đối tượng', type: 'number', table: true, form: { col: 6 } },
    sortOrderField<FaqItem>(),
    isActiveField<FaqItem>(),
  ],
})

const sharedCardProductFields = [
  { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
  {
    name: 'denominationId',
    label: 'Mệnh giá',
    type: 'select',
    table: true,
    form: { required: true, parseAsNumber: true, options: denominationOptions },
  },
  { name: 'faceValue', label: 'Giá trị', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'salePrice', label: 'Giá bán', type: 'number', table: true, form: { required: true, col: 6 } },
  { name: 'discountRate', label: 'Chiết khấu (%)', type: 'number', table: true, form: { col: 6 } },
  { name: 'maxQuantity', label: 'Số lượng tối đa', type: 'number', table: true, form: { col: 6 } },
] as const

const gameCardFields: EntityFieldDef<GameCard>[] = [
  ...sharedCardProductFields,
  { name: 'publisherBrand', label: 'Nhà phát hành', type: 'text', table: true, form: { required: true } },
  statusField<GameCard>(),
  isActiveField<GameCard>(),
]

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
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    {
      name: 'carrierId',
      label: 'Nhà mạng',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: carrierOptions },
    },
    { name: 'dataAmount', label: 'Dung lượng', type: 'text', table: true, form: { required: true } },
    { name: 'validityDays', label: 'Số ngày', type: 'number', table: true, form: { col: 6 } },
    { name: 'price', label: 'Giá', type: 'number', table: true, form: { col: 6 } },
    { name: 'activationMethod', label: 'Cách kích hoạt', type: 'text', table: true, form: { required: true } },
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
    { name: 'productType', label: 'Loại sản phẩm', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'productId', label: 'ID sản phẩm', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'originalPrice', label: 'Giá gốc', type: 'number', table: true, form: { col: 6 } },
    { name: 'salePrice', label: 'Giá bán', type: 'number', table: true, form: { col: 6 } },
    { name: 'discountRate', label: 'Chiết khấu (%)', type: 'number', table: true, form: { col: 6 } },
    { name: 'startDate', label: 'Từ ngày', type: 'text', table: true, form: { col: 6 } },
    { name: 'endDate', label: 'Đến ngày', type: 'text', table: true, form: { col: 6 } },
    isActiveField<CardProductPrice>(),
  ],
})

export const productEntities = [
  esimPricesEntity,
  esimFaqEntity,
  gameCardsEntity,
  dataPackagesEntity,
  cardPricesEntity,
]
