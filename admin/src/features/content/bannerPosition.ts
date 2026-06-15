import type { ContentBannerPosition } from '@/features/content/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

export const BANNER_POSITION_OPTIONS: FormFieldOption[] = [
  { value: '1', label: 'Home Hero' },
  { value: '2', label: 'Home Middle' },
  { value: '3', label: 'Product List Top' },
  { value: '4', label: 'Product Detail' },
  { value: '5', label: 'Checkout' },
  { value: '6', label: 'Blog' },
]

const positionLabelByValue = new Map(BANNER_POSITION_OPTIONS.map((opt) => [Number(opt.value), opt.label]))

export function getBannerPositionLabel(position: ContentBannerPosition): string {
  return positionLabelByValue.get(position) ?? String(position)
}
