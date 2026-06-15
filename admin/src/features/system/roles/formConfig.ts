import type { RoleFormValues } from '@/features/system/roles/types'
import { getDefaultRoleValues } from '@/features/system/roles/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

const viewFields = [
  { name: 'code' as const, label: 'Mã role', type: 'text' as const, col: 6 as const },
  { name: 'name' as const, label: 'Tên role', type: 'text' as const, col: 6 as const },
  { name: 'description' as const, label: 'Mô tả', type: 'textarea' as const, col: 12 as const },
  { name: 'isActive' as const, label: 'Trạng thái', type: 'checkbox' as const, col: 12 as const },
]

export const roleFormConfig: EntityFormConfig<RoleFormValues> = {
  entityName: 'vai trò',
  getDefaultValues: getDefaultRoleValues,
  viewFields,
  fields: [
    { name: 'code', label: 'Mã role', type: 'text', required: true, col: 6, placeholder: 'ADMIN' },
    { name: 'name', label: 'Tên role', type: 'text', required: true, col: 6 },
    { name: 'description', label: 'Mô tả', type: 'textarea', col: 12 },
  ],
}

export function buildRoleEditFormConfig(): EntityFormConfig<RoleFormValues> {
  return {
    entityName: 'vai trò',
    getDefaultValues: getDefaultRoleValues,
    viewFields,
    fields: [
      { name: 'name', label: 'Tên role', type: 'text', required: true, col: 6 },
      { name: 'description', label: 'Mô tả', type: 'textarea', col: 12 },
      { name: 'isActive', label: 'Đang hoạt động', type: 'checkbox', col: 12 },
    ],
  }
}
