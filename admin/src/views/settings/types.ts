export type SettingsEntityBase = {
  id: number
  isActive: boolean
}

export type Category = SettingsEntityBase & {
  name: string
  slug: string
  icon: string
  description: string
  sortOrder: number
}

export type Brand = SettingsEntityBase & {
  name: string
  slug: string
  logoUrl: string
  brandColor: string
  websiteUrl: string
  sortOrder: number
}

export type TagType = 'product' | 'marketing' | 'technical'

export type Tag = SettingsEntityBase & {
  name: string
  slug: string
  color: string
  icon: string
  type: TagType
  sortOrder: number
}

export type Country = SettingsEntityBase & {
  name: string
  englishName: string
  slug: string
  isoCode: string
  flagEmoji: string
  regionCode: string
  bannerUrl: string
  seoTitle: string
  seoDescription: string
  sortOrder: number
}

export type Carrier = SettingsEntityBase & {
  name: string
  slug: string
  countryId: number
  countryName: string
  logoUrl: string
  support5G: boolean
  coverageNote: string
  sortOrder: number
}

export type Denomination = SettingsEntityBase & {
  value: number
  displayName: string
  currencyCode: string
  sortOrder: number
}

export type SupportedDevice = SettingsEntityBase & {
  brandName: string
  deviceName: string
  modelCode: string
  supportEsim: boolean
  note: string
}

export type SettingsCrudLabels = {
  searchPlaceholder: string
  addButton: string
  emptyMessage: string
  itemName: string
  deleteTitle?: string
  deleteConfirm?: string
}
