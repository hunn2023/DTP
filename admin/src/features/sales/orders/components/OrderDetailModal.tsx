import { Modal, Spinner } from 'react-bootstrap'

import type { OrderDetail } from '@/apis/ordersApi'
import OrderDetailActions, {
  type OrderDetailActionHandlers,
} from '@/features/sales/orders/components/OrderDetailActions'
import { OrderDetailContent } from '@/features/sales/orders/components/orderDetailView'

type OrderDetailModalProps = {
  order: OrderDetail | null
  loading: boolean
  isSaving?: boolean
  handlers: OrderDetailActionHandlers
  onClose: () => void
}

const OrderDetailModal = ({
  order,
  loading,
  isSaving = false,
  handlers,
  onClose,
}: OrderDetailModalProps) => (
  <Modal show={order !== null || loading} onHide={onClose} centered scrollable size="xl">
    <Modal.Header closeButton className="border-bottom">
      <Modal.Title>Chi tiết đơn hàng</Modal.Title>
    </Modal.Header>
    <Modal.Body className="py-3">
      {loading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải chi tiết đơn...
        </div>
      ) : order ? (
        <OrderDetailContent order={order} />
      ) : null}
    </Modal.Body>
    {!loading && order ? (
      <Modal.Footer className="border-top d-flex flex-wrap justify-content-between gap-2">
        <OrderDetailActions order={order} handlers={handlers} isSaving={isSaving} />
        <button type="button" className="btn btn-light btn-sm" onClick={onClose}>
          Đóng
        </button>
      </Modal.Footer>
    ) : null}
  </Modal>
)

export default OrderDetailModal
