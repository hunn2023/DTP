import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/views/settings/components/settingsTableColumns'
import type { SettingsTableHandlers } from '@/views/settings/hooks/useSettingsCrudTable'
import type { Denomination } from '@/views/settings/types'

const helper = createColumnHelper<Denomination>()

export function buildDenominationColumns(handlers: SettingsTableHandlers<Denomination>) {
  return [
    createSelectColumn<Denomination>(),
    createIdColumn<Denomination>(),
    helper.accessor('value', {
      header: 'Giá trị',
      cell: ({ getValue }) => <span className="fw-semibold">{getValue().toLocaleString('vi-VN')}</span>,
    }),
    helper.accessor('displayName', { header: 'Tên hiển thị' }),
    helper.accessor('currencyCode', {
      header: 'Tiền tệ',
      cell: ({ getValue }) => <code>{getValue()}</code>,
    }),
    createSortOrderColumn<Denomination>(),
    createIsActiveColumn<Denomination>(),
    createActionsColumn(handlers),
  ] as ColumnDef<Denomination>[]
}
