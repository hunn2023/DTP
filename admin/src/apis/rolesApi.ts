import type { PermissionRow } from '@/apis/permissionsApi'
import { normalizePermission } from '@/apis/permissionsApi'
import { API_PATHS } from '@/shared/config/api'
import { parseCreatedId } from '@/shared/lib/createdId'
import { readBool, readString } from '@/shared/lib/dtoNormalize'
import { httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type RoleRow = {
  id: string
  code: string
  name: string
  description: string
  isActive: boolean
  permissionCount: number
}

export type RoleDetail = RoleRow & {
  permissions: PermissionRow[]
}

export type RoleCreatePayload = {
  code: string
  name: string
  description?: string
}

export type RoleUpdatePayload = {
  name: string
  description?: string
  isActive: boolean
}

export type AssignRolePermissionsPayload = {
  permissionIds: string[]
}

function normalizeRole(raw: Raw): RoleRow {
  const permissionsRaw = (raw.permissions ?? raw.Permissions ?? []) as Raw[]
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    name: readString(raw, 'name', 'Name'),
    description: readString(raw, 'description', 'Description'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
    permissionCount: Array.isArray(permissionsRaw) ? permissionsRaw.length : 0,
  }
}

function normalizeRoleDetail(raw: Raw): RoleDetail {
  const permissionsRaw = (raw.permissions ?? raw.Permissions ?? []) as Raw[]
  const permissions = Array.isArray(permissionsRaw)
    ? permissionsRaw.map((item) => normalizePermission(item))
    : []

  return {
    ...normalizeRole(raw),
    permissions,
  }
}

function normalizeList(raw: unknown): RoleRow[] {
  if (Array.isArray(raw)) {
    return raw.map((item) => normalizeRole(item as Raw))
  }
  if (raw && typeof raw === 'object') {
    const record = raw as Raw
    const items = (record.items ?? record.Items) as unknown
    if (Array.isArray(items)) {
      return items.map((item) => normalizeRole(item as Raw))
    }
  }
  return []
}

let cachedRoles: RoleRow[] | null = null
let inflightRoles: Promise<RoleRow[]> | null = null

export function invalidateRolesCache(): void {
  cachedRoles = null
  inflightRoles = null
}

export async function fetchRoles(): Promise<RoleRow[]> {
  if (cachedRoles) return cachedRoles
  if (inflightRoles) return inflightRoles

  inflightRoles = httpGet<unknown>(API_PATHS.adminRoles)
    .then((raw) => {
      cachedRoles = normalizeList(raw)
      return cachedRoles
    })
    .finally(() => {
      inflightRoles = null
    })

  return inflightRoles
}

export async function fetchRoleById(id: string): Promise<RoleDetail> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminRoles}/${id}`)
  return normalizeRoleDetail(raw)
}

export async function createRole(payload: RoleCreatePayload): Promise<string> {
  const body = {
    code: payload.code.trim(),
    name: payload.name.trim(),
    description: payload.description?.trim() || null,
  }
  const raw = await httpPost<unknown>(API_PATHS.adminRoles, body)
  invalidateRolesCache()
  return parseCreatedId(raw)
}

export async function updateRole(id: string, payload: RoleUpdatePayload): Promise<void> {
  const body = {
    name: payload.name.trim(),
    description: payload.description?.trim() || null,
    isActive: payload.isActive,
  }
  await httpPut<unknown>(`${API_PATHS.adminRoles}/${id}`, body)
  invalidateRolesCache()
}

export async function assignRolePermissions(
  id: string,
  payload: AssignRolePermissionsPayload,
): Promise<void> {
  await httpPost<unknown>(`${API_PATHS.adminRoles}/${id}/permissions`, {
    permissionIds: payload.permissionIds,
  })
  invalidateRolesCache()
}
