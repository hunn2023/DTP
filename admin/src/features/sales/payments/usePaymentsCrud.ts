import { useCallback, useMemo, useState } from 'react'

import {
  fetchPaymentDetailById,
  fetchPaymentDetailByOrderId,
  type PaymentDetailResult,
} from '@/apis/paymentsApi'
import * as ordersApi from '@/apis/ordersApi'
import type { OrderRow } from '@/apis/ordersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { buildPaymentOrderColumns } from '@/features/sales/payments/columns'
import { mergePaymentDetail } from '@/features/sales/payments/paymentDetailTypes'
import {
  defaultOrderFilterForm,
  orderFilterFormKey,
  toOrdersQueryFilters,
  type OrderFilterForm,
} from '@/features/sales/orders/orderFilterTypes'
import { getErrorMessage } from '@/features/sales/shared/getErrorMessage'
import { usePagedList } from '@/features/sales/shared/usePagedList'

export function usePaymentsCrud() {
  const { showNotification } = useNotificationContext()
  const [filterForm, setFilterForm] = useState<OrderFilterForm>(() => defaultOrderFilterForm())
  const [detailOpen, setDetailOpen] = useState(false)
  const [detailLoading, setDetailLoading] = useState(false)
  const [detailRaw, setDetailRaw] = useState<PaymentDetailResult | null>(null)
  const [paymentIdLookup, setPaymentIdLookup] = useState('')

  const queryFilters = useMemo(() => toOrdersQueryFilters(filterForm), [filterForm])
  const filterKey = useMemo(() => orderFilterFormKey(filterForm), [filterForm])

  const notifyError = useCallback(
    (message: string) => {
      showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
    },
    [showNotification],
  )

  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      ordersApi.fetchOrdersPage(pageIndex + 1, pageSize, keyword, queryFilters),
    [queryFilters],
  )

  const openDetailFromOrder = useCallback(
    async (row: OrderRow) => {
      setDetailOpen(true)
      setDetailRaw(null)
      setDetailLoading(true)
      try {
        setDetailRaw(await fetchPaymentDetailByOrderId(row.id))
      } catch (error) {
        setDetailOpen(false)
        notifyError(getErrorMessage(error, 'Không tải được giao dịch theo đơn'))
      } finally {
        setDetailLoading(false)
      }
    },
    [notifyError],
  )

  const loadPaymentById = useCallback(async () => {
    const id = paymentIdLookup.trim()
    if (!id) return
    setDetailOpen(true)
    setDetailRaw(null)
    setDetailLoading(true)
    try {
      setDetailRaw(await fetchPaymentDetailById(id))
    } catch (error) {
      setDetailOpen(false)
      notifyError(getErrorMessage(error, 'Không tải được giao dịch theo mã'))
    } finally {
      setDetailLoading(false)
    }
  }, [paymentIdLookup, notifyError])

  const handlers = useMemo(
    () => ({
      onView: (row: OrderRow) => void openDetailFromOrder(row),
    }),
    [openDetailFromOrder],
  )

  const buildColumns = useCallback(() => buildPaymentOrderColumns(handlers), [handlers])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    filterKey,
    emptyMessage: 'Chưa có giao dịch',
  })

  const closeDetail = useCallback(() => {
    setDetailOpen(false)
    setDetailRaw(null)
  }, [])

  const paymentDetail = useMemo(
    () => (detailRaw ? mergePaymentDetail(detailRaw) : null),
    [detailRaw],
  )

  return {
    list,
    filterForm,
    setFilterForm,
    detailOpen,
    detailLoading,
    detailRaw,
    paymentDetail,
    paymentIdLookup,
    setPaymentIdLookup,
    loadPaymentById,
    openDetailFromOrder,
    closeDetail,
  }
}
