import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  getContentStatusBadgeClass,
  getContentStatusLabel,
} from '@/features/content/shared/contentStatus'
import type { ContentPage } from '@/features/content/types'
import {
  createActionsColumn,
  createIdColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type PageTableHandlers = ActionHandlers<ContentPage & { isActive: boolean }>

const helper = createColumnHelper<ContentPage & { isActive: boolean }>()

export function buildPageColumns(handlers: PageTableHandlers) {
  return [
    createSelectColumn<ContentPage & { isActive: boolean }>(),
    createIdColumn<ContentPage & { isActive: boolean }>(),
    helper.accessor('title', {
      header: 'Tiêu đề',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.title}</div>
          <div className="text-muted fs-xxs">/{row.original.slug}</div>
        </div>
      ),
    }),
    helper.accessor('code', {
      header: 'Mã',
      cell: ({ getValue }) => <span className="badge badge-soft-secondary fs-xxs">{getValue()}</span>,
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ getValue }) => (
        <span className={`badge ${getContentStatusBadgeClass(getValue())} fs-xxs`}>
          {getContentStatusLabel(getValue())}
        </span>
      ),
    }),
    createSortOrderColumn<ContentPage>(),
    createActionsColumn(handlers, { toggleActive: false }),
  ] as ColumnDef<ContentPage & { isActive: boolean }>[]
}

export function toPageRow(page: ContentPage): ContentPage & { isActive: boolean } {
  return { ...page, isActive: page.status === 1 }
}
