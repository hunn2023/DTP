import { fetchCarriersPage } from '@/apis/carriersApi'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'

export async function searchCarrierSelectOptions(keyword: string): Promise<ApiSearchSelectOption[]> {
  const result = await fetchCarriersPage(1, 30, keyword.trim() || undefined)
  return result.items.map((item) => ({ value: item.id, label: item.name }))
}

export async function resolveCarrierSelectOption(carrierId: string): Promise<ApiSearchSelectOption | null> {
  if (!carrierId) return null

  const result = await fetchCarriersPage(1, 100)
  const found = result.items.find((item) => item.id === carrierId)
  return found ? { value: found.id, label: found.name } : null
}
