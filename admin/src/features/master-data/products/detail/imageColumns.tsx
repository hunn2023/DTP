import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import type { ProductImageRow } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type ImageTableHandlers = ActionHandlers<ProductImageRow>

const helper = createColumnHelper<ProductImageRow>()

export function buildImageColumns(handlers: ImageTableHandlers) {
  return [
    createSelectColumn<ProductImageRow>(),
    createIdColumn<ProductImageRow>(),
    helper.accessor('imageUrl', {
      header: 'Ảnh',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <img
            src={row.original.imageUrl}
            alt={row.original.altText || 'product'}
            style={{ width: 40, height: 40, objectFit: 'cover' }}
            className="rounded border"
          />
          <span className="text-muted fs-xs text-truncate" style={{ maxWidth: 200 }}>
            {row.original.imageUrl}
          </span>
        </div>
      ),
    }),
    helper.accessor('altText', { header: 'Alt text' }),
    helper.accessor('sortOrder', { header: 'TT' }),
    helper.accessor('isThumbnail', {
      header: 'Thumbnail',
      cell: ({ getValue }) => (getValue() ? '✓' : '—'),
    }),
    createActionsColumn(handlers, { toggleActive: false }),
  ] as ColumnDef<ProductImageRow>[]
}
