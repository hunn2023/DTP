import type { UserFormValues } from '@/features/system/users/types'
import { getDefaultUserValues } from '@/features/system/users/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

const viewFields = [
  { name: 'email' as const, label: 'Email', type: 'text' as const, col: 6 as const },
  { name: 'fullName' as const, label: 'Họ tên', type: 'text' as const, col: 6 as const },
  { name: 'phone' as const, label: 'SĐT', type: 'text' as const, col: 6 as const },
  { name: 'avatarUrl' as const, label: 'Avatar URL', type: 'url' as const, col: 6 as const },
  { name: 'isActive' as const, label: 'Đang hoạt động', type: 'checkbox' as const, col: 12 as const },
]

export const userCreateFormConfig: EntityFormConfig<UserFormValues> = {
  entityName: 'tài khoản',
  getDefaultValues: getDefaultUserValues,
  fields: [
    { name: 'email', label: 'Email', type: 'text', required: true, col: 6 },
    { name: 'fullName', label: 'Họ tên', type: 'text', required: true, col: 6 },
    { name: 'phone', label: 'SĐT', type: 'text', col: 6 },
    { name: 'password', label: 'Mật khẩu', type: 'password', required: true, col: 6 },
  ],
}

export const userEditFormConfig: EntityFormConfig<UserFormValues> = {
  entityName: 'tài khoản',
  getDefaultValues: getDefaultUserValues,
  viewFields,
  fields: [
    { name: 'fullName', label: 'Họ tên', type: 'text', required: true, col: 6 },
    { name: 'phone', label: 'SĐT', type: 'text', col: 6 },
    { name: 'avatarUrl', label: 'Avatar URL', type: 'url', col: 12 },
    { name: 'isActive', label: 'Đang hoạt động', type: 'checkbox', col: 12 },
  ],
}
