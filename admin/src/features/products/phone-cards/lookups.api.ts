import { fetchProductVariantOptions } from '@/features/products/esim-packages/lookups.api'
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

export async function fetchProviderOptions(): Promise<FormFieldOption[]> {
  const raw = await httpGet<Raw>(API_PATHS.adminProviders, {
    params: { pageIndex: 1, pageSize: 500 },
  })
  return normalizePagedItems(raw).map((item) => ({
    value: readString(item, 'id', 'Id'),
    label: `${readString(item, 'code', 'Code')} — ${readString(item, 'name', 'Name')}`,
  }))
}

export type PhoneCardLookups = {
  productVariantOptions: FormFieldOption[]
  providerOptions: FormFieldOption[]
}

export async function fetchPhoneCardLookups(): Promise<PhoneCardLookups> {
  const [productVariantOptions, providerOptions] = await Promise.all([
    fetchProductVariantOptions(),
    fetchProviderOptions(),
  ])
  return { productVariantOptions, providerOptions }
}
