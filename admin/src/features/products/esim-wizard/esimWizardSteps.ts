import type { EsimWizardTab } from '@/features/products/esim-wizard/types'

export type EsimWizardStep = {
  key: EsimWizardTab
  step: number
  title: string
  subtitle: string
}

export const ESIM_WIZARD_STEPS: EsimWizardStep[] = [
  { key: 'variants', step: 1, title: 'Biến thể', subtitle: 'Chọn sản phẩm & SKU' },
  { key: 'prices', step: 2, title: 'Giá bán', subtitle: 'VND/USD & hiệu lực' },
  { key: 'packages', step: 3, title: 'Gói eSIM', subtitle: 'Data, hạn dùng, chính sách' },
  { key: 'carriers', step: 4, title: 'Nhà mạng', subtitle: 'Phạm vi hỗ trợ' },
  { key: 'features', step: 5, title: 'Tính năng', subtitle: 'Điểm nổi bật trên card' },
  { key: 'review', step: 6, title: 'Xem lại', subtitle: 'Kiểm tra trước khi hoàn tất' },
]

export function getEsimStepIndex(tab: EsimWizardTab): number {
  return ESIM_WIZARD_STEPS.findIndex((item) => item.key === tab)
}

export function getEsimStepByTab(tab: EsimWizardTab): EsimWizardStep {
  return ESIM_WIZARD_STEPS[getEsimStepIndex(tab)] ?? ESIM_WIZARD_STEPS[0]
}

export function getEsimTabTitle(tab: EsimWizardTab): string {
  return getEsimStepByTab(tab).title
}
