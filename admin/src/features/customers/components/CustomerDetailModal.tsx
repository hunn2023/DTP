import {
  Badge,
  Button,
  Card,
  CardBody,
  Col,
  Modal,
  ModalBody,
  ModalFooter,
  ModalHeader,
  ModalTitle,
  Row,
  Spinner,
} from 'react-bootstrap'
import { LuMail, LuPhone, LuShoppingBag, LuWallet } from 'react-icons/lu'

import type { CustomerDetail } from '@/apis/customersApi'
import CustomerAvatar from '@/features/customers/components/CustomerAvatar'
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

function InfoItem({ label, value }: { label: string; value: string }) {
  return (
    <div className="mb-3">
      <div className="text-muted fs-xxs text-uppercase mb-1">{label}</div>
      <div className="fw-medium">{value || '—'}</div>
    </div>
  )
}

function StatCard({
  icon: Icon,
  label,
  value,
  variant,
}: {
  icon: typeof LuShoppingBag
  label: string
  value: string
  variant: 'primary' | 'success'
}) {
  return (
    <Card className="h-100 border shadow-none">
      <CardBody className="d-flex align-items-center gap-3 py-3">
        <span
          className={`avatar-sm rounded-circle bg-${variant}-subtle text-${variant} d-inline-flex align-items-center justify-content-center flex-shrink-0`}>
          <Icon size={18} />
        </span>
        <div className="min-w-0">
          <div className="text-muted fs-xxs">{label}</div>
          <div className="fw-semibold text-truncate">{value}</div>
        </div>
      </CardBody>
    </Card>
  )
}

const CustomerDetailModal = ({
  show,
  customer,
  isLoading,
  isUpdating,
  onHide,
  onToggleLock,
}: CustomerDetailModalProps) => (
  <Modal show={show} onHide={onHide} centered size="lg" scrollable>
    <ModalHeader closeButton className="border-bottom">
      <ModalTitle>Chi tiết khách hàng</ModalTitle>
    </ModalHeader>
    <ModalBody className="py-4">
      {isLoading ? (
        <div className="text-center py-5">
          <Spinner animation="border" size="sm" className="me-2" />
          Đang tải chi tiết...
        </div>
      ) : customer ? (
        <>
          <div className="d-flex align-items-start gap-3 mb-4 pb-3 border-bottom">
            <CustomerAvatar fullName={customer.fullName} avatarUrl={customer.avatarUrl} size="lg" />
            <div className="flex-grow-1 min-w-0">
              <h5 className="mb-1 text-truncate">{customer.fullName || '—'}</h5>
              <div className="d-flex flex-wrap align-items-center gap-2 text-muted fs-sm mb-2">
                <span className="d-inline-flex align-items-center gap-1">
                  <LuMail size={14} />
                  {customer.email || '—'}
                </span>
                {customer.phone && (
                  <span className="d-inline-flex align-items-center gap-1">
                    <LuPhone size={14} />
                    {customer.phone}
                  </span>
                )}
              </div>
              <div className="d-flex flex-wrap gap-2">
                <Badge
                  bg={customer.isActive ? 'success' : 'secondary'}
                  className="rounded-pill px-2 py-1 fw-normal">
                  {customer.isActive ? 'Hoạt động' : 'Đã khóa'}
                </Badge>
                <Badge
                  bg={customer.emailConfirmed ? 'info' : 'warning'}
                  className="rounded-pill px-2 py-1 fw-normal">
                  {customer.emailConfirmed ? 'Email đã xác nhận' : 'Chưa xác nhận email'}
                </Badge>
              </div>
            </div>
          </div>

          <Row className="g-3 mb-4">
            <Col sm={6}>
              <StatCard
                icon={LuShoppingBag}
                label="Tổng đơn hàng"
                value={formatDisplayNumber(customer.totalOrders)}
                variant="primary"
              />
            </Col>
            <Col sm={6}>
              <StatCard
                icon={LuWallet}
                label="Tổng chi tiêu"
                value={`${formatDisplayNumber(customer.totalSpent)} đ`}
                variant="success"
              />
            </Col>
          </Row>

          <Row className="g-4">
            <Col md={6}>
              <h6 className="text-muted fs-xs text-uppercase mb-3">Thông tin tài khoản</h6>
              <InfoItem label="Vai trò" value={customer.roles.length > 0 ? customer.roles.join(', ') : '—'} />
              <InfoItem label="Đăng nhập gần nhất" value={formatDateTime(customer.lastLoginAt)} />
            </Col>
            <Col md={6}>
              <h6 className="text-muted fs-xs text-uppercase mb-3">Lịch sử mua hàng</h6>
              <InfoItem label="Đơn đầu tiên" value={formatDateTime(customer.firstOrderAt)} />
              <InfoItem label="Đơn gần nhất" value={formatDateTime(customer.lastOrderAt)} />
            </Col>
          </Row>
        </>
      ) : (
        <p className="text-muted mb-0 text-center py-4">Không có dữ liệu chi tiết.</p>
      )}
    </ModalBody>
    <ModalFooter className="border-top">
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
