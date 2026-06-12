import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Badge } from 'react-bootstrap'

import type { EsimPackage } from '@/features/products/esim-packages/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type EsimPackageTableHandlers = ActionHandlers<EsimPackage>

const helper = createColumnHelper<EsimPackage>()

function formatData(row: EsimPackage): string {
  if (row.isUnlimited) return 'Unlimited'
  if (row.dataAmount == null) return '-'
  return `${row.dataAmount} ${row.dataUnit}`
}

function formatCarriers(row: EsimPackage): string {
  if (row.carriers.length === 0) return 'Chưa gán nhà mạng'
  return row.carriers.map((c) => c.carrierName).join(', ')
}

export function buildEsimPackageColumns(handlers: EsimPackageTableHandlers) {
  return [
    createSelectColumn<EsimPackage>(),
    createIdColumn<EsimPackage>(),
    helper.accessor('name', {
      header: 'Gói eSIM',
      cell: ({ row }) => (
        <div className="py-1">
          <div className="fw-semibold">{row.original.name}</div>
          <div className="text-muted fs-xxs">
            {row.original.countryName || 'Chưa chọn quốc gia'} · {formatCarriers(row.original)}
          </div>
        </div>
      ),
    }),
    helper.accessor('productVariantName', {
      header: 'Biến thể / SP',
      cell: ({ row }) => (
        <div className="fs-xs">
          <div className="fw-semibold">{row.original.productVariantName || '-'}</div>
          <div className="text-muted">{row.original.productName || '-'}</div>
        </div>
      ),
    }),
    helper.accessor('providerName', {
      header: 'Nhà cung cấp',
      cell: ({ getValue }) => <span>{getValue() || '-'}</span>,
    }),
    helper.display({
      id: 'packageMeta',
      header: 'Data / Hạn',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2 flex-wrap">
          <Badge bg={row.original.isUnlimited ? 'success-subtle' : 'info-subtle'} text={row.original.isUnlimited ? 'success' : 'info'}>
            {formatData(row.original)}
          </Badge>
          <span className="text-muted fs-xs">{row.original.validityDays} ngày</span>
        </div>
      ),
    }),
    helper.accessor('speedPolicy', {
      header: 'Tốc độ',
      cell: ({ getValue }) => <span className="text-nowrap">{getValue() || '-'}</span>,
    }),
    helper.accessor('slug', {
      header: 'Slug',
      cell: ({ getValue }) => <code className="fs-xxs">{getValue()}</code>,
    }),
    createSortOrderColumn<EsimPackage>(),
    createIsActiveColumn<EsimPackage>(),
    createActionsColumn(handlers),
  ] as ColumnDef<EsimPackage>[]
}
