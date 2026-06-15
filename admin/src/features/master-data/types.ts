import type { CrudEntityBase } from '@/modules/crud/types'

export type Category = {
  id: string
  isActive: boolean
  name: string
  slug: string
  code: string
  sortOrder: number
}

export type Brand = CrudEntityBase & {
  name: string
  slug: string
  logoUrl: string
  brandColor: string
  websiteUrl: string
  sortOrder: number
}

export type TagType = 'product' | 'marketing' | 'technical'

export type Tag = CrudEntityBase & {
  name: string
  slug: string
  color: string
  icon: string
  type: TagType
  sortOrder: number
}

export type Country = {
  id: string
  isActive: boolean
  name: string
  slug: string
  isoCode: string
  flagUrl: string
  region: string
  description: string
  sortOrder: number
}

export type Carrier = {
  id: string
  isActive: boolean
  name: string
  slug: string
  code: string
  countryId: string
  countryName: string
  countryFlagUrl: string
  logoUrl: string
  sortOrder: number
}

export type Denomination = CrudEntityBase & {
  value: number
  displayName: string
  currencyCode: string
  sortOrder: number
}

export type SupportedDevice = CrudEntityBase & {
  brandName: string
  deviceName: string
  modelCode: string
  supportEsim: boolean
  note: string
}
