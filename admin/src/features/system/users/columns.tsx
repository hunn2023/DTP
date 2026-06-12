import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEdit, TbLock, TbLockOpen, TbUsers } from 'react-icons/tb'

import type { UserRow } from '@/apis/usersApi'

export type UserTableHandlers = {
  onEdit: (row: UserRow) => void
  onToggleLock: (row: UserRow) => void
  onAssignRoles: (row: UserRow) => void
}

const helper = createColumnHelper<UserRow>()

export function buildUserColumns(handlers: UserTableHandlers) {
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
    {
      id: 'actions',
      header: 'Thao tác',
      enableSorting: false,
      cell: ({ row }: { row: { original: UserRow } }) => (
        <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Sửa"
            onClick={() => handlers.onEdit(row.original)}>
            <TbEdit className="fs-lg" />
          </Button>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Gán vai trò"
            onClick={() => handlers.onAssignRoles(row.original)}>
            <TbUsers className="fs-lg" />
          </Button>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title={row.original.isActive ? 'Khóa tài khoản' : 'Mở khóa'}
            onClick={() => handlers.onToggleLock(row.original)}>
            {row.original.isActive ? <TbLock className="fs-lg" /> : <TbLockOpen className="fs-lg" />}
          </Button>
        </div>
      ),
    },
  ] as ColumnDef<UserRow>[]
}
