import { API_PATHS } from '@/shared/config/api'
import { readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type PermissionRow = {
  id: string
  code: string
  name: string
  module: string
  description: string
  action: string
}

function parseActionFromCode(code: string): string {
  const parts = code.split('.')
  return parts.length > 1 ? parts[parts.length - 1] : ''
}

function normalizePermission(raw: Raw): PermissionRow {
  const code = readString(raw, 'code', 'Code')
  return {
    id: readString(raw, 'id', 'Id'),
    code,
    name: readString(raw, 'name', 'Name'),
    module: readString(raw, 'module', 'Module'),
    description: readString(raw, 'description', 'Description'),
    action: parseActionFromCode(code),
  }
}

function normalizeList(raw: unknown): PermissionRow[] {
  if (Array.isArray(raw)) {
    return raw.map((item) => normalizePermission(item as Raw))
  }
  if (raw && typeof raw === 'object') {
    const record = raw as Raw
    const items = (record.items ?? record.Items) as unknown
    if (Array.isArray(items)) {
      return items.map((item) => normalizePermission(item as Raw))
    }
  }
  return []
}

export async function fetchPermissions(): Promise<PermissionRow[]> {
  const raw = await httpGet<unknown>(API_PATHS.adminPermissions)
  return normalizeList(raw)
}
