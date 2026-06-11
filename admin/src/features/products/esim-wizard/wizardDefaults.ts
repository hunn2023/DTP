import type { EsimPackageForm } from '@/features/products/esim-wizard/types'
import type { ProductPriceRow, ProductVariant } from '@/features/master-data/products/types'

export function getDefaultVariantValues(productId = ''): ProductVariant {
  return {
    id: '',
    productId,
    sku: '',
    name: '',
    shortName: '',
    description: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function getDefaultPriceValues(productId = '', variantId = ''): ProductPriceRow {
  return {
    id: '',
    productId,
    productName: '',
    productVariantId: variantId,
    productVariantName: '',
    currency: 'VND',
    originalPrice: 0,
    salePrice: 0,
    costPrice: 0,
    startDate: '',
    endDate: '',
    note: '',
    isActive: true,
  }
}

export function getDefaultPackageValues(
  productId = '',
  variantId = '',
  countryId = '',
): EsimPackageForm {
  return {
    id: '',
    productId,
    productVariantId: variantId,
    providerId: '',
    countryId,
    name: '',
    slug: '',
    providerPackageCode: '',
    dataAmount: 1,
    dataUnit: 'GB',
    validityDays: 1,
    isUnlimited: false,
    coverageType: 'Country',
    coverageDescription: '',
    activationPolicy: 'FirstUse',
    speedPolicy: '4G LTE',
    hotspotSupported: true,
    phoneNumberSupported: false,
    smsSupported: false,
    kycRequired: false,
    qrDeliveryType: 'Automatic',
    sortOrder: 1,
    isActive: true,
    carrierIds: [],
    carriers: [],
  }
}
