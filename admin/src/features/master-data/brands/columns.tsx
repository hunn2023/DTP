import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/modules/crud/components/tableColumns'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { Brand } from '@/features/master-data/types'

const helper = createColumnHelper<Brand>()

export function buildBrandColumns(handlers: EntityTableHandlers<Brand>) {
  return [
    createSelectColumn<Brand>(),
    createIdColumn<Brand>(),
    helper.accessor('name', {
      header: 'Thương hiệu',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <img src={row.original.logoUrl} alt="" width={32} height={32} className="rounded object-fit-contain bg-light p-1" />
          <div>
            <div className="fw-semibold">{row.original.name}</div>
            <div className="text-muted fs-xxs">/{row.original.slug}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('brandColor', {
      header: 'Màu',
      cell: ({ getValue }) => (
        <span className="d-inline-flex align-items-center gap-2">
          <span className="rounded-circle border" style={{ width: 16, height: 16, backgroundColor: getValue() }} />
          <code className="fs-xxs">{getValue()}</code>
        </span>
      ),
    }),
    helper.accessor('websiteUrl', {
      header: 'Website',
      cell: ({ getValue }) => (
        <a href={getValue()} target="_blank" rel="noreferrer" className="fs-xs">
          {getValue()}
        </a>
      ),
    }),
    createSortOrderColumn<Brand>(),
    createIsActiveColumn<Brand>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Brand>[]
}
