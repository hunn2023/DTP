import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import { getTagTypeLabel } from '@/views/settings/tags/data'
import type { Tag } from '@/views/settings/types'

const helper = createColumnHelper<Tag>()

export function buildTagColumns(handlers: SettingsTableHandlers<Tag>) {
  return [
    createSelectColumn<Tag>(),
    createIdColumn<Tag>(),
    helper.accessor('name', {
      header: 'Tag',
      cell: ({ row }) => (
        <span className="badge fs-xs d-inline-flex align-items-center gap-1" style={{ backgroundColor: row.original.color, color: '#fff' }}>
          <span>{row.original.icon}</span>
          {row.original.name}
        </span>
      ),
    }),
    helper.accessor('slug', { header: 'Slug', cell: ({ getValue }) => <code className="fs-xxs">/{getValue()}</code> }),
    helper.accessor('type', {
      header: 'Loại',
      cell: ({ getValue }) => <span className="badge badge-soft-info fs-xxs">{getTagTypeLabel(getValue())}</span>,
    }),
    createSortOrderColumn<Tag>(),
    createIsActiveColumn<Tag>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Tag>[]
}
