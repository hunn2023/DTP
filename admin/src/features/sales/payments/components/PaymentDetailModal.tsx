import { Modal, Spinner } from 'react-bootstrap'

import type { PaymentRow } from '@/apis/paymentsApi'
import { PaymentDetailContent } from '@/features/sales/payments/components/paymentDetailView'

type PaymentDetailModalProps = {
  open: boolean
  payment: PaymentRow | null
  loading: boolean
  onClose: () => void
}

const PaymentDetailModal = ({ open, payment, loading, onClose }: PaymentDetailModalProps) => (
  <Modal show={open} onHide={onClose} centered scrollable size="xl">
    <Modal.Header closeButton className="border-bottom">
      <Modal.Title>Chi tiết thanh toán</Modal.Title>
    </Modal.Header>
    <Modal.Body className="py-3">
      {loading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải giao dịch...
        </div>
      ) : payment ? (
        <PaymentDetailContent payment={payment} />
      ) : null}
    </Modal.Body>
    {!loading && payment ? (
      <Modal.Footer className="border-top">
        <button type="button" className="btn btn-light btn-sm" onClick={onClose}>
          Đóng
        </button>
      </Modal.Footer>
    ) : null}
  </Modal>
)

export default PaymentDetailModal
