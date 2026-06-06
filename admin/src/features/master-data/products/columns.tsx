import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Link } from 'react-router'

import type { CatalogProduct } from '@/features/master-data/products/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type ProductTableHandlers = ActionHandlers<CatalogProduct> & {
  onManage?: (row: CatalogProduct) => void
}

const helper = createColumnHelper<CatalogProduct>()

export function buildProductColumns(handlers: ProductTableHandlers) {
  return [
    createSelectColumn<CatalogProduct>(),
    createIdColumn<CatalogProduct>(),
    helper.accessor('name', {
      header: 'Sản phẩm',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            <code>{row.original.slug}</code>
          </div>
        </div>
      ),
    }),
    helper.accessor('categoryName', {
      header: 'Danh mục',
      cell: ({ row }) =>
        row.original.categoryName || row.original.categoryId || <span className="text-muted">—</span>,
    }),
    helper.accessor('shortDescription', {
      header: 'Mô tả ngắn',
      cell: ({ getValue }) => {
        const text = getValue()
        if (!text) return <span className="text-muted">—</span>
        return (
          <span className="text-muted fs-xs text-truncate d-inline-block" style={{ maxWidth: 180 }}>
            {text}
          </span>
        )
      },
    }),
    helper.accessor('thumbnailUrl', {
      header: 'Ảnh',
      cell: ({ getValue }) => {
        const url = getValue()
        return url ? (
          <img src={url} alt="" style={{ width: 36, height: 36, objectFit: 'cover' }} className="rounded border" />
        ) : (
          <span className="text-muted">—</span>
        )
      },
    }),
    helper.accessor('code', {
      header: 'Mã',
      cell: ({ getValue }) => {
        const code = getValue()
        return code ? <code>{code}</code> : <span className="text-muted">—</span>
      },
    }),
    helper.accessor('sortOrder', { header: 'TT' }),
    createIsActiveColumn<CatalogProduct>(),
    helper.display({
      id: 'manage',
      header: '',
      cell: ({ row }) => (
        <Link
          to={`/settings/products/${row.original.id}`}
          className="btn btn-sm btn-soft-primary"
          onClick={(e) => e.stopPropagation()}>
          Chi tiết
        </Link>
      ),
    }),
    createActionsColumn(handlers),
  ] as ColumnDef<CatalogProduct>[]
}
