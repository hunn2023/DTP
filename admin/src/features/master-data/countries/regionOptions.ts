import type { FormFieldOption } from '@/modules/crud/form/types'

export const COUNTRY_REGION_OPTIONS: FormFieldOption[] = [
  { value: 'Châu Á', label: 'Châu Á' },
  { value: 'Châu Âu', label: 'Châu Âu' },
  { value: 'Bắc Mỹ', label: 'Bắc Mỹ' },
  { value: 'Nam Mỹ', label: 'Nam Mỹ' },
  { value: 'Châu Phi', label: 'Châu Phi' },
  { value: 'Châu Đại Dương', label: 'Châu Đại Dương' },
  { value: 'Trung Đông', label: 'Trung Đông' },
  { value: 'Caribe', label: 'Caribe' },
  { value: 'Toàn cầu', label: 'Toàn cầu' },
]

export function getCountryRegionOptions(currentValue?: string): FormFieldOption[] {
  const trimmed = currentValue?.trim()
  if (!trimmed || COUNTRY_REGION_OPTIONS.some((opt) => opt.value === trimmed)) {
    return COUNTRY_REGION_OPTIONS
  }
  return [{ value: trimmed, label: trimmed }, ...COUNTRY_REGION_OPTIONS]
}
