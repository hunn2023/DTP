export type EsimWizardTab =
  | 'variants'
  | 'prices'
  | 'packages'
  | 'carriers'
  | 'features'
  | 'review'

export type ProductVariantFeatureRow = {
  id: string
  productVariantId: string
  text: string
  icon: string
  sortOrder: number
  isActive: boolean
}

export type EsimPackageCarrierLink = {
  carrierId: string
  carrierName: string
}

export type EsimPackageForm = {
  id: string
  productId: string
  productVariantId: string
  providerId: string
  countryId: string
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
  carrierIds: string[]
  carriers: EsimPackageCarrierLink[]
}

export type EsimWizardSummary = {
  productName: string
  variantName: string
  salePrice: number
  originalPrice: number
  currency: string
  packageName: string
  providerName: string
  carrierNames: string[]
  featureCount: number
  isActive: boolean
}
