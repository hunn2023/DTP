import { fetchCountryById, fetchCountriesPage } from '@/apis/countriesApi'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'
import type { Country } from '@/features/master-data/types'

export function mapCountryToSelectOption(country: Country): ApiSearchSelectOption {
  const label = country.isoCode ? `${country.isoCode} — ${country.name}` : country.name
  return { value: country.id, label }
}

export async function searchCountrySelectOptions(keyword: string): Promise<ApiSearchSelectOption[]> {
  const result = await fetchCountriesPage(1, 30, keyword.trim() || undefined)
  return result.items.map(mapCountryToSelectOption)
}

export async function resolveCountrySelectOption(
  countryId: string,
): Promise<ApiSearchSelectOption | null> {
  if (!countryId) return null

  const country = await fetchCountryById(countryId)
  if (country) return mapCountryToSelectOption(country)

  const page = await fetchCountriesPage(1, 100)
  const found = page.items.find((item) => item.id === countryId)
  return found ? mapCountryToSelectOption(found) : null
}
