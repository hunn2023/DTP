import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ProductAttributeRow } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type AttributeTableHandlers = ActionHandlers<ProductAttributeRow>

const helper = createColumnHelper<ProductAttributeRow>()

export function buildAttributeColumns(handlers: AttributeTableHandlers) {
  return [
    createSelectColumn<ProductAttributeRow>(),
    createIdColumn<ProductAttributeRow>(),
    helper.accessor('key', {
      header: 'Key',
      cell: ({ getValue }) => <code>{getValue()}</code>,
    }),
    helper.accessor('displayName', {
      header: 'Tên hiển thị',
      cell: ({ getValue }) => <span className="fw-semibold">{getValue() || '—'}</span>,
    }),
    helper.accessor('value', { header: 'Giá trị' }),
    helper.accessor('sortOrder', { header: 'TT' }),
    helper.accessor('isVisible', {
      header: 'Hiển thị',
      cell: ({ getValue }) => (
        <span className={`badge ${getValue() ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
          {getValue() ? 'Có' : 'Ẩn'}
        </span>
      ),
    }),
    createActionsColumn(handlers, { toggleActive: false }),
  ] as ColumnDef<ProductAttributeRow>[]
}
