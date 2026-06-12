export type RoleFormValues = {
  id: string
  code: string
  name: string
  description: string
  isActive: boolean
}

export function getDefaultRoleValues(): RoleFormValues {
  return {
    id: '',
    code: '',
    name: '',
    description: '',
    isActive: true,
  }
}

export function roleRowToFormValues(row: {
  id: string
  code: string
  name: string
  description: string
  isActive: boolean
}): RoleFormValues {
  return {
    id: row.id,
    code: row.code,
    name: row.name,
    description: row.description,
    isActive: row.isActive,
  }
}
