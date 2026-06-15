import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ProductFaqRow } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type FaqTableHandlers = ActionHandlers<ProductFaqRow>

const helper = createColumnHelper<ProductFaqRow>()

export function buildFaqColumns(handlers: FaqTableHandlers) {
  return [
    createSelectColumn<ProductFaqRow>(),
    createIdColumn<ProductFaqRow>(),
    helper.accessor('question', {
      header: 'Câu hỏi',
      cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span>,
    }),
    helper.accessor('answer', {
      header: 'Trả lời',
      cell: ({ getValue }) => {
        const text = getValue()
        return (
          <span className="text-muted text-truncate d-inline-block" style={{ maxWidth: 280 }}>
            {text.length > 80 ? `${text.slice(0, 80)}…` : text}
          </span>
        )
      },
    }),
    helper.accessor('sortOrder', { header: 'TT' }),
    helper.accessor('isActive', {
      header: 'Hiển thị',
      cell: ({ getValue }) => (
        <span className={`badge ${getValue() ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
          {getValue() ? 'Có' : 'Ẩn'}
        </span>
      ),
    }),
    createActionsColumn(handlers, { toggleActive: false }),
  ] as ColumnDef<ProductFaqRow>[]
}
