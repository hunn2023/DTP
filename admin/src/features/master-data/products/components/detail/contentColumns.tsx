import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import { getContentTypeLabel } from '@/features/master-data/products/components/detail/contentFormConfig'
import type { ProductContentRow } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type ContentTableHandlers = ActionHandlers<ProductContentRow>

const helper = createColumnHelper<ProductContentRow>()

export function buildContentColumns(handlers: ContentTableHandlers) {
  return [
    createSelectColumn<ProductContentRow>(),
    createIdColumn<ProductContentRow>(),
    helper.accessor('contentType', {
      header: 'Loại',
      cell: ({ row }) => (
        <span className="badge badge-soft-info fs-xxs">
          {row.original.contentTypeName || getContentTypeLabel(row.original.contentType)}
        </span>
      ),
    }),
    helper.accessor('title', {
      header: 'Tiêu đề',
      cell: ({ getValue }) => <span className="fw-semibold">{getValue()}</span>,
    }),
    helper.accessor('summary', {
      header: 'Tóm tắt',
      cell: ({ getValue }) => {
        const text = getValue()
        if (!text) return <span className="text-muted">—</span>
        return (
          <span className="text-muted text-truncate d-inline-block" style={{ maxWidth: 200 }}>
            {text.length > 60 ? `${text.slice(0, 60)}…` : text}
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
  ] as ColumnDef<ProductContentRow>[]
}
