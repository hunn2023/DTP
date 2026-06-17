import { fetchCustomerDetail, fetchCustomersPage } from '@/apis/customersApi'
import type { CustomerRow } from '@/apis/customersApi'
import type { ApiSearchSelectOption } from '@/components/form/ApiSearchSelect'

function mapCustomerToSelectOption(customer: CustomerRow): ApiSearchSelectOption {
  const name = customer.fullName.trim()
  const label = name ? `${name} — ${customer.email}` : customer.email
  return { value: customer.userId, label }
}

export async function searchCustomerSelectOptions(keyword: string): Promise<ApiSearchSelectOption[]> {
  const result = await fetchCustomersPage(1, 30, { keyword: keyword.trim() || undefined })
  return result.items.map(mapCustomerToSelectOption)
}

export async function resolveCustomerSelectOption(
  userId: string,
): Promise<ApiSearchSelectOption | null> {
  if (!userId) return null

  try {
    const detail = await fetchCustomerDetail(userId)
    return mapCustomerToSelectOption(detail)
  } catch {
    const page = await fetchCustomersPage(1, 50)
    const found = page.items.find((item) => item.userId === userId)
    return found ? mapCustomerToSelectOption(found) : null
  }
}
