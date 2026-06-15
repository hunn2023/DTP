import { Button, Card } from 'react-bootstrap'
import { TbCalendar, TbEdit, TbLink, TbSearch, TbTrash } from 'react-icons/tb'

import { getSeoEntityTypeLabel } from '@/features/content/seo/data'
import type { SeoMetadata } from '@/features/content/types'
import { formatDateTime } from '@/features/system/shared/format'

import './seo-grid.scss'

type SeoCardProps = {
  item: SeoMetadata
  onClick: () => void
  onDelete: (id: string) => void
}

const SeoCard = ({ item, onClick, onDelete }: SeoCardProps) => {
  const targetLabel = item.routePath.trim() || item.entityId.trim() || '—'

  return (
    <Card
      className="h-100 seo-card shadow-none border"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <Card.Body className="p-3 seo-card__body">
        <div className="d-flex align-items-start justify-content-between gap-2 mb-2">
          <span className="badge badge-soft-info">{getSeoEntityTypeLabel(item.entityType)}</span>
          <span className="badge badge-soft-secondary text-uppercase fs-xxs">{item.robots}</span>
        </div>

        <h6 className="seo-card__title mb-2" title={item.metaTitle}>
          {item.metaTitle}
        </h6>

        <p className="seo-card__route text-muted mb-2" title={targetLabel}>
          <TbLink className="me-1" />
          {targetLabel}
        </p>

        {item.metaDescription ? (
          <p className="seo-card__desc text-muted mb-2">{item.metaDescription}</p>
        ) : null}

        <div className="seo-card__meta d-flex flex-wrap gap-3">
          {item.metaKeywords ? (
            <span className="d-inline-flex align-items-center gap-1 text-truncate">
              <TbSearch className="seo-card__meta-icon flex-shrink-0" />
              <span className="text-truncate">{item.metaKeywords}</span>
            </span>
          ) : null}
          {item.updatedAt ? (
            <span className="d-inline-flex align-items-center gap-1">
              <TbCalendar className="seo-card__meta-icon flex-shrink-0" />
              <span>{formatDateTime(item.updatedAt)}</span>
            </span>
          ) : null}
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
          onClick={() => onDelete(item.id)}>
          <TbTrash className="fs-lg" />
        </Button>
      </Card.Footer>
    </Card>
  )
}

export default SeoCard
