import {
  type ColumnDef,
  type ColumnFiltersState,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'
import type { PaymentProviderTableHandlers } from '@/features/master-data/payment-providers/columns'
import { paymentProviderFormConfig } from '@/features/master-data/payment-providers/formConfig'
import type { PaymentProvider } from '@/features/master-data/payment-providers/types'
import * as paymentProvidersApi from '@/apis/paymentProvidersApi'
import type { FormModalMode } from '@/modules/crud/form/types'

type UsePaymentProvidersCrudParams = {
  buildColumns: (handlers: PaymentProviderTableHandlers) => ColumnDef<PaymentProvider>[]
  pageSize?: number
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function parseOptionalAmount(value: number | null | undefined): number | null {
  if (value == null || Number.isNaN(value)) return null
  return value
}

export function usePaymentProvidersCrud({
  buildColumns,
  pageSize = 10,
}: UsePaymentProvidersCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<PaymentProvider[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<PaymentProvider | null>(null)
  const [isSaving, setIsSaving] = useState(false)

  const originalRef = useRef<PaymentProvider | null>(null)

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

  const notifyErrorRef = useRef(notifyError)
  notifyErrorRef.current = notifyError

  const loadData = useCallback(async () => {
    setIsLoading(true)
    try {
      const items = await paymentProvidersApi.fetchPaymentProviders()
      setData(items)
    } catch (e) {
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách cổng thanh toán'))
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void loadData()
  }, [loadData])

  const reload = useCallback(() => {
    void loadData()
  }, [loadData])

  const openEdit = useCallback((row: PaymentProvider) => {
    originalRef.current = { ...row }
    setFormValues({ ...row })
    setFormMode('edit')
  }, [])

  const openView = useCallback((row: PaymentProvider) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
    originalRef.current = null
  }, [])

  const toggleActive = useCallback(
    async (row: PaymentProvider) => {
      try {
        await paymentProvidersApi.setPaymentProviderActive(row.id, !row.isActive)
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã bật cổng thanh toán' : 'Đã tắt cổng thanh toán')
        if (row.isActive) reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không cập nhật được trạng thái'))
      }
    },
    [reload, notifySuccess, notifyError],
  )

  const saveForm = useCallback(
    async (values: PaymentProvider) => {
      if (formMode !== 'edit') return

      const original = originalRef.current
      if (!original) return

      const sortOrder = values.sortOrder
      const minAmount = parseOptionalAmount(values.minAmount)
      const maxAmount = parseOptionalAmount(values.maxAmount)
      const shouldSetDefault = values.isDefault && !original.isDefault

      setIsSaving(true)
      try {
        const updates: Array<() => Promise<void>> = []

        if (shouldSetDefault) {
          updates.push(() => paymentProvidersApi.setPaymentProviderDefault(values.id))
        }

        if (sortOrder !== original.sortOrder) {
          updates.push(() => paymentProvidersApi.updatePaymentProviderSortOrder(values.id, sortOrder))
        }

        if (minAmount !== original.minAmount || maxAmount !== original.maxAmount) {
          updates.push(() =>
            paymentProvidersApi.updatePaymentProviderLimits(values.id, minAmount, maxAmount),
          )
        }

        if (updates.length === 0) {
          notifySuccess('Không có thay đổi')
          closeFormModal()
          return
        }

        for (const update of updates) {
          await update()
        }
        notifySuccess('Đã cập nhật cổng thanh toán')
        closeFormModal()
        reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được cấu hình thanh toán'))
      } finally {
        setIsSaving(false)
      }
    },
    [formMode, closeFormModal, reload, notifySuccess, notifyError],
  )

  const handlers = useMemo(
    () => ({
      onToggleActive: toggleActive,
      onDeleteRequest: () => {},
      onEdit: (row: PaymentProvider) => void openEdit(row),
    }),
    [toggleActive, openEdit],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, columnFilters, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => String(row.id),
    globalFilterFn: 'includesString',
  })

  const paginationInfo = useMemo(() => {
    const filteredCount = table.getFilteredRowModel().rows.length
    const { pageIndex, pageSize: size } = pagination
    if (filteredCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + table.getRowModel().rows.length, filteredCount)
    return { start, end, total: filteredCount }
  }, [table, pagination])

  const setPageSize = useCallback((size: number) => {
    setPagination({ pageIndex: 0, pageSize: size })
  }, [])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    paginationInfo,
    pageCount: table.getPageCount(),
    pageSize: pagination.pageSize,
    setPageSize,
    formMode,
    formValues,
    openEdit,
    openView,
    closeFormModal,
    saveForm,
    formConfig: paymentProviderFormConfig,
    isLoading,
    isSaving,
    reload,
  }
}
