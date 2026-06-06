import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'
import type { EsimPackage } from '@/features/products/esim-packages/types'

export type EsimPackageTableHandlers = ActionHandlers<EsimPackage>

const helper = createColumnHelper<EsimPackage>()

function formatData(row: EsimPackage): string {
  if (row.isUnlimited) return 'Unlimited'
  return `${row.dataAmount} ${row.dataUnit}`
}

function formatPrice(row: EsimPackage): string {
  return `${row.price.toLocaleString('vi-VN')} ${row.currency}`
}

export function buildEsimPackageColumns(handlers: EsimPackageTableHandlers) {
  return [
    createSelectColumn<EsimPackage>(),
    createIdColumn<EsimPackage>(),
    helper.accessor('name', {
      header: 'Gói eSIM',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            {row.original.countryName} · {row.original.carrierName}
          </div>
        </div>
      ),
    }),
    helper.accessor('productVariantName', {
      header: 'Biến thể / SP',
      cell: ({ row }) => (
        <div className="fs-xs">
          <div>{row.original.productVariantName || '—'}</div>
          <div className="text-muted">{row.original.productName || '—'}</div>
        </div>
      ),
    }),
    helper.accessor('slug', {
      header: 'Slug',
      cell: ({ getValue }) => <code className="fs-xxs">{getValue()}</code>,
    }),
    helper.display({
      id: 'data',
      header: 'Data',
      cell: ({ row }) => <span>{formatData(row.original)}</span>,
    }),
    helper.accessor('validityDays', {
      header: 'Thời hạn',
      cell: ({ getValue }) => <span>{getValue()} ngày</span>,
    }),
    helper.display({
      id: 'price',
      header: 'Giá',
      cell: ({ row }) => <span className="fw-medium">{formatPrice(row.original)}</span>,
    }),
    helper.accessor('currency', { header: 'Tiền tệ' }),
    helper.accessor('isUnlimited', {
      header: 'Unlimited',
      cell: ({ getValue }) => (getValue() ? '✓' : '—'),
    }),
    createSortOrderColumn<EsimPackage>(),
    createIsActiveColumn<EsimPackage>(),
    createActionsColumn(handlers),
  ] as ColumnDef<EsimPackage>[]
}
