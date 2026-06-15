export type UserFormValues = {
  id: string
  email: string
  fullName: string
  phone: string
  password: string
  avatarUrl: string
  isActive: boolean
  roleIds: string[]
}

export function getDefaultUserValues(): UserFormValues {
  return {
    id: '',
    email: '',
    fullName: '',
    phone: '',
    password: '',
    avatarUrl: '',
    isActive: true,
    roleIds: [],
  }
}

export function userRowToFormValues(row: {
  id: string
  email: string
  fullName: string
  phone: string
  isActive: boolean
}): UserFormValues {
  return {
    id: row.id,
    email: row.email,
    fullName: row.fullName,
    phone: row.phone,
    password: '',
    avatarUrl: '',
    isActive: row.isActive,
    roleIds: [],
  }
}

export function userDetailToFormValues(detail: {
  id: string
  email: string
  fullName: string
  phone: string
  avatarUrl: string
  isActive: boolean
}): UserFormValues {
  return {
    id: detail.id,
    email: detail.email,
    fullName: detail.fullName,
    phone: detail.phone,
    password: '',
    avatarUrl: detail.avatarUrl,
    isActive: detail.isActive,
    roleIds: [],
  }
}
