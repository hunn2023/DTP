import type { EsimPackageCarrierLink } from '@/features/products/esim-wizard/types'

export type EsimPackage = {
  id: string
  productId: string
  productName: string
  productVariantId: string
  productVariantName: string
  providerId: string
  providerName: string
  countryId: string
  countryName: string
  name: string
  slug: string
  providerPackageCode: string
  dataAmount: number | null
  dataUnit: string
  validityDays: number
  isUnlimited: boolean
  coverageType: string
  coverageDescription: string
  activationPolicy: string
  speedPolicy: string
  hotspotSupported: boolean
  phoneNumberSupported: boolean
  smsSupported: boolean
  kycRequired: boolean
  qrDeliveryType: string
  sortOrder: number
  isActive: boolean
  carriers: EsimPackageCarrierLink[]
}
