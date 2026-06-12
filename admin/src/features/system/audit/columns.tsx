import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  AUDIT_ACTION_TYPE_LABELS,
  AUDIT_STATUS_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/system/shared/format'
import type { AuditLogRow } from '@/apis/auditLogsApi'

const helper = createColumnHelper<AuditLogRow>()

export function buildAuditLogColumns() {
  return [
    helper.accessor('createdAt', {
      header: 'Thời gian',
      cell: ({ getValue }) => formatDateTime(getValue()),
    }),
    helper.accessor('userName', {
      header: 'Người dùng',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.userName || '—'}</div>
          <div className="text-muted fs-xxs">{row.original.userId || '—'}</div>
        </div>
      ),
    }),
    helper.accessor('module', { header: 'Module' }),
    helper.accessor('action', { header: 'Hành động' }),
    helper.accessor('actionType', {
      header: 'Loại',
      cell: ({ getValue }) => enumLabel(getValue(), AUDIT_ACTION_TYPE_LABELS),
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ getValue }) => {
        const status = getValue()
        const label = enumLabel(status, AUDIT_STATUS_LABELS).toUpperCase()
        const badgeClass =
          status === 1 ? 'badge-soft-primary' : status === 2 ? 'badge-soft-danger' : 'badge-soft-warning'
        return <span className={`badge ${badgeClass} fs-xxs`}>{label}</span>
      },
    }),
    helper.accessor('entityName', {
      header: 'Đối tượng',
      cell: ({ row }) => (
        <div>
          <div>{row.original.entityName || '—'}</div>
          <div className="text-muted fs-xxs">{row.original.entityId || ''}</div>
        </div>
      ),
    }),
    helper.accessor('ipAddress', {
      header: 'IP',
      cell: ({ getValue }) => getValue() || '—',
    }),
    {
      id: 'viewHint',
      header: '',
      cell: () => <span className="text-muted fs-xxs">Xem chi tiết →</span>,
    },
  ] as ColumnDef<AuditLogRow>[]
}
