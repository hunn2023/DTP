import { Alert, Modal, Spinner } from 'react-bootstrap'
import { LuCircleAlert } from 'react-icons/lu'

import type { PaymentRow } from '@/apis/paymentsApi'
import { PaymentDetailContent } from '@/features/sales/payments/components/paymentDetailView'

type PaymentDetailModalProps = {
  open: boolean
  payment: PaymentRow | null
  loading: boolean
  error: string | null
  onClose: () => void
}

const PaymentDetailModal = ({ open, payment, loading, error, onClose }: PaymentDetailModalProps) => (
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
      ) : error ? (
        <div className="text-center py-5 px-3">
          <div className="avatar-md mx-auto mb-3">
            <span className="avatar-title bg-danger-subtle text-danger rounded-circle d-inline-flex align-items-center justify-content-center">
              <LuCircleAlert size={24} />
            </span>
          </div>
          <Alert variant="danger" className="mb-0 d-inline-block text-start">
            {error}
          </Alert>
        </div>
      ) : payment ? (
        <PaymentDetailContent payment={payment} />
      ) : null}
    </Modal.Body>
    {!loading && (payment || error) ? (
      <Modal.Footer className="border-top">
        <button type="button" className="btn btn-light btn-sm" onClick={onClose}>
          Đóng
        </button>
      </Modal.Footer>
    ) : null}
  </Modal>
)

export default PaymentDetailModal
