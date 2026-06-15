import { useCallback, useEffect, useMemo, useState } from 'react'

import * as ordersApi from '@/apis/ordersApi'
import type { OrderDetail, OrderRow } from '@/apis/ordersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { OrderTableHandlers } from '@/features/sales/orders/columns'
import { buildOrderColumns } from '@/features/sales/orders/columns'
import {
  defaultOrderFilterForm,
  orderFilterFormKey,
  toOrdersQueryFilters,
  type OrderFilterForm,
} from '@/features/sales/orders/orderFilterTypes'
import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { getErrorMessage } from '@/features/sales/shared/getErrorMessage'
import { usePagedList } from '@/features/sales/shared/usePagedList'

type PromptAction = 'markPaid' | 'cancel'

export function useOrdersCrud(config: OrderPageConfig) {
  const { showNotification } = useNotificationContext()
  const [detail, setDetail] = useState<OrderDetail | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [actionRow, setActionRow] = useState<OrderRow | null>(null)
  const [promptAction, setPromptAction] = useState<PromptAction | null>(null)
  const [confirmCompleteRow, setConfirmCompleteRow] = useState<OrderRow | null>(null)
  const [isSaving, setIsSaving] = useState(false)
  const [reloadKey, setReloadKey] = useState(0)
  const [filterForm, setFilterForm] = useState<OrderFilterForm>(() => defaultOrderFilterForm(config.filters))

  useEffect(() => {
    setFilterForm((prev) => {
      const next = defaultOrderFilterForm(config.filters)
      return orderFilterFormKey(prev) === orderFilterFormKey(next) ? prev : next
    })
  }, [config.path])

  const queryFilters = useMemo(() => toOrdersQueryFilters(filterForm), [filterForm])
  const filterKey = useMemo(() => orderFilterFormKey(filterForm), [filterForm])

  const notifySuccess = useCallback(
    (message: string) => {
      showNotification({ title: 'Thành công', message, variant: 'success', delay: 2500 })
    },
    [showNotification],
  )

  const notifyError = useCallback(
    (message: string) => {
      showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
    },
    [showNotification],
  )

  const bumpReload = useCallback(() => setReloadKey((n) => n + 1), [])

  const fetchPage = useCallback(
    (pageIndex: number, pageSize: number, keyword?: string) =>
      ordersApi.fetchOrdersPage(pageIndex + 1, pageSize, keyword, queryFilters),
    [queryFilters],
  )

  const openDetail = useCallback(
    async (row: OrderRow) => {
      setDetail(null)
      setDetailLoading(true)
      try {
        setDetail(await ordersApi.fetchOrderById(row.id))
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không tải được chi tiết đơn'))
      } finally {
        setDetailLoading(false)
      }
    },
    [notifyError],
  )

  const reloadDetail = useCallback(
    async (orderId: string) => {
      try {
        setDetail(await ordersApi.fetchOrderById(orderId))
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không tải lại được chi tiết đơn'))
      }
    },
    [notifyError],
  )

  const handlers = useMemo<OrderTableHandlers>(
    () => ({
      onView: (row) => void openDetail(row),
      onMarkPaid: (row) => {
        setActionRow(row)
        setPromptAction('markPaid')
      },
      onComplete: (row) => setConfirmCompleteRow(row),
      onCancel: (row) => {
        setActionRow(row)
        setPromptAction('cancel')
      },
    }),
    [openDetail],
  )

  const buildColumns = useCallback(() => buildOrderColumns(handlers), [handlers])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    reloadKey,
    filterKey,
    emptyMessage: 'Chưa có đơn hàng',
  })

  const closeDetail = useCallback(() => setDetail(null), [])

  const closePrompt = useCallback(() => {
    setActionRow(null)
    setPromptAction(null)
  }, [])

  const submitPrompt = useCallback(
    async (value: string) => {
      if (!actionRow || !promptAction) return
      setIsSaving(true)
      try {
        if (promptAction === 'markPaid') {
          await ordersApi.markOrderPaid(actionRow.id, { paymentTransactionId: value })
          notifySuccess('Đã đánh dấu thanh toán')
        } else {
          await ordersApi.cancelOrder(actionRow.id, { reason: value })
          notifySuccess('Đã hủy đơn hàng')
        }
        closePrompt()
        bumpReload()
        await reloadDetail(actionRow.id)
      } catch (error) {
        notifyError(getErrorMessage(error, 'Thao tác thất bại'))
      } finally {
        setIsSaving(false)
      }
    },
    [actionRow, promptAction, notifySuccess, notifyError, closePrompt, bumpReload, reloadDetail],
  )

  const confirmComplete = useCallback(async () => {
    if (!confirmCompleteRow) return
    setIsSaving(true)
    try {
      await ordersApi.completeOrder(confirmCompleteRow.id)
      notifySuccess('Đã hoàn thành đơn hàng')
      setConfirmCompleteRow(null)
      bumpReload()
      await reloadDetail(confirmCompleteRow.id)
    } catch (error) {
      notifyError(getErrorMessage(error, 'Không hoàn thành được đơn'))
    } finally {
      setIsSaving(false)
    }
  }, [confirmCompleteRow, notifySuccess, notifyError, bumpReload, reloadDetail])

  return {
    list,
    filterForm,
    setFilterForm,
    handlers,
    openDetail,
    detail,
    detailLoading,
    closeDetail,
    promptAction,
    actionRow,
    confirmCompleteRow,
    setConfirmCompleteRow,
    isSaving,
    closePrompt,
    submitPrompt,
    confirmComplete,
  }
}
