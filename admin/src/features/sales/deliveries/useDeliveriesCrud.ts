import { useCallback, useMemo, useState } from 'react'

import * as deliveriesApi from '@/apis/deliveriesApi'
import type { DeliveryRow } from '@/apis/deliveriesApi'
import type { DeliveriesQueryFilters } from '@/apis/deliveriesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { DeliveryTableHandlers } from '@/features/sales/deliveries/columns'
import { buildDeliveryColumns } from '@/features/sales/deliveries/columns'
import { getErrorMessage } from '@/features/sales/shared/getErrorMessage'
import { usePagedList } from '@/features/sales/shared/usePagedList'

type PromptAction = 'delivered' | 'failed'

type UseDeliveriesCrudParams = {
  filters?: DeliveriesQueryFilters
}

export function useDeliveriesCrud({ filters = {} }: UseDeliveriesCrudParams = {}) {
  const { showNotification } = useNotificationContext()
  const [detail, setDetail] = useState<DeliveryRow | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [actionRow, setActionRow] = useState<DeliveryRow | null>(null)
  const [promptAction, setPromptAction] = useState<PromptAction | null>(null)
  const [isSaving, setIsSaving] = useState(false)
  const [reloadKey, setReloadKey] = useState(0)

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
      deliveriesApi.fetchDeliveriesPage(pageIndex + 1, pageSize, keyword, filters),
    [filters],
  )

  const runAction = useCallback(
    async (_row: DeliveryRow, action: () => Promise<void>, successMessage: string) => {
      setIsSaving(true)
      try {
        await action()
        notifySuccess(successMessage)
        bumpReload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Thao tác thất bại'))
      } finally {
        setIsSaving(false)
      }
    },
    [notifySuccess, notifyError, bumpReload],
  )

  const openDetail = useCallback(
    async (row: DeliveryRow) => {
      setDetail(row)
      setDetailLoading(true)
      try {
        const fetchedDetail = await deliveriesApi.fetchDeliveryById(row.id)
        setDetail({
          ...fetchedDetail,
          items: fetchedDetail.items.length > 0 ? fetchedDetail.items : row.items,
        })
      } catch (error) {
        setDetail(null)
        notifyError(getErrorMessage(error, 'Không tải được chi tiết giao hàng'))
      } finally {
        setDetailLoading(false)
      }
    },
    [notifyError],
  )

  const handlers = useMemo<DeliveryTableHandlers>(
    () => ({
      onView: (row) => void openDetail(row),
      onProcess: (row) =>
        void runAction(row, () => deliveriesApi.processDelivery(row.id), 'Đã xử lý giao hàng'),
      onMarkDelivered: (row) => {
        setActionRow(row)
        setPromptAction('delivered')
      },
      onMarkFailed: (row) => {
        setActionRow(row)
        setPromptAction('failed')
      },
      onResendEsimEmail: (row) =>
        void runAction(row, () => deliveriesApi.resendDeliveryEsimEmail(row.id), 'Đã gửi lại email eSIM'),
    }),
    [openDetail, runAction],
  )

  const buildColumns = useCallback(() => buildDeliveryColumns(handlers), [handlers])

  const list = usePagedList({
    fetchPage,
    buildColumns,
    filterKey: `${filters.status ?? ''}`,
    reloadKey,
    emptyMessage: 'Chưa có giao hàng',
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
        if (promptAction === 'delivered') {
          await deliveriesApi.markDeliveryDelivered(actionRow.id, { note: value || undefined })
          notifySuccess('Đã đánh dấu giao thành công')
        } else {
          await deliveriesApi.markDeliveryFailed(actionRow.id, { error: value })
          notifySuccess('Đã đánh dấu giao hàng lỗi')
        }
        closePrompt()
        bumpReload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không cập nhật được trạng thái'))
      } finally {
        setIsSaving(false)
      }
    },
    [actionRow, promptAction, notifySuccess, notifyError, closePrompt, bumpReload],
  )

  return {
    list,
    openDetail,
    detail,
    detailLoading,
    closeDetail,
    promptAction,
    actionRow,
    isSaving,
    closePrompt,
    submitPrompt,
  }
}
