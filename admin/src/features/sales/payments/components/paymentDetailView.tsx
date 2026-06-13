import { Badge, Card, CardBody, CardHeader, Col, Row } from 'react-bootstrap'

import type { PaymentRow } from '@/apis/paymentsApi'
import {
  formatCurrency,
  formatDateTime,
  formatPaymentMethod,
  resolveCurrencyUnit,
} from '@/features/sales/shared/format'

function InfoField({ label, value }: { label: string; value: string }) {
  return (
    <div className="mb-2">
      <div className="text-muted fs-xxs text-uppercase mb-1">{label}</div>
      <div className="fw-medium text-break">{value || '—'}</div>
    </div>
  )
}

function PaymentStatusBadge({ status }: { status: string }) {
  if (!status) return <span>—</span>
  return (
    <Badge bg="info" className="rounded-pill px-2 py-1 fw-normal fs-xs">
      {status}
    </Badge>
  )
}

function PaymentSummaryCard({ payment }: { payment: PaymentRow }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Giao dịch</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Mã giao dịch" value={payment.id} />
        <InfoField label="Mã đơn" value={payment.orderCode} />
        <InfoField label="Trạng thái" value={payment.status} />
        <InfoField label="Số tiền" value={formatCurrency(payment.amount, payment.currency)} />
        <InfoField label="Tiền tệ" value={resolveCurrencyUnit(payment.currency)} />
      </CardBody>
    </Card>
  )
}

function PaymentProviderCard({ payment }: { payment: PaymentRow }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Cổng thanh toán</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Provider" value={payment.provider} />
        <InfoField label="Phương thức" value={formatPaymentMethod(payment.method)} />
        <InfoField label="Request ID" value={payment.requestId} />
        <InfoField label="Mã GD cổng" value={payment.providerTransactionId} />
      </CardBody>
    </Card>
  )
}

function PaymentTimelineCard({ payment }: { payment: PaymentRow }) {
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Thời gian</span>
      </CardHeader>
      <CardBody className="py-3">
        <InfoField label="Tạo lúc" value={formatDateTime(payment.createdAt)} />
        <InfoField label="Thanh toán lúc" value={formatDateTime(payment.paidAt)} />
        <InfoField label="Hết hạn" value={formatDateTime(payment.expiredAt)} />
        <InfoField label="Customer ID" value={payment.customerId} />
      </CardBody>
    </Card>
  )
}

function PaymentLinkCard({ payment }: { payment: PaymentRow }) {
  const hasLinks = payment.paymentUrl || payment.qrImageUrl || payment.qrCode
  return (
    <Card className="h-100">
      <CardHeader className="py-2 bg-light bg-opacity-50">
        <span className="fw-semibold fs-sm">Liên kết / QR</span>
      </CardHeader>
      <CardBody className="py-3">
        {payment.paymentUrl ? (
          <InfoField label="Payment URL" value={payment.paymentUrl} />
        ) : null}
        {payment.qrImageUrl ? (
          <InfoField label="QR Image URL" value={payment.qrImageUrl} />
        ) : null}
        {payment.qrCode ? <InfoField label="QR Code" value={payment.qrCode} /> : null}
        {!hasLinks ? <div className="text-muted fs-sm">Không có liên kết thanh toán</div> : null}
      </CardBody>
    </Card>
  )
}

export function PaymentDetailContent({ payment }: { payment: PaymentRow }) {
  return (
    <>
      <div className="d-flex flex-wrap align-items-start justify-content-between gap-2 mb-3">
        <div>
          <div className="text-muted fs-sm mb-1">Mã giao dịch</div>
          <code className="fs-md">{payment.id}</code>
        </div>
        <PaymentStatusBadge status={payment.status} />
      </div>

      <Row className="g-3">
        <Col md={6} xl={3}>
          <PaymentSummaryCard payment={payment} />
        </Col>
        <Col md={6} xl={3}>
          <PaymentProviderCard payment={payment} />
        </Col>
        <Col md={6} xl={3}>
          <PaymentTimelineCard payment={payment} />
        </Col>
        <Col md={6} xl={3}>
          <PaymentLinkCard payment={payment} />
        </Col>
      </Row>
    </>
  )
}
