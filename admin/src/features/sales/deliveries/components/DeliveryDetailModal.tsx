import { Modal, Spinner } from 'react-bootstrap'

import type { DeliveryRow } from '@/apis/deliveriesApi'
import {
  DELIVERY_STATUS_LABELS,
  DELIVERY_TYPE_LABELS,
  enumLabel,
  formatDateTime,
} from '@/features/sales/shared/format'

type DeliveryDetailModalProps = {
  delivery: DeliveryRow | null
  loading: boolean
  onClose: () => void
}

const DeliveryDetailModal = ({ delivery, loading, onClose }: DeliveryDetailModalProps) => (
  <Modal show={delivery !== null || loading} onHide={onClose} centered scrollable size="lg">
    <Modal.Header closeButton>
      <Modal.Title>Chi tiết giao hàng</Modal.Title>
    </Modal.Header>
    <Modal.Body>
      {loading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải...
        </div>
      ) : delivery ? (
        <dl className="row mb-0">
          <dt className="col-sm-4">Mã đơn</dt>
          <dd className="col-sm-8"><code>{delivery.orderCode}</code></dd>
          <dt className="col-sm-4">Loại</dt>
          <dd className="col-sm-8">{enumLabel(delivery.deliveryType, DELIVERY_TYPE_LABELS)}</dd>
          <dt className="col-sm-4">Trạng thái</dt>
          <dd className="col-sm-8">{enumLabel(delivery.status, DELIVERY_STATUS_LABELS)}</dd>
          <dt className="col-sm-4">Khách hàng</dt>
          <dd className="col-sm-8">{delivery.customerName || '—'}</dd>
          <dt className="col-sm-4">Customer ID</dt>
          <dd className="col-sm-8"><code>{delivery.customerId || '—'}</code></dd>
          <dt className="col-sm-4">Email</dt>
          <dd className="col-sm-8">{delivery.customerEmail || '—'}</dd>
          <dt className="col-sm-4">Lần thử</dt>
          <dd className="col-sm-8">{delivery.attemptCount}</dd>
          <dt className="col-sm-4">IP</dt>
          <dd className="col-sm-8">{delivery.ipAddress || '—'}</dd>
          <dt className="col-sm-4">Ghi chú</dt>
          <dd className="col-sm-8">{delivery.note || '—'}</dd>
          <dt className="col-sm-4">Lỗi gần nhất</dt>
          <dd className="col-sm-8">{delivery.lastError || '—'}</dd>
          <dt className="col-sm-4">Giao lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.deliveredAt)}</dd>
          <dt className="col-sm-4">Email giao hàng</dt>
          <dd className="col-sm-8">
            {delivery.emailError || (delivery.emailSent ? 'Đã gửi' : 'Chưa gửi')}
            {delivery.emailSentAt ? ` · ${formatDateTime(delivery.emailSentAt)}` : ''}
          </dd>
          <dt className="col-sm-4">Lỗi lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.failedAt)}</dd>
          <dt className="col-sm-4">Tạo lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.createdAt)}</dd>
          <dt className="col-sm-4">Sản phẩm</dt>
          <dd className="col-sm-8">
            {delivery.items.length === 0 ? (
              '—'
            ) : (
              <div className="d-flex flex-column gap-2">
                {delivery.items.map((item) => (
                  <div key={item.id} className="border rounded p-2">
                    <div className="fw-semibold">{item.productName || '—'}</div>
                    <div className="text-muted fs-xxs">
                      SL {item.quantity} · {item.isDelivered ? 'Đã giao' : 'Chưa giao'}
                    </div>
                    <div className="fs-xxs">Serial: <code>{item.serialNumber || '—'}</code></div>
                    <div className="fs-xxs">Provider ref: <code>{item.providerReference || '—'}</code></div>
                    <div className="fs-xxs text-break">Activation: <code>{item.activationCode || '—'}</code></div>
                    {item.qrCodeUrl && (
                      <div className="fs-xxs text-break">
                        QR: <a href={item.qrCodeUrl} target="_blank" rel="noreferrer">{item.qrCodeUrl}</a>
                      </div>
                    )}
                    <div className="text-muted fs-xxs">Giao lúc: {formatDateTime(item.deliveredAt)}</div>
                  </div>
                ))}
              </div>
            )}
          </dd>
        </dl>
      ) : null}
    </Modal.Body>
  </Modal>
)

export default DeliveryDetailModal
