import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEdit, TbEye, TbEyeOff, TbKey } from 'react-icons/tb'

import type { RoleRow } from '@/apis/rolesApi'
import { createIsActiveColumn } from '@/modules/crud/components/tableColumns'

export type RoleTableHandlers = {
  onEdit: (row: RoleRow) => void
  onToggleActive: (row: RoleRow) => void
  onAssignPermissions: (row: RoleRow) => void
}

const helper = createColumnHelper<RoleRow>()

export function buildRoleColumns(handlers: RoleTableHandlers) {
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
    createIsActiveColumn<RoleRow>(),
    {
      id: 'actions',
      header: 'Thao tác',
      enableSorting: false,
      cell: ({ row }: { row: { original: RoleRow } }) => (
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
            title="Gán quyền"
            onClick={() => handlers.onAssignPermissions(row.original)}>
            <TbKey className="fs-lg" />
          </Button>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title={row.original.isActive ? 'Ngưng hoạt động' : 'Kích hoạt'}
            onClick={() => handlers.onToggleActive(row.original)}>
            {row.original.isActive ? <TbEyeOff className="fs-lg" /> : <TbEye className="fs-lg" />}
          </Button>
        </div>
      ),
    },
  ] as ColumnDef<RoleRow>[]
}
