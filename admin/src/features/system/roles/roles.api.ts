import { API_PATHS } from '@/shared/config/api'
import { readBool, readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type RoleRow = {
  id: string
  code: string
  name: string
  description: string
  isActive: boolean
  permissionCount: number
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

export async function fetchRoles(): Promise<RoleRow[]> {
  const raw = await httpGet<unknown>(API_PATHS.adminRoles)
  return normalizeList(raw)
}

export async function fetchRoleById(id: string): Promise<RoleRow> {
  const raw = await httpGet<Raw>(`${API_PATHS.adminRoles}/${id}`)
  return normalizeRole(raw)
}
