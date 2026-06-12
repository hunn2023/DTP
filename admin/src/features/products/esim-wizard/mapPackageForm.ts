import type { EsimPackage } from '@/features/products/esim-packages/types'
import type { EsimPackagePayload } from '@/apis/esimPackagesApi'
import type { EsimPackageForm } from '@/features/products/esim-wizard/types'
import { slugify } from '@/modules/crud/form/slugify'

export function mapPackageToForm(pkg: EsimPackage): EsimPackageForm {
  return {
    id: pkg.id,
    productId: pkg.productId,
    productVariantId: pkg.productVariantId,
    providerId: pkg.providerId,
    countryId: pkg.countryId,
    name: pkg.name,
    slug: pkg.slug,
    providerPackageCode: pkg.providerPackageCode,
    dataAmount: pkg.dataAmount,
    dataUnit: pkg.dataUnit,
    validityDays: pkg.validityDays,
    isUnlimited: pkg.isUnlimited,
    coverageType: pkg.coverageType,
    coverageDescription: pkg.coverageDescription,
    activationPolicy: pkg.activationPolicy,
    speedPolicy: pkg.speedPolicy,
    hotspotSupported: pkg.hotspotSupported,
    phoneNumberSupported: pkg.phoneNumberSupported,
    smsSupported: pkg.smsSupported,
    kycRequired: pkg.kycRequired,
    qrDeliveryType: pkg.qrDeliveryType,
    sortOrder: pkg.sortOrder,
    isActive: pkg.isActive,
    carrierIds: pkg.carriers.map((c) => c.carrierId),
    carriers: pkg.carriers,
  }
}

export function toPackagePayload(values: EsimPackageForm): EsimPackagePayload {
  return {
    productId: values.productId,
    productVariantId: values.productVariantId,
    providerId: values.providerId,
    countryId: values.countryId,
    name: values.name.trim(),
    slug: values.slug.trim() || slugify(values.name),
    providerPackageCode: values.providerPackageCode.trim(),
    dataAmount: values.isUnlimited ? null : values.dataAmount,
    dataUnit: values.dataUnit,
    validityDays: values.validityDays,
    isUnlimited: values.isUnlimited,
    coverageType: values.coverageType,
    coverageDescription: values.coverageDescription.trim() || undefined,
    activationPolicy: values.activationPolicy,
    speedPolicy: values.speedPolicy.trim() || undefined,
    hotspotSupported: values.hotspotSupported,
    phoneNumberSupported: values.phoneNumberSupported,
    smsSupported: values.smsSupported,
    kycRequired: values.kycRequired,
    qrDeliveryType: values.qrDeliveryType,
    sortOrder: values.sortOrder,
    isActive: values.isActive,
    carrierIds: values.carrierIds,
  }
}
