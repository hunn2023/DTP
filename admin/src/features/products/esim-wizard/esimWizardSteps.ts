import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

export type EsimWizardStep = {
  key: EsimWizardTab
  step: number
  title: string
  subtitle: string
}

export const ESIM_WIZARD_STEPS: EsimWizardStep[] = [
  { key: 'variants', step: 1, title: 'Variant', subtitle: 'ProductVariants' },
  { key: 'prices', step: 2, title: 'Giá', subtitle: 'ProductPrices' },
  { key: 'packages', step: 3, title: 'eSIM', subtitle: 'EsimPackages' },
  { key: 'carriers', step: 4, title: 'Carrier', subtitle: 'Chọn nhà mạng' },
  { key: 'features', step: 5, title: 'Feature', subtitle: 'ProductVariantFeatures' },
  { key: 'review', step: 6, title: 'Xem lại', subtitle: 'Tổng hợp thông tin' },
]

export function getEsimStepIndex(tab: EsimWizardTab): number {
  return ESIM_WIZARD_STEPS.findIndex((item) => item.key === tab)
}

export function getEsimStepByTab(tab: EsimWizardTab): EsimWizardStep {
  return ESIM_WIZARD_STEPS[getEsimStepIndex(tab)] ?? ESIM_WIZARD_STEPS[0]
}
