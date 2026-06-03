import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/modules/crud/components/tableColumns'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { Country } from '@/features/master-data/types'

const helper = createColumnHelper<Country>()

export function buildCountryColumns(handlers: EntityTableHandlers<Country>) {
  return [
    createSelectColumn<Country>(),
    createIdColumn<Country>(),
    helper.accessor('name', {
      header: 'Quốc gia',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <span className="fs-4">{row.original.flagEmoji}</span>
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">{row.original.englishName}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('isoCode', { header: 'Mã ISO', cell: ({ getValue }) => <code>{getValue()}</code> }),
    helper.accessor('regionCode', {
      header: 'Khu vực',
      cell: ({ getValue }) => <span className="badge badge-soft-primary fs-xxs">{getValue()}</span>,
    }),
    helper.accessor('seoTitle', {
      header: 'SEO',
      cell: ({ row }) => (
        <div className="fs-xs text-muted" style={{ maxWidth: 220 }}>
          <div className="text-truncate">{row.original.seoTitle}</div>
        </div>
      ),
    }),
    createSortOrderColumn<Country>(),
    createIsActiveColumn<Country>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Country>[]
}
