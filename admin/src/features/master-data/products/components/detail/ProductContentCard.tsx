import { Button, Card } from 'react-bootstrap'
import { TbEdit, TbTrash } from 'react-icons/tb'

import { getContentTypeLabel } from '@/features/master-data/products/components/detail/contentFormConfig'
import type { ProductContentRow } from '@/features/master-data/products/types'

import './product-contents-grid.scss'

type Props = {
  item: ProductContentRow
  onClick: () => void
  onDelete: (id: string) => void
}

const ProductContentCard = ({ item, onClick, onDelete }: Props) => {
  const typeLabel = item.contentTypeName || getContentTypeLabel(item.contentType)

  return (
    <Card
      className="h-100 product-content-card"
      role="button"
      onClick={onClick}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault()
          onClick()
        }
      }}
      tabIndex={0}>
      <Card.Body className="p-3 product-content-card__body">
        <div className="d-flex align-items-start justify-content-between gap-2 mb-2">
          <span className="badge badge-soft-info">{typeLabel}</span>
          <span className={`badge ${item.isActive ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
            {item.isActive ? 'Hiển thị' : 'Ẩn'}
          </span>
        </div>

        <h6 className="product-content-card__title mb-2" title={item.title}>
          {item.title}
        </h6>

        {item.summary ? (
          <p className="product-content-card__summary text-muted mb-2">{item.summary}</p>
        ) : (
          <p className="product-content-card__summary text-muted mb-2">Chưa có tóm tắt</p>
        )}

        <div className="product-content-card__meta d-flex flex-wrap gap-3">
          <span>TT: {item.sortOrder}</span>
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

export default ProductContentCard
