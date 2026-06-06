import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'
import type { Carrier } from '@/features/master-data/types'

export type CarrierTableHandlers = ActionHandlers<Carrier>

const helper = createColumnHelper<Carrier>()

export function buildCarrierColumns(handlers: CarrierTableHandlers) {
  return [
    createSelectColumn<Carrier>(),
    createIdColumn<Carrier>(),
    helper.accessor('name', {
      header: 'Nhà mạng',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          {row.original.logoUrl ? (
            <img
              src={row.original.logoUrl}
              alt=""
              width={28}
              height={28}
              className="rounded bg-light p-1"
            />
          ) : null}
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">/{row.original.slug}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('code', { header: 'Mã', cell: ({ getValue }) => <code>{getValue() || '—'}</code> }),
    helper.accessor('countryName', {
      header: 'Quốc gia',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    createSortOrderColumn<Carrier>(),
    createIsActiveColumn<Carrier>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Carrier>[]
}
