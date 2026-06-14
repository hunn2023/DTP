import { useState } from 'react'
import { Form } from 'react-bootstrap'
import { TbPhoto } from 'react-icons/tb'

import './banner-image-preview.scss'

type BannerImageUrlPreviewProps = {
  label: string
  required?: boolean
  hint?: string
  value: string
  error?: string
  variant: 'desktop' | 'mobile'
  placeholder?: string
  onChange: (value: string) => void
}

const BannerImageUrlPreview = ({
  label,
  required,
  hint,
  value,
  error,
  variant,
  placeholder,
  onChange,
}: BannerImageUrlPreviewProps) => {
  const [previewFailed, setPreviewFailed] = useState(false)
  const trimmed = value.trim()
  const showPreview = Boolean(trimmed) && !previewFailed

  return (
    <div>
      <Form.Label className="fw-semibold">
        {label}
        {required ? <span className="text-danger ms-1">*</span> : null}
        {hint ? <span className="text-muted fw-normal ms-1">({hint})</span> : null}
      </Form.Label>
      <Form.Control
        type="url"
        value={value}
        placeholder={placeholder ?? 'https://cdn.example.com/banner.jpg'}
        isInvalid={Boolean(error)}
        onChange={(e) => {
          setPreviewFailed(false)
          onChange(e.target.value)
        }}
      />
      {error ? <div className="text-danger fs-xs mt-1">{error}</div> : null}

      <div className={`banner-image-preview banner-image-preview--${variant} mt-2`}>
        {showPreview ? (
          <img
            src={trimmed}
            alt={label}
            onError={() => setPreviewFailed(true)}
          />
        ) : (
          <div className="banner-image-preview__placeholder">
            <TbPhoto className="fs-2 text-muted" />
            <span className="text-muted fs-xs">
              {trimmed && previewFailed ? 'Không tải được ảnh từ URL' : 'Nhập URL để xem trước'}
            </span>
          </div>
        )}
      </div>
    </div>
  )
}

export default BannerImageUrlPreview
