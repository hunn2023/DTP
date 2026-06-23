import type { CatalogProduct } from '@/features/master-data/products/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

export function getDefaultProductValues(): CatalogProduct {
  return {
    id: '',
    code: '',
    name: '',
    slug: '',
    categoryId: '',
    categoryName: '',
    countryId: '',
    countryName: '',
    shortDescription: '',
    description: '',
    locationText: '',
    thumbnailUrl: '',
    isFeatured: false,
    isHot: false,
    soldCount: 0,
    sortOrder: 1,
    isActive: true,
  }
}

/** Mã 12 chữ số theo thời gian: YYMMDDHHmmss */
export function generateTimedProductCode(date = new Date()): string {
  const pad = (value: number) => String(value).padStart(2, '0')
  const year = String(date.getFullYear()).slice(-2)
  return `${year}${pad(date.getMonth() + 1)}${pad(date.getDate())}${pad(date.getHours())}${pad(date.getMinutes())}${pad(date.getSeconds())}`
}

export function getNewProductValues(): CatalogProduct {
  return {
    ...getDefaultProductValues(),
    code: generateTimedProductCode(),
  }
}

export type ProductFormLookups = {
  categoryOptions: FormFieldOption[]
  countryOptions: FormFieldOption[]
}

export function toProductPayload(values: CatalogProduct, isCreate: boolean) {
  return {
    code: values.code.trim() || undefined,
    name: values.name.trim(),
    slug: values.slug.trim(),
    categoryId: values.categoryId.trim(),
    countryId: values.countryId.trim() || undefined,
    shortDescription: values.shortDescription.trim() || undefined,
    description: values.description.trim() || undefined,
    locationText: values.locationText.trim() || undefined,
    isFeatured: values.isFeatured,
    isHot: values.isHot,
    sortOrder: values.sortOrder,
    isActive: isCreate ? true : values.isActive,
  }
}
