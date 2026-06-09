import type { ProductFormTab } from '@/features/master-data/products/types'

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
]

export function getStepIndex(tab: ProductFormTab): number {
  return PRODUCT_FORM_STEPS.findIndex((item) => item.key === tab)
}

export function getStepByTab(tab: ProductFormTab): ProductFormStep {
  return PRODUCT_FORM_STEPS[getStepIndex(tab)] ?? PRODUCT_FORM_STEPS[0]
}
