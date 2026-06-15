import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'

import { getBannerPositionLabel } from '@/features/content/bannerPosition'
import BannerListTitleCell from '@/features/content/banners/components/BannerListTitleCell'
import type { ContentBanner } from '@/features/content/types'
import {
  createActionsColumn,
  createIdColumn,
  createIsActiveColumn,
  createSelectColumn,
  createSortOrderColumn,
  type ActionHandlers,
} from '@/modules/crud/components/tableColumns'

export type BannerTableHandlers = ActionHandlers<ContentBanner>

const helper = createColumnHelper<ContentBanner>()

export function buildBannerColumns(handlers: BannerTableHandlers) {
  return [
    createSelectColumn<ContentBanner>(),
    createIdColumn<ContentBanner>(),
    helper.accessor('title', {
      header: 'Tiêu đề',
      cell: ({ row }) => <BannerListTitleCell key={row.original.id} banner={row.original} />,
    }),
    helper.accessor('position', {
      header: 'Vị trí',
      cell: ({ getValue }) => (
        <span className="badge badge-soft-info fs-xxs">{getBannerPositionLabel(getValue())}</span>
      ),
    }),
    helper.accessor('startDate', {
      header: 'Thời gian',
      cell: ({ row }) => {
        const { startDate, endDate } = row.original
        if (!startDate && !endDate) return <span className="text-muted">—</span>
        return (
          <span className="text-muted fs-xxs">
            {startDate || '...'} → {endDate || '...'}
          </span>
        )
      },
    }),
    createSortOrderColumn<ContentBanner>(),
    createIsActiveColumn<ContentBanner>(),
    createActionsColumn(handlers),
  ] as ColumnDef<ContentBanner>[]
}
