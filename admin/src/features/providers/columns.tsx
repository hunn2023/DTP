import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'
import type { CatalogProvider } from '@/features/providers/types'

export type ProviderTableHandlers = ActionHandlers<CatalogProvider>

const helper = createColumnHelper<CatalogProvider>()

export function buildProviderColumns(handlers: ProviderTableHandlers) {
  return [
    createSelectColumn<CatalogProvider>(),
    createIdColumn<CatalogProvider>(),
    helper.accessor('name', {
      header: 'Nhà cung cấp',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            <code>{row.original.code}</code>
          </div>
        </div>
      ),
    }),
    helper.accessor('apiBaseUrl', {
      header: 'API URL',
      cell: ({ getValue }) => {
        const url = getValue()
        return url ? (
          <span className="text-muted fs-xs text-truncate d-inline-block" style={{ maxWidth: 220 }}>
            {url}
          </span>
        ) : (
          <span className="text-muted">—</span>
        )
      },
    }),
    helper.accessor('supportEmail', {
      header: 'Email',
      cell: ({ getValue }) => getValue() || <span className="text-muted">—</span>,
    }),
    createIsActiveColumn<CatalogProvider>(),
    createActionsColumn(handlers),
  ] as ColumnDef<CatalogProvider>[]
}
