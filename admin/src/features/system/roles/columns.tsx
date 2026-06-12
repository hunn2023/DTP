import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { RoleRow } from '@/apis/rolesApi'

const helper = createColumnHelper<RoleRow>()

export function buildRoleColumns() {
  return [
    helper.accessor('name', {
      header: 'Tên',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            <code>{row.original.code}</code>
          </div>
        </div>
      ),
    }),
    helper.accessor('description', {
      header: 'Mô tả',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('permissionCount', { header: 'Số quyền' }),
    helper.accessor('isActive', {
      header: 'Trạng thái',
      cell: ({ getValue }) => (
        <span className={`badge ${getValue() ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
          {getValue() ? 'Hoạt động' : 'Ngưng'}
        </span>
      ),
    }),
  ] as ColumnDef<RoleRow>[]
}
