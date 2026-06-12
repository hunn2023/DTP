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

export type PermissionsByModule = Record<string, PermissionRow[]>

function parseActionFromCode(code: string): string {
  const parts = code.split('.')
  return parts.length > 1 ? parts[parts.length - 1] : ''
}

export function normalizePermission(raw: Raw): PermissionRow {
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

function normalizeByModule(raw: unknown): PermissionsByModule {
  if (!raw || typeof raw !== 'object' || Array.isArray(raw)) return {}

  const result: PermissionsByModule = {}
  Object.entries(raw as Record<string, unknown>).forEach(([module, value]) => {
    if (!Array.isArray(value)) return
    result[module] = value.map((item) => normalizePermission(item as Raw))
  })
  return result
}

let cachedPermissions: PermissionRow[] | null = null
let inflightPermissions: Promise<PermissionRow[]> | null = null

let cachedPermissionsByModule: PermissionsByModule | null = null
let inflightPermissionsByModule: Promise<PermissionsByModule> | null = null

export function invalidatePermissionsCache(): void {
  cachedPermissions = null
  inflightPermissions = null
  cachedPermissionsByModule = null
  inflightPermissionsByModule = null
}

export async function fetchPermissions(): Promise<PermissionRow[]> {
  if (cachedPermissions) return cachedPermissions
  if (inflightPermissions) return inflightPermissions

  inflightPermissions = httpGet<unknown>(API_PATHS.adminPermissions)
    .then((raw) => {
      cachedPermissions = normalizeList(raw)
      return cachedPermissions
    })
    .finally(() => {
      inflightPermissions = null
    })

  return inflightPermissions
}

export async function fetchPermissionsByModule(): Promise<PermissionsByModule> {
  if (cachedPermissionsByModule) return cachedPermissionsByModule
  if (inflightPermissionsByModule) return inflightPermissionsByModule

  inflightPermissionsByModule = httpGet<unknown>(API_PATHS.adminPermissionsByModule)
    .then((raw) => {
      cachedPermissionsByModule = normalizeByModule(raw)
      return cachedPermissionsByModule
    })
    .finally(() => {
      inflightPermissionsByModule = null
    })

  return inflightPermissionsByModule
}
