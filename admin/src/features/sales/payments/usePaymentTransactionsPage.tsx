import type { ColumnDef } from '@tanstack/react-table'
import { useCallback, useState } from 'react'
import { Button } from 'react-bootstrap'

import { fetchPaymentByOrderId, type PaymentRow } from '@/apis/paymentsApi'
import { fetchOrdersPage } from '@/apis/ordersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { buildPaymentColumns } from '@/features/sales/payments/columns'
import { usePagedList } from '@/features/sales/shared/usePagedList'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function usePaymentTransactionsPage() {
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

  const closeDetail = useCallback(() => {
    setSelectedOrderId(null)
    setPaymentDetail(null)
  }, [])

  return {
    list,
    selectedOrderId,
    paymentDetail,
    isLoadingDetail,
    closeDetail,
  }
}
