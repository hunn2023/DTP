import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'
import type { Country } from '@/features/master-data/types'

export type CountryTableHandlers = ActionHandlers<Country>

const helper = createColumnHelper<Country>()

export function buildCountryColumns(handlers: CountryTableHandlers) {
  return [
    createSelectColumn<Country>(),
    createIdColumn<Country>(),
    helper.accessor('name', {
      header: 'Quốc gia',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          {row.original.flagUrl ? (
            <img
              src={row.original.flagUrl}
              alt=""
              width={28}
              height={20}
              className="rounded border"
              style={{ objectFit: 'cover' }}
            />
          ) : null}
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">{row.original.slug}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('isoCode', { header: 'Mã', cell: ({ getValue }) => <code>{getValue()}</code> }),
    helper.accessor('region', {
      header: 'Khu vực',
      cell: ({ getValue }) => <span className="text-muted">{getValue() || '—'}</span>,
    }),
    createSortOrderColumn<Country>(),
    createIsActiveColumn<Country>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Country>[]
}
