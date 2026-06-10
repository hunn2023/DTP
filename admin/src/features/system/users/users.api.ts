import { API_PATHS } from '@/shared/config/api'
import { normalizePaged, readBool, readString } from '@/shared/lib/dtoNormalize'
import { httpGet } from '@/shared/lib/http'

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
