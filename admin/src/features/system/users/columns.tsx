import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { UserRow } from '@/features/system/users/users.api'

const helper = createColumnHelper<UserRow>()

export function buildUserColumns() {
  return [
    helper.accessor('fullName', {
      header: 'Họ tên',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.fullName || '—'}</div>
          <div className="text-muted fs-xxs">{row.original.email}</div>
        </div>
      ),
    }),
    helper.accessor('phone', {
      header: 'SĐT',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ row }) => (
        <span
          className={`badge ${row.original.isActive ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
          {row.original.status}
        </span>
      ),
    }),
    helper.accessor('emailConfirmed', {
      header: 'Xác nhận email',
      cell: ({ getValue }) => (getValue() ? 'Đã xác nhận' : 'Chưa'),
    }),
  ] as ColumnDef<UserRow>[]
}
