/** URL backend — đọc từ .env, dùng cho Vite proxy (dev) và fetch trực tiếp (prod). */
export const API_BASE_URL = import.meta.env.VITE_API_BASE_URL ?? 'http://localhost:5104'

/**
 * Dev: gọi `/api/...` cùng origin → Vite proxy sang VITE_API_BASE_URL (không cần CORS).
 * Prod: gọi thẳng API_BASE_URL.
 */
export const API_REQUEST_BASE = import.meta.env.DEV ? '' : API_BASE_URL

export const API_PATHS = {
  adminCategories: '/api/admin/catalog/categories',
  publicCategories: '/api/catalog/categories',
  adminCountries: '/api/admin/catalog/countries',
  publicCountries: '/api/catalog/countries',
  adminEsimPackages: '/api/admin/catalog/esim-packages',
  adminPhoneCards: '/api/admin/catalog/phone-cards',
  adminCarriers: '/api/admin/catalog/carriers',
  adminProviders: '/api/admin/catalog/providers',
  adminProducts: '/api/admin/catalog/products',
  adminProductVariants: '/api/admin/catalog/product-variants',
  adminProductVariantsByProduct: '/api/admin/catalog/product-variants/by-product',
  adminProductImages: '/api/admin/catalog/product-images',
  adminProductImagesByProduct: '/api/admin/catalog/product-images/by-product',
  adminProductAttributes: '/api/admin/catalog/product-attributes',
  adminProductAttributesByProduct: '/api/admin/catalog/product-attributes/by-product',
  adminProductPrices: '/api/admin/catalog/product-prices',
  adminProductVariantFeatures: '/api/admin/catalog/product-variant-features',
  adminProductVariantFeaturesByVariant:
    '/api/admin/catalog/product-variant-features/by-variant',
} as const
