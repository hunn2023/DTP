import { Button, Card } from 'react-bootstrap'
import { TbCalendar, TbEdit, TbFileText, TbHash, TbTrash } from 'react-icons/tb'

import {
  getContentStatusBadgeClass,
  getContentStatusLabel,
} from '@/features/content/shared/contentStatus'
import type { ContentPage } from '@/features/content/types'
import { formatDateTime } from '@/features/system/shared/format'

import './pages-grid.scss'

type PageCardProps = {
  page: ContentPage
  onClick: () => void
  onDelete: (id: string) => void
}

const PageCard = ({ page, onClick, onDelete }: PageCardProps) => {
  const displayDate = page.publishedAt || page.createdAt

  return (
    <Card
      className="h-100 page-card shadow-none border"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <Card.Body className="p-3 page-card__body">
        <div className="d-flex align-items-start justify-content-between gap-2 mb-2">
          <span className={`badge ${getContentStatusBadgeClass(page.status)}`}>
            {getContentStatusLabel(page.status)}
          </span>
          {page.code ? <span className="badge badge-soft-secondary">{page.code}</span> : null}
        </div>

        <h6 className="page-card__title mb-2" title={page.title}>
          {page.title}
        </h6>

        <p className="page-card__slug text-muted mb-2" title={page.slug}>
          /{page.slug}
        </p>

        {page.summary ? (
          <p className="page-card__summary text-muted mb-2">{page.summary}</p>
        ) : null}

        <div className="page-card__meta d-flex flex-wrap gap-3">
          <span className="d-inline-flex align-items-center gap-1">
            <TbHash className="page-card__meta-icon flex-shrink-0" />
            <span>Thứ tự: {page.sortOrder}</span>
          </span>
          {displayDate ? (
            <span className="d-inline-flex align-items-center gap-1">
              <TbCalendar className="page-card__meta-icon flex-shrink-0" />
              <span>{formatDateTime(displayDate)}</span>
            </span>
          ) : null}
          <span className="d-inline-flex align-items-center gap-1">
            <TbFileText className="page-card__meta-icon flex-shrink-0" />
            <span>{page.content.trim() ? 'Có nội dung' : 'Chưa có nội dung'}</span>
          </span>
        </div>
      </Card.Body>

      <Card.Footer
        className="bg-transparent border-top d-flex align-items-center justify-content-end gap-1 py-2 px-3"
        onClick={(e) => e.stopPropagation()}>
        <Button variant="light" size="sm" className="btn-icon rounded-circle" title="Sửa" onClick={onClick}>
          <TbEdit className="fs-lg" />
        </Button>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title="Xóa"
          onClick={() => onDelete(page.id)}>
          <TbTrash className="fs-lg" />
        </Button>
      </Card.Footer>
    </Card>
  )
}

export default PageCard
