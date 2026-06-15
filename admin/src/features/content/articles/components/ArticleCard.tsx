import { useState } from 'react'
import { Button, Card } from 'react-bootstrap'
import { LuFolder, LuUser } from 'react-icons/lu'
import { TbCalendar, TbEdit, TbEye, TbTrash } from 'react-icons/tb'

import defaultArticleImage from '@/assets/images/cards/none-images.png'
import {
  getContentStatusBadgeClass,
  getContentStatusLabel,
} from '@/features/content/shared/contentStatus'
import type { ContentArticleListItem } from '@/features/content/types'
import { formatDateTime } from '@/features/system/shared/format'

import './articles-grid.scss'

type ArticleCardProps = {
  article: ContentArticleListItem
  categoryName?: string
  onClick: () => void
  onDelete: (id: string) => void
}

const ArticleCard = ({ article, categoryName, onClick, onDelete }: ArticleCardProps) => {
  const [imageFailed, setImageFailed] = useState(false)
  const thumbUrl = article.thumbnailUrl.trim()
  const showImage = Boolean(thumbUrl) && !imageFailed
  const displayDate = article.publishedAt || article.createdAt

  return (
    <Card
      className="h-100 article-card shadow-none border"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <div className="article-card__media position-relative">
        {showImage ? (
          <img
            src={thumbUrl}
            alt={article.title}
            className="article-card__image"
            onError={() => setImageFailed(true)}
          />
        ) : (
          <div className="article-card__placeholder">
            <img src={defaultArticleImage} alt="" className="article-card__image article-card__image--default" />
          </div>
        )}

        <div className="article-card__badges position-absolute top-0 start-0 m-2 d-flex flex-wrap gap-1">
          <span className={`badge ${getContentStatusBadgeClass(article.status)}`}>
            {getContentStatusLabel(article.status)}
          </span>
          {article.isFeatured ? <span className="badge bg-primary">Nổi bật</span> : null}
        </div>
      </div>

      <Card.Body className="p-3 article-card__body">
        <h6 className="article-card__title mb-2" title={article.title}>
          {article.title}
        </h6>
        <p className="article-card__slug text-muted mb-2" title={article.slug}>
          /{article.slug}
        </p>
        {article.summary ? (
          <p className="article-card__summary text-muted mb-2">{article.summary}</p>
        ) : null}

        <div className="article-card__meta d-flex flex-wrap gap-3">
          {categoryName ? (
            <span className="d-inline-flex align-items-center gap-1 text-truncate">
              <LuFolder className="article-card__meta-icon flex-shrink-0" />
              <span className="text-truncate">{categoryName}</span>
            </span>
          ) : article.categoryCode ? (
            <span className="d-inline-flex align-items-center gap-1 text-truncate">
              <LuFolder className="article-card__meta-icon flex-shrink-0" />
              <span className="text-truncate">{article.categoryCode}</span>
            </span>
          ) : null}
          {article.authorName ? (
            <span className="d-inline-flex align-items-center gap-1 text-truncate">
              <LuUser className="article-card__meta-icon flex-shrink-0" />
              <span className="text-truncate">{article.authorName}</span>
            </span>
          ) : null}
          {displayDate ? (
            <span className="d-inline-flex align-items-center gap-1">
              <TbCalendar className="article-card__meta-icon flex-shrink-0" />
              <span>{formatDateTime(displayDate)}</span>
            </span>
          ) : null}
          <span className="d-inline-flex align-items-center gap-1">
            <TbEye className="article-card__meta-icon flex-shrink-0" />
            <span>{article.viewCount}</span>
          </span>
        </div>
      </Card.Body>

      <Card.Footer
        className="bg-transparent border-top d-flex align-items-center justify-content-end gap-1 py-2 px-3"
        onClick={(e) => e.stopPropagation()}>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title="Sửa"
          onClick={onClick}>
          <TbEdit className="fs-lg" />
        </Button>
        <Button
          variant="light"
          size="sm"
          className="btn-icon rounded-circle"
          title="Xóa"
          onClick={() => onDelete(article.id)}>
          <TbTrash className="fs-lg" />
        </Button>
      </Card.Footer>
    </Card>
  )
}

export default ArticleCard
