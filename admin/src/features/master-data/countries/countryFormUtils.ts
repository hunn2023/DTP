import type { Country } from '@/features/master-data/types'

const INFO_FIELDS: (keyof Country)[] = [
  'name',
  'slug',
  'isoCode',
  'region',
  'description',
  'sortOrder',
  'isActive',
]

export function isCountryInfoDirty(current: Country, baseline: Country): boolean {
  return INFO_FIELDS.some((key) => current[key] !== baseline[key])
}

export type CountrySaveChangesInput = {
  values: Country
  flagFile?: File
  saveInfo: boolean
  saveFlag: boolean
}
