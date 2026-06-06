import { fetchCarrierOptions } from '@/features/master-data/carriers/carriers.api'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import { fetchAllProductVariantOptions } from '@/features/master-data/products/product-variants.api'
import type { FormFieldOption } from '@/modules/crud/form/types'

export async function fetchCountryOptions(): Promise<FormFieldOption[]> {
  const countries = await fetchCountries()
  return countries.map((item) => ({
    value: item.id,
    label: `${item.isoCode} ${item.name}`,
  }))
}

export type EsimPackageLookups = {
  countryOptions: Awaited<ReturnType<typeof fetchCountryOptions>>
  carrierOptions: Awaited<ReturnType<typeof fetchCarrierOptions>>
  productVariantOptions: Awaited<ReturnType<typeof fetchAllProductVariantOptions>>
}

export async function fetchEsimFilterOptions(): Promise<Pick<EsimPackageLookups, 'countryOptions' | 'carrierOptions'>> {
  const [countryOptions, carrierOptions] = await Promise.all([
    fetchCountryOptions(),
    fetchCarrierOptions(),
  ])
  return { countryOptions, carrierOptions }
}

export async function fetchEsimPackageLookups(): Promise<EsimPackageLookups> {
  const [countryOptions, carrierOptions, productVariantOptions] = await Promise.all([
    fetchCountryOptions(),
    fetchCarrierOptions(),
    fetchAllProductVariantOptions(),
  ])
  return { countryOptions, carrierOptions, productVariantOptions }
}

export { fetchCarrierOptions, fetchAllProductVariantOptions as fetchProductVariantOptions }
