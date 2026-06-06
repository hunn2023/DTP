import { fetchCarrierOptions } from '@/features/master-data/carriers/carriers.api'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import { API_PATHS } from '@/shared/config/api'
import { httpGet } from '@/shared/lib/http'
import type { FormFieldOption } from '@/modules/crud/form/types'

type Raw = Record<string, unknown>

function readString(raw: Raw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  return value == null ? '' : String(value)
}

function normalizePagedItems(raw: Raw): Raw[] {
  const items = raw.items ?? raw.Items
  return Array.isArray(items) ? (items as Raw[]) : []
}

export async function fetchCountryOptions(): Promise<FormFieldOption[]> {
  const countries = await fetchCountries()
  return countries.map((item) => ({
    value: item.id,
    label: `${item.isoCode} ${item.name}`,
  }))
}

export async function fetchProductVariantOptions(): Promise<FormFieldOption[]> {
  const productsRaw = await httpGet<Raw>(API_PATHS.adminProducts, {
    params: { pageIndex: 1, pageSize: 100, isActive: true },
  })
  const products = normalizePagedItems(productsRaw)

  const variantGroups = await Promise.all(
    products.map(async (product) => {
      const productId = readString(product, 'id', 'Id')
      const productName = readString(product, 'name', 'Name')
      if (!productId) return [] as FormFieldOption[]

      try {
        const variantsRaw = await httpGet<unknown>(
          `${API_PATHS.adminProductVariantsByProduct}/${productId}`,
        )
        const variants = Array.isArray(variantsRaw) ? (variantsRaw as Raw[]) : []
        return variants.map((variant) => ({
          value: readString(variant, 'id', 'Id'),
          label: `${productName} — ${readString(variant, 'name', 'Name')}`,
        }))
      } catch {
        return []
      }
    }),
  )

  return variantGroups.flat()
}

export type EsimPackageLookups = {
  countryOptions: FormFieldOption[]
  carrierOptions: FormFieldOption[]
  productVariantOptions: FormFieldOption[]
}

export async function fetchEsimPackageLookups(): Promise<EsimPackageLookups> {
  const [countryOptions, carrierOptions, productVariantOptions] = await Promise.all([
    fetchCountryOptions(),
    fetchCarrierOptions(),
    fetchProductVariantOptions(),
  ])
  return { countryOptions, carrierOptions, productVariantOptions }
}

export { fetchCarrierOptions }
