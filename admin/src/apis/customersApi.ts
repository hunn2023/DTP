import { API_PATHS } from '@/shared/config/api'
import {
  readBool,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpGet, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type CustomerRow = {
  userId: string
  email: string
  phone: string
  fullName: string
  avatarUrl: string
  isActive: boolean
  emailConfirmed: boolean
  lastLoginAt: string
  roles: string[]
  totalOrders: number
  totalSpent: number
  status: string
}

export type CustomerDetail = CustomerRow & {
  firstOrderAt: string
  lastOrderAt: string
}

export type CustomerListFilters = {
  keyword?: string
  isActive?: boolean
}

export type UpdateCustomerStatusPayload = {
  reason?: string
}

export type CustomersPageResult = {
  items: CustomerRow[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const CUSTOMER_PAGE_SIZE_OPTIONS = [10, 20, 50] as const

function readRoles(raw: Raw): string[] {
  const value = raw.roles ?? raw.Roles
  if (!Array.isArray(value)) return []
  return value.map((item) => String(item))
}

function readDateTime(raw: Raw, camel: string, pascal: string): string {
  const value = raw[camel] ?? raw[pascal]
  if (value == null || value === '') return ''
  return String(value)
}

function normalizeListItem(raw: Raw): CustomerRow {
  const isActive = readBool(raw, 'isActive', 'IsActive')
  return {
    userId: readString(raw, 'userId', 'UserId'),
    email: readString(raw, 'email', 'Email'),
    phone: readString(raw, 'phone', 'Phone'),
    fullName: readString(raw, 'fullName', 'FullName'),
    avatarUrl: readString(raw, 'avatarUrl', 'AvatarUrl'),
    isActive,
    emailConfirmed: readBool(raw, 'emailConfirmed', 'EmailConfirmed'),
    lastLoginAt: readDateTime(raw, 'lastLoginAt', 'LastLoginAt'),
    roles: readRoles(raw),
    totalOrders: readNumber(raw, 'totalOrders', 'TotalOrders'),
    totalSpent: readNumber(raw, 'totalSpent', 'TotalSpent'),
    status: isActive ? 'Active' : 'Locked',
  }
}

function normalizeDetail(raw: Raw): CustomerDetail {
  return {
    ...normalizeListItem(raw),
    firstOrderAt: readDateTime(raw, 'firstOrderAt', 'FirstOrderAt'),
    lastOrderAt: readDateTime(raw, 'lastOrderAt', 'LastOrderAt'),
  }
}

function normalizeCustomersPaged(raw: Raw): CustomersPageResult {
  const itemsRaw = (raw.items ?? raw.Items ?? []) as Raw[]
  return {
    items: Array.isArray(itemsRaw) ? itemsRaw.map(normalizeListItem) : [],
    totalCount:
      readNumber(raw, 'totalItems', 'TotalItems') || readNumber(raw, 'totalCount', 'TotalCount'),
    pageIndex: readNumber(raw, 'pageIndex', 'PageIndex'),
    pageSize: readNumber(raw, 'pageSize', 'PageSize'),
  }
}

export async function fetchCustomersPage(
  pageIndex = 1,
  pageSize = 20,
  filters: CustomerListFilters = {},
): Promise<CustomersPageResult> {
  const raw = await httpGet<Raw>(API_PATHS.adminCustomers, {
    params: {
      pageIndex,
      pageSize,
      keyword: filters.keyword?.trim() || undefined,
      isActive: filters.isActive,
    },
  })

  return normalizeCustomersPaged(raw)
}

export async function fetchCustomerDetail(userId: string): Promise<CustomerDetail> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminCustomers}/${userId}`)
  return normalizeDetail(raw)
}

export async function lockCustomer(
  userId: string,
  payload: UpdateCustomerStatusPayload = {},
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminCustomers}/${userId}/lock`, {
    reason: payload.reason?.trim() || null,
  })
}

export async function unlockCustomer(
  userId: string,
  payload: UpdateCustomerStatusPayload = {},
): Promise<void> {
  await httpPut<unknown>(`${API_PATHS.adminCustomers}/${userId}/unlock`, {
    reason: payload.reason?.trim() || null,
  })
}
