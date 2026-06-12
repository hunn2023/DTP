import type { ColumnDef } from '@tanstack/react-table'
import { useCallback, useState } from 'react'
import { Button, Modal, Spinner } from 'react-bootstrap'

import { buildPaymentColumns } from '@/features/sales/payments/columns'
import { fetchPaymentByOrderId, type PaymentRow } from '@/apis/paymentsApi'
import { fetchOrdersPage } from '@/apis/ordersApi'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import { formatCurrency, formatDateTime } from '@/features/sales/shared/format'
import { usePagedList } from '@/features/sales/shared/usePagedList'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'
import { useNotificationContext } from '@/context/useNotificationContext'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function PaymentDetailModal({
  payment,
  loading,
  onClose,
}: {
  payment: PaymentRow | null
  loading: boolean
  onClose: () => void
}) {
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

const PaymentTransactionsPage = () => {
  const { showNotification } = useNotificationContext()
  const [selectedOrderId, setSelectedOrderId] = useState<string | null>(null)
  const [paymentDetail, setPaymentDetail] = useState<PaymentRow | null>(null)
  const [isLoadingDetail, setIsLoadingDetail] = useState(false)

  const fetchPage = useCallback(async (pageIndex: number, pageSize: number, keyword?: string) => {
    const orders = await fetchOrdersPage(pageIndex + 1, pageSize, keyword)
    const items: PaymentRow[] = orders.items.map((order) => ({
      id: order.id,
      orderId: order.id,
      orderCode: order.orderCode,
      customerId: order.customerId,
      amount: order.totalAmount,
      currency: order.currency,
      provider: '—',
      method: order.paymentMethod || '—',
      status: String(order.paymentStatus),
      providerTransactionId: '',
      paidAt: order.paidAt,
      createdAt: order.createdAt,
      expiredAt: '',
    }))
    return { ...orders, items }
  }, [])

  const loadPaymentDetail = useCallback(
    async (orderId: string) => {
      setSelectedOrderId(orderId)
      setPaymentDetail(null)
      setIsLoadingDetail(true)
      try {
        const payment = await fetchPaymentByOrderId(orderId)
        setPaymentDetail(payment)
      } catch (error) {
        setSelectedOrderId(null)
        showNotification({
          title: 'Lỗi',
          message: getErrorMessage(error, 'Không tải được giao dịch thanh toán'),
          variant: 'danger',
          delay: 4000,
        })
      } finally {
        setIsLoadingDetail(false)
      }
    },
    [showNotification],
  )

  const buildColumns = useCallback((): ColumnDef<PaymentRow>[] => {
    const base = buildPaymentColumns()
    return [
      ...base,
      {
        id: 'actions',
        header: '',
        cell: ({ row }) => (
          <Button
            variant="light"
            size="sm"
            onClick={() => void loadPaymentDetail(row.original.orderId)}>
            Chi tiết
          </Button>
        ),
      },
    ]
  }, [loadPaymentDetail])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    emptyMessage: 'Chưa có giao dịch',
  })

  const closeDetail = () => {
    setSelectedOrderId(null)
    setPaymentDetail(null)
  }

  return (
    <EntityPageLayout
      title="Giao dịch thanh toán"
      subtitle="Bán hàng"
      description="Danh sách đơn và chi tiết giao dịch từ API thanh toán.">
      <PagedListTable
        searchPlaceholder="Tìm mã đơn, khách..."
        loadingLabel="Đang tải giao dịch..."
        {...list}
      />
      <PaymentDetailModal
        payment={selectedOrderId ? paymentDetail : null}
        loading={isLoadingDetail}
        onClose={closeDetail}
      />
    </EntityPageLayout>
  )
}

export default PaymentTransactionsPage
