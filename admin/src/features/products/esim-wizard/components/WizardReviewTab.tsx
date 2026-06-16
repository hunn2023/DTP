import { Alert, Badge, Card, Col, Row } from 'react-bootstrap'
import { Link } from 'react-router'

import type { EsimWizardSummary } from '@/features/products/esim-wizard/types'

type WizardReviewTabProps = {
  summary: EsimWizardSummary
  isSetupFlow: boolean
}

function formatPrice(value: number, currency: string): string {
  return `${value.toLocaleString('vi-VN')} ${currency}`
}

const WizardReviewTab = ({ summary, isSetupFlow }: WizardReviewTabProps) => {
  const items = [
    ['Sản phẩm', summary.productName || '-'],
    ['Biến thể', summary.variantName || '-'],
    ['Gói eSIM', summary.packageName || '-'],
    ['Nhà cung cấp', summary.providerName || '-'],
    ['Nhà mạng hỗ trợ', summary.carrierNames.length > 0 ? summary.carrierNames.join(', ') : '-'],
    ['Tính năng', `${summary.featureCount} tính năng`],
  ]

  return (
    <div>
      {isSetupFlow && (
        <Alert variant="primary" className="mb-3">
          Đã tạo gói eSIM thành công. Bạn có thể kiểm tra lại thông tin hoặc quay về danh sách.
        </Alert>
      )}

      <Row className="g-3">
        <Col lg={4}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <div className="text-muted fs-sm mb-1">Giá bán</div>
              <div className="h4 fw-semibold mb-1">{formatPrice(summary.salePrice, summary.currency)}</div>
              {summary.originalPrice > summary.salePrice && (
                <div className="text-muted text-decoration-line-through">
                  {formatPrice(summary.originalPrice, summary.currency)}
                </div>
              )}
              <Badge bg={summary.isActive ? 'primary' : 'secondary'} className="mt-3">
                {summary.isActive ? 'Hoạt động' : 'Ngừng'}
              </Badge>
            </Card.Body>
          </Card>
        </Col>
        <Col lg={8}>
          <Card className="border shadow-none h-100">
            <Card.Body>
              <h5 className="fw-semibold mb-3">Tổng quan cấu hình</h5>
              <div className="d-flex flex-column gap-3">
                {items.map(([label, value]) => (
                  <div key={label} className="d-flex justify-content-between gap-3 border-bottom pb-2">
                    <span className="text-muted">{label}</span>
                    <span className="fw-semibold text-end">{value}</span>
                  </div>
                ))}
              </div>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <div className="mt-4 d-flex justify-content-between align-items-center">
        <Link to="/products/esim/packages" className="btn btn-light">
          Quay về danh sách
        </Link>
        <Link to="/products/esim/wizard/new" className="btn btn-outline-primary">
          Tạo gói khác
        </Link>
      </div>
    </div>
  )
}

export default WizardReviewTab
