import { useEffect, useState } from 'react'
import { TbPhoto } from 'react-icons/tb'

import type { ContentBanner } from '@/features/content/types'

import './banner-list-title.scss'

type BannerListTitleCellProps = {
  banner: ContentBanner
}

function getBannerThumbUrl(banner: ContentBanner): string {
  return banner.imageUrl.trim() || banner.mobileImageUrl.trim()
}

const BannerListTitleCell = ({ banner }: BannerListTitleCellProps) => {
  const thumbUrl = getBannerThumbUrl(banner)
  const [imageFailed, setImageFailed] = useState(false)
  const showImage = Boolean(thumbUrl) && !imageFailed

  useEffect(() => {
    setImageFailed(false)
  }, [thumbUrl])

  return (
    <div className="banner-list-title d-flex align-items-center gap-3">
      <div className="banner-list-title__thumb">
        {showImage ? (
          <img
            src={thumbUrl}
            alt={banner.title}
            onError={() => setImageFailed(true)}
          />
        ) : (
          <span className="banner-list-title__placeholder">
            <TbPhoto />
          </span>
        )}
      </div>
      <div className="banner-list-title__content min-w-0">
        <div className="fw-semibold text-truncate" title={banner.title}>
          {banner.title}
        </div>
        {banner.linkUrl ? (
          <div className="banner-list-title__link text-muted fs-xxs" title={banner.linkUrl}>
            {banner.linkUrl}
          </div>
        ) : (
          <div className="text-muted fs-xxs">—</div>
        )}
      </div>
    </div>
  )
}

export default BannerListTitleCell
