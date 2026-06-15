import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ContentFaq } from '@/features/content/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type FaqTableHandlers = ActionHandlers<ContentFaq>

const helper = createColumnHelper<ContentFaq>()

export function buildFaqColumns(
  handlers: FaqTableHandlers,
  categoryNameByCode?: Map<string, string>,
) {  return [
    createSelectColumn<ContentFaq>(),
    createIdColumn<ContentFaq>(),
    helper.accessor('question', {
      header: 'Câu hỏi',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.question}</div>
          <div className="text-muted fs-xxs text-truncate" style={{ maxWidth: '320px' }}>
            {row.original.answer}
          </div>
        </div>
      ),
    }),
    helper.accessor('categoryCode', {
      header: 'Danh mục',
      cell: ({ getValue }) => {
        const code = getValue()
        if (!code) return <span className="text-muted">—</span>
        const name = categoryNameByCode?.get(code)
        return (
          <span className="badge badge-soft-secondary fs-xxs">
            {name ? `${code} — ${name}` : code}
          </span>
        )
      },
    }),    createSortOrderColumn<ContentFaq>(),
    createIsActiveColumn<ContentFaq>(),
    createActionsColumn(handlers),
  ] as ColumnDef<ContentFaq>[]
}
