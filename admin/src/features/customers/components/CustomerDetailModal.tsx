import { Button, Modal, ModalBody, ModalFooter, ModalHeader, ModalTitle, Spinner } from 'react-bootstrap'

import type { CustomerDetail } from '@/apis/customersApi'
import { formatDisplayNumber } from '@/components/form/numberFieldUtils'

type CustomerDetailModalProps = {
  show: boolean
  customer: CustomerDetail | null
  isLoading: boolean
  isUpdating: boolean
  onHide: () => void
  onToggleLock: () => void
}

function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('vi-VN')
}

const CustomerDetailModal = ({
  show,
  customer,
  isLoading,
  isUpdating,
  onHide,
  onToggleLock,
}: CustomerDetailModalProps) => (
  <Modal show={show} onHide={onHide} centered size="lg">
    <ModalHeader closeButton>
      <ModalTitle>Chi tiết khách hàng</ModalTitle>
    </ModalHeader>
    <ModalBody>
      {isLoading ? (
        <div className="text-center py-4">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải chi tiết...
        </div>
      ) : customer ? (
        <div className="row g-3">
          <div className="col-md-6">
            <div className="text-muted fs-xs">Họ tên</div>
            <div className="fw-semibold">{customer.fullName || '—'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Email</div>
            <div>{customer.email || '—'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">SĐT</div>
            <div>{customer.phone || '—'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Trạng thái</div>
            <div>{customer.isActive ? 'Hoạt động' : 'Đã khóa'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Xác nhận email</div>
            <div>{customer.emailConfirmed ? 'Đã xác nhận' : 'Chưa'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Vai trò</div>
            <div>{customer.roles.length > 0 ? customer.roles.join(', ') : '—'}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Tổng đơn hàng</div>
            <div>{formatDisplayNumber(customer.totalOrders)}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Tổng chi tiêu</div>
            <div>{formatDisplayNumber(customer.totalSpent)} đ</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Đăng nhập gần nhất</div>
            <div>{formatDateTime(customer.lastLoginAt)}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Đơn đầu tiên</div>
            <div>{formatDateTime(customer.firstOrderAt)}</div>
          </div>
          <div className="col-md-6">
            <div className="text-muted fs-xs">Đơn gần nhất</div>
            <div>{formatDateTime(customer.lastOrderAt)}</div>
          </div>
        </div>
      ) : (
        <p className="text-muted mb-0">Không có dữ liệu chi tiết.</p>
      )}
    </ModalBody>
    <ModalFooter>
      {customer && (
        <Button
          variant={customer.isActive ? 'outline-danger' : 'outline-primary'}
          disabled={isUpdating}
          onClick={onToggleLock}>
          {isUpdating ? 'Đang xử lý...' : customer.isActive ? 'Khóa tài khoản' : 'Mở khóa'}
        </Button>
      )}
      <Button variant="light" onClick={onHide}>
        Đóng
      </Button>
    </ModalFooter>
  </Modal>
)

export default CustomerDetailModal
