import { useState } from 'react'
import { TbPhoto } from 'react-icons/tb'

import type { ContentArticleListItem } from '@/features/content/types'

import './article-list-title.scss'

type ArticleListTitleCellProps = {
  article: ContentArticleListItem
}

const ArticleListTitleCell = ({ article }: ArticleListTitleCellProps) => {
  const [imageFailed, setImageFailed] = useState(false)
  const thumbUrl = article.thumbnailUrl.trim()
  const showImage = Boolean(thumbUrl) && !imageFailed

  return (
    <div className="article-list-title d-flex align-items-center gap-3">
      <div className="article-list-title__thumb">
        {showImage ? (
          <img src={thumbUrl} alt={article.title} onError={() => setImageFailed(true)} />
        ) : (
          <span className="article-list-title__placeholder">
            <TbPhoto />
          </span>
        )}
      </div>
      <div className="article-list-title__content min-w-0">
        <div className="fw-semibold text-truncate" title={article.title}>
          {article.title}
        </div>
        <div className="text-muted fs-xxs article-list-title__slug" title={article.slug}>
          /{article.slug}
        </div>
      </div>
    </div>
  )
}

export default ArticleListTitleCell
