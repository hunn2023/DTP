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
  <Modal show={delivery !== null || loading} onHide={onClose} centered scrollable>
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
          <dt className="col-sm-4">Email</dt>
          <dd className="col-sm-8">{delivery.customerEmail || '—'}</dd>
          <dt className="col-sm-4">Lần thử</dt>
          <dd className="col-sm-8">{delivery.attemptCount}</dd>
          <dt className="col-sm-4">Lỗi gần nhất</dt>
          <dd className="col-sm-8">{delivery.lastError || '—'}</dd>
          <dt className="col-sm-4">Giao lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.sentAt)}</dd>
          <dt className="col-sm-4">Tạo lúc</dt>
          <dd className="col-sm-8">{formatDateTime(delivery.createdAt)}</dd>
        </dl>
      ) : null}
    </Modal.Body>
  </Modal>
)

export default DeliveryDetailModal
