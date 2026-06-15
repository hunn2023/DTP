import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEdit, TbEye, TbStar, TbStarOff, TbTrash } from 'react-icons/tb'

import {
  getContentStatusBadgeClass,
  getContentStatusLabel,
} from '@/features/content/shared/contentStatus'
import type { ContentArticleListItem } from '@/features/content/types'
import ArticleListTitleCell from '@/features/content/articles/components/ArticleListTitleCell'
import { createIdColumn, createSelectColumn, createSortOrderColumn } from '@/modules/crud/components/tableColumns'

export type ArticleTableHandlers = {
  onEdit: (row: ContentArticleListItem) => void
  onDeleteRequest: (id: string) => void
  onTogglePublish: (row: ContentArticleListItem) => void
  onToggleFeatured: (row: ContentArticleListItem) => void
}

const helper = createColumnHelper<ContentArticleListItem>()

function createArticleActionsColumn(handlers: ArticleTableHandlers) {
  return {
    id: 'actions',
    header: 'Thao tác',
    enableSorting: false,
    cell: ({ row }: { row: { original: ContentArticleListItem } }) => (
      <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title="Sửa"
          onClick={() => handlers.onEdit(row.original)}>
          <TbEdit className="fs-lg" />
        </Button>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title={row.original.status === 1 ? 'Ẩn bài' : 'Đăng bài'}
          onClick={() => handlers.onTogglePublish(row.original)}>
          <TbEye className="fs-lg" />
        </Button>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title={row.original.isFeatured ? 'Bỏ nổi bật' : 'Nổi bật'}
          onClick={() => handlers.onToggleFeatured(row.original)}>
          {row.original.isFeatured ? <TbStarOff className="fs-lg" /> : <TbStar className="fs-lg" />}
        </Button>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title="Xóa"
          onClick={() => handlers.onDeleteRequest(row.original.id)}>
          <TbTrash className="fs-lg" />
        </Button>
      </div>
    ),
  }
}

export function buildArticleColumns(
  handlers: ArticleTableHandlers,
  categoryNameByCode?: Map<string, string>,
) {
  return [
    createSelectColumn<ContentArticleListItem>(),
    createIdColumn<ContentArticleListItem & { isActive: boolean }>(),
    helper.accessor('title', {
      header: 'Bài viết',
      cell: ({ row }) => <ArticleListTitleCell key={row.original.id} article={row.original} />,
    }),
    helper.accessor('categoryCode', {
      header: 'Danh mục',
      cell: ({ getValue }) => {
        const code = getValue()
        if (!code) return <span className="text-muted">—</span>
        const name = categoryNameByCode?.get(code)
        return <span className="badge badge-soft-info fs-xxs">{name ? `${code} — ${name}` : code}</span>
      },
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ getValue }) => {
        const status = getValue()
        return (
          <span className={`badge ${getContentStatusBadgeClass(status)} fs-xxs`}>
            {getContentStatusLabel(status)}
          </span>
        )
      },
    }),
    helper.accessor('isFeatured', {
      header: 'Nổi bật',
      cell: ({ getValue }) =>
        getValue() ? (
          <span className="badge badge-soft-warning fs-xxs">Featured</span>
        ) : (
          <span className="text-muted">—</span>
        ),
    }),
    createSortOrderColumn<ContentArticleListItem>(),
    createArticleActionsColumn(handlers),
  ] as ColumnDef<ContentArticleListItem>[]
}
