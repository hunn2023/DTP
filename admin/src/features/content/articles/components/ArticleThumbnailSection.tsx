import { useState } from 'react'
import { Button } from 'react-bootstrap'
import { TbPhoto } from 'react-icons/tb'

import '@/features/content/banners/components/banner-image-preview.scss'

type ArticleThumbnailSectionProps = {
  thumbnailUrl: string
  isUploading?: boolean
  onChooseClick: () => void
}

const ArticleThumbnailSection = ({
  thumbnailUrl,
  isUploading = false,
  onChooseClick,
}: ArticleThumbnailSectionProps) => {
  const [previewFailed, setPreviewFailed] = useState(false)
  const trimmed = thumbnailUrl.trim()
  const showPreview = Boolean(trimmed) && !previewFailed

  return (
    <div>
      <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-2">
        <span className="fw-semibold mb-0">Ảnh thumbnail</span>
        <Button
          type="button"
          variant="outline-primary"
          size="sm"
          disabled={isUploading}
          onClick={onChooseClick}>
          Chọn ảnh thumbnail
        </Button>
      </div>

      {showPreview ? (
        <div className="banner-image-preview banner-image-preview--desktop">
          <img
            src={trimmed}
            alt="Ảnh thumbnail bài viết"
            loading="lazy"
            decoding="async"
            onError={() => setPreviewFailed(true)}
          />
        </div>
      ) : trimmed && previewFailed ? (
        <div className="banner-image-preview banner-image-preview--desktop">
          <div className="banner-image-preview__placeholder">
            <TbPhoto className="fs-2 text-muted" />
            <span className="text-muted fs-xs">Không tải được ảnh thumbnail</span>
          </div>
        </div>
      ) : null}
    </div>
  )
}

export default ArticleThumbnailSection
