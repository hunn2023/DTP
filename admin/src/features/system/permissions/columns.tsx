import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { PermissionRow } from '@/apis/permissionsApi'

const helper = createColumnHelper<PermissionRow>()

export function buildPermissionColumns() {
  return [
    helper.accessor('code', {
      header: 'Mã',
      cell: ({ getValue }) => <code>{getValue()}</code>,
    }),
    helper.accessor('name', {
      header: 'Tên',
      cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span>,
    }),
    helper.accessor('module', { header: 'Module' }),
    helper.accessor('action', {
      header: 'Hành động',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('description', {
      header: 'Mô tả',
      cell: ({ getValue }) => getValue() || '—',
    }),
  ] as ColumnDef<PermissionRow>[]
}
