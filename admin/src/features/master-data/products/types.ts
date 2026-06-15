export type CatalogProduct = {
  id: string
  code: string
  name: string
  slug: string
  categoryId: string
  categoryName: string
  countryId: string
  countryName: string
  shortDescription: string
  description: string
  locationText: string
  thumbnailUrl: string
  isFeatured: boolean
  isHot: boolean
  soldCount: number
  sortOrder: number
  isActive: boolean
}

export type ProductVariant = {
  id: string
  productId: string
  sku: string
  name: string
  shortName: string
  description: string
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
  isActive: boolean
  contentType: string
  size: number
}

export type ProductAttributeRow = {
  id: string
  productId: string
  key: string
  displayName: string
  value: string
  sortOrder: number
  isVisible: boolean
  /** Dùng cho EntityFormModal (checkbox) — đồng bộ với isVisible */
  isActive: boolean
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
  note: string
  isActive: boolean
}

export type ProductFaqRow = {
  id: string
  productId: string
  question: string
  answer: string
  sortOrder: number
  isActive: boolean
}

export type ProductContentType = 1 | 2 | 3 | 4 | 5 | 6

export type ProductContentRow = {
  id: string
  productId: string
  contentType: ProductContentType
  contentTypeName: string
  title: string
  summary: string
  bodyHtml: string
  sortOrder: number
  isActive: boolean
}

export type ProductFormTab = 'product' | 'images' | 'attributes' | 'faqs' | 'contents'
