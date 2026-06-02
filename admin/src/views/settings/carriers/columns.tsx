import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { Carrier } from '@/views/settings/types'

const helper = createColumnHelper<Carrier>()

export function buildCarrierColumns(handlers: SettingsTableHandlers<Carrier>) {
  return [
    createSelectColumn<Carrier>(),
    createIdColumn<Carrier>(),
    helper.accessor('name', {
      header: 'Nhà mạng',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <img src={row.original.logoUrl} alt="" width={28} height={28} className="rounded bg-light p-1" />
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">/{row.original.slug}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('countryName', { header: 'Quốc gia' }),
    helper.accessor('support5G', {
      header: '5G',
      cell: ({ getValue }) => (
        <span className={`badge ${getValue() ? 'badge-soft-success' : 'badge-soft-secondary'} fs-xxs`}>
          {getValue() ? 'Có' : 'Không'}
        </span>
      ),
    }),
    helper.accessor('coverageNote', {
      header: 'Ghi chú phủ sóng',
      cell: ({ getValue }) => <span className="text-muted fs-xs">{getValue()}</span>,
    }),
    createSortOrderColumn<Carrier>(),
    createIsActiveColumn<Carrier>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Carrier>[]
}
