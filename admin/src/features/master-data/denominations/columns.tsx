import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
} from '@/modules/crud/components/tableColumns'
import type { EntityTableHandlers } from '@/modules/crud/hooks/useEntityCrud'
import type { Denomination } from '@/features/master-data/types'

const helper = createColumnHelper<Denomination>()

export function buildDenominationColumns(handlers: EntityTableHandlers<Denomination>) {
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
