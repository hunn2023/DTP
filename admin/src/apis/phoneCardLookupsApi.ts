import { fetchProductVariantOptions } from '@/apis/esimPackageLookupsApi'
import { fetchProviderOptions } from '@/apis/providersApi'
import type { FormFieldOption } from '@/modules/crud/form/types'

export type PhoneCardLookups = {
  productVariantOptions: FormFieldOption[]
  providerOptions: FormFieldOption[]
}

export async function fetchPhoneCardFilterOptions(): Promise<Pick<PhoneCardLookups, 'providerOptions'>> {
  const providerOptions = await fetchProviderOptions()
  return { providerOptions }
}

export async function fetchPhoneCardLookups(): Promise<PhoneCardLookups> {
  const [productVariantOptions, providerOptions] = await Promise.all([
    fetchProductVariantOptions(),
    fetchProviderOptions(),
  ])
  return { productVariantOptions, providerOptions }
}

export { fetchProviderOptions }
