export type CatalogProduct = {
  id: string
  code: string
  name: string
  slug: string
  categoryId: string
  categoryName: string
  shortDescription: string
  description: string
  thumbnailUrl: string
  sortOrder: number
  isActive: boolean
}

export type ProductVariant = {
  id: string
  productId: string
  sku: string
  name: string
  price: number
  originalPrice: number | null
  durationDays: number | null
  dataAmount: number | null
  dataUnit: string
  isUnlimited: boolean
  sortOrder: number
  isActive: boolean
}

export type ProductImageRow = {
  id: string
  productId: string
  imageUrl: string
  altText: string
  sortOrder: number
  isThumbnail: boolean
  isActive: true
}

export type ProductAttributeRow = {
  id: string
  productId: string
  name: string
  value: string
  sortOrder: number
  isActive: true
}

export type ProductPriceRow = {
  id: string
  productId: string
  productName: string
  productVariantId: string
  productVariantName: string
  currency: string
  originalPrice: number
  salePrice: number
  costPrice: number
  startDate: string
  endDate: string
  isActive: boolean
}
