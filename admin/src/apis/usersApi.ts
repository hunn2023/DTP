import { API_PATHS } from '@/shared/config/api'
import { parseCreatedId } from '@/shared/lib/createdId'
import { normalizePaged, readBool, readString } from '@/shared/lib/dtoNormalize'
import { httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type UserRow = {
  id: string
  email: string
  fullName: string
  phone: string
  isActive: boolean
  emailConfirmed: boolean
  status: string
}

export type UserDetail = {
  id: string
  email: string
  fullName: string
  phone: string
  avatarUrl: string
  isActive: boolean
  emailConfirmed: boolean
}

export type UserCreatePayload = {
  email: string
  phone?: string
  fullName: string
  password: string
  roleIds?: string[]
}

export type UserUpdatePayload = {
  phone?: string
  fullName: string
  avatarUrl?: string
  isActive: boolean
}

export type AssignUserRolesPayload = {
  roleIds: string[]
}

function normalizeUser(raw: Raw): UserRow {
  const isActive = readBool(raw, 'isActive', 'IsActive')
  return {
    id: readString(raw, 'id', 'Id'),
    email: readString(raw, 'email', 'Email'),
    fullName: readString(raw, 'fullName', 'FullName'),
    phone: readString(raw, 'phone', 'Phone'),
    isActive,
    emailConfirmed: readBool(raw, 'emailConfirmed', 'EmailConfirmed'),
    status: isActive ? 'Active' : 'Locked',
  }
}

function normalizeUserDetail(raw: Raw): UserDetail {
  return {
    id: readString(raw, 'id', 'Id'),
    email: readString(raw, 'email', 'Email'),
    fullName: readString(raw, 'fullName', 'FullName'),
    phone: readString(raw, 'phone', 'Phone'),
    avatarUrl: readString(raw, 'avatarUrl', 'AvatarUrl'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    emailConfirmed: readBool(raw, 'emailConfirmed', 'EmailConfirmed'),
  }
}

export async function fetchUsersPage(
  pageIndex: number,
  pageSize: number,
  keyword?: string,
): Promise<{ items: UserRow[]; totalCount: number; pageIndex: number; pageSize: number }> {
  const raw = await httpGet<Raw>(API_PATHS.adminUsers, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
    },
  })

  const paged = normalizePaged(raw, normalizeUser)
  return { ...paged, items: paged.items }
}

export async function fetchUserById(id: string): Promise<UserDetail> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminUsers}/${id}`)
  return normalizeUserDetail(raw)
}

export async function createUser(payload: UserCreatePayload): Promise<string> {
  const body = {
    email: payload.email.trim(),
    phone: payload.phone?.trim() || null,
    fullName: payload.fullName.trim(),
    password: payload.password,
    roleIds: payload.roleIds ?? [],
  }
  const raw = await httpPost<unknown>(API_PATHS.adminUsers, body)
  return parseCreatedId(raw)
}

export async function updateUser(id: string, payload: UserUpdatePayload): Promise<void> {
  const body = {
    phone: payload.phone?.trim() || null,
    fullName: payload.fullName.trim(),
    avatarUrl: payload.avatarUrl?.trim() || null,
    isActive: payload.isActive,
  }
  await httpPut<unknown>(`${API_PATHS.adminUsers}/${id}`, body)
}

export async function lockUser(id: string): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminUsers}/${id}/lock`, {})
}

export async function unlockUser(id: string): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminUsers}/${id}/unlock`, {})
}

export async function assignUserRoles(id: string, payload: AssignUserRolesPayload): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminUsers}/${id}/assign-roles`, {
    roleIds: payload.roleIds,
  })
}
