import { fetchProductVariantOptions } from '@/features/products/esim-packages/lookups.api'
import { fetchProviderOptions } from '@/features/providers/providers.api'
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
