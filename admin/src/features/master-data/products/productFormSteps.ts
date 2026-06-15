import type { ProductFormTab } from '@/features/master-data/products/types'

export const PRODUCT_TAB_LABELS: Record<ProductFormTab, string> = {
  product: 'Thông tin Product',
  images: 'Hình ảnh',
  attributes: 'Thuộc tính',
  faqs: 'FAQ',
  contents: 'Nội dung',
}

export function getProductTabTitle(tab: ProductFormTab): string {
  return PRODUCT_TAB_LABELS[tab] ?? PRODUCT_TAB_LABELS.product
}

export type ProductFormStep = {
  key: ProductFormTab
  step: number
  title: string
  subtitle: string
}

export const PRODUCT_FORM_STEPS: ProductFormStep[] = [
  { key: 'product', step: 1, title: 'Product', subtitle: 'Thông tin cơ bản' },
  { key: 'images', step: 2, title: 'Ảnh', subtitle: 'Upload ảnh Product' },
  { key: 'attributes', step: 3, title: 'Thuộc tính', subtitle: 'ProductAttributes' },
  { key: 'faqs', step: 4, title: 'FAQ', subtitle: 'ProductFaq' },
  { key: 'contents', step: 5, title: 'Nội dung', subtitle: 'ProductContent' },
]

export function getStepIndex(tab: ProductFormTab): number {
  return PRODUCT_FORM_STEPS.findIndex((item) => item.key === tab)
}

export function getStepByTab(tab: ProductFormTab): ProductFormStep {
  return PRODUCT_FORM_STEPS[getStepIndex(tab)] ?? PRODUCT_FORM_STEPS[0]
}
