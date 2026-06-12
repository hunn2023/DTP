import { Modal, Spinner } from 'react-bootstrap'

import type { PaymentRow } from '@/apis/paymentsApi'
import { formatCurrency, formatDateTime } from '@/features/sales/shared/format'

type PaymentDetailModalProps = {
  payment: PaymentRow | null
  loading: boolean
  onClose: () => void
}

const PaymentDetailModal = ({ payment, loading, onClose }: PaymentDetailModalProps) => {
  return (
    <Modal show={payment !== null || loading} onHide={onClose} centered>
      <Modal.Header closeButton>
        <Modal.Title>Chi tiết thanh toán</Modal.Title>
      </Modal.Header>
      <Modal.Body>
        {loading ? (
          <div className="text-center py-4">
            <Spinner animation="border" size="sm" className="me-2" />
            Đang tải giao dịch...
          </div>
        ) : payment ? (
          <dl className="row mb-0">
            <dt className="col-sm-4">Mã đơn</dt>
            <dd className="col-sm-8"><code>{payment.orderCode}</code></dd>
            <dt className="col-sm-4">Mã GD cổng</dt>
            <dd className="col-sm-8">{payment.providerTransactionId || '—'}</dd>
            <dt className="col-sm-4">Cổng / PT</dt>
            <dd className="col-sm-8">{payment.provider} · {payment.method}</dd>
            <dt className="col-sm-4">Số tiền</dt>
            <dd className="col-sm-8">{formatCurrency(payment.amount, payment.currency)}</dd>
            <dt className="col-sm-4">Trạng thái</dt>
            <dd className="col-sm-8">{payment.status}</dd>
            <dt className="col-sm-4">Thanh toán lúc</dt>
            <dd className="col-sm-8">{formatDateTime(payment.paidAt)}</dd>
            <dt className="col-sm-4">Hết hạn</dt>
            <dd className="col-sm-8">{formatDateTime(payment.expiredAt)}</dd>
          </dl>
        ) : null}
      </Modal.Body>
    </Modal>
  )
}

export default PaymentDetailModal
