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
    helper.accessor('name', { header: 'Tên', cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span> }),
    helper.accessor('value', { header: 'Giá trị' }),
    helper.accessor('sortOrder', { header: 'TT' }),
    createActionsColumn(handlers, { toggleActive: false }),
  ] as ColumnDef<ProductAttributeRow>[]
}
