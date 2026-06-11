import { Alert, Badge, ListGroup } from 'react-bootstrap'
import { Link } from 'react-router'

import type { EsimWizardSummary } from '@/features/products/esim-wizard/types'

type WizardReviewTabProps = {
  summary: EsimWizardSummary
  isNew: boolean
}

function formatPrice(value: number, currency: string): string {
  return `${value.toLocaleString('vi-VN')} ${currency}`
}

const WizardReviewTab = ({ summary, isNew }: WizardReviewTabProps) => {
  return (
    <div>
      {isNew && (
        <Alert variant="primary" className="mb-4">
          Đã tạo eSIM Package thành công! Bạn có thể xem hoặc tạo thêm variant khác.
        </Alert>
      )}

      <h5 className="fw-semibold mb-3">Xem lại thông tin</h5>

      <ListGroup variant="flush" className="border rounded">
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Product</span>
          <span className="fw-semibold">{summary.productName || '—'}</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Variant</span>
          <span className="fw-semibold">{summary.variantName || '—'}</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Giá bán</span>
          <span>
            <strong>{formatPrice(summary.salePrice, summary.currency)}</strong>
            {summary.originalPrice > summary.salePrice && (
              <span className="text-muted text-decoration-line-through ms-2 fs-sm">
                {formatPrice(summary.originalPrice, summary.currency)}
              </span>
            )}
          </span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Gói eSIM</span>
          <span>{summary.packageName || '—'}</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Nhà cung cấp</span>
          <span>{summary.providerName || '—'}</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Nhà mạng hỗ trợ</span>
          <span>{summary.carrierNames.length > 0 ? summary.carrierNames.join(', ') : '—'}</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Tính năng</span>
          <span>{summary.featureCount} tính năng</span>
        </ListGroup.Item>
        <ListGroup.Item className="d-flex justify-content-between">
          <span className="text-muted">Trạng thái</span>
          <Badge bg={summary.isActive ? 'primary' : 'secondary'}>
            {summary.isActive ? 'Hoạt động' : 'Ngưng'}
          </Badge>
        </ListGroup.Item>
      </ListGroup>

      <div className="mt-4 d-flex justify-content-between align-items-center">
        <Link to="/products/esim/packages" className="btn btn-light">
          Quay về danh sách
        </Link>
        <Link to="/products/esim/wizard/new" className="btn btn-outline-primary">
          + Tạo variant khác
        </Link>
      </div>
    </div>
  )
}

export default WizardReviewTab
