import {
  type ColumnDef,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import * as faqsApi from '@/apis/productFaqsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { FaqTableHandlers } from '@/features/master-data/products/components/detail/faqColumns'
import {
  buildFaqFormConfig,
  getDefaultFaqValues,
} from '@/features/master-data/products/components/detail/faqFormConfig'
import type { ProductFaqRow } from '@/features/master-data/products/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  productId: string
  isTabActive: boolean
  buildColumns: (handlers: FaqTableHandlers) => ColumnDef<ProductFaqRow>[]
}

function toPayload(values: ProductFaqRow): faqsApi.ProductFaqPayload {
  return {
    productId: values.productId,
    question: values.question.trim(),
    answer: values.answer.trim(),
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

function toUpdatePayload(values: ProductFaqRow): faqsApi.ProductFaqUpdatePayload {
  const { productId: _, ...rest } = toPayload(values)
  return rest
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductFaqsCrud({ productId, isTabActive, buildColumns }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ProductFaqRow[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [isFormLoading, setIsFormLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ProductFaqRow | null>(null)
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteId, setPendingDeleteId] = useState('')

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

  const loadSeqRef = useRef(0)

  const reload = useCallback(async () => {
    const seq = ++loadSeqRef.current
    setIsLoading(true)
    try {
      const items = await faqsApi.fetchProductFaqsByProduct(productId)
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được FAQ'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId])

  useEffect(() => {
    if (!isTabActive) return
    void reload()
  }, [isTabActive, productId, reload])

  const formConfig = useMemo(() => buildFaqFormConfig(productId), [productId])

  const openEdit = useCallback(
    async (row: ProductFaqRow) => {
      setIsFormLoading(true)
      try {
        const detail = await faqsApi.fetchProductFaqById(row.id)
        if (!detail) {
          notifyError('Không tìm thấy FAQ')
          return
        }
        setFormValues(detail)
        setFormMode('edit')
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không tải được chi tiết FAQ'))
      } finally {
        setIsFormLoading(false)
      }
    },
    [notifyError],
  )

  const handlers = useMemo(
    () => ({
      onToggleActive: async () => {},
      onDeleteRequest: (id: string) => {
        setPendingDeleteId(id)
        setShowDeleteModal(true)
      },
      onEdit: (row: ProductFaqRow) => {
        void openEdit(row)
      },
    }),
    [openEdit],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getRowId: (row) => row.id,
    globalFilterFn: 'includesString',
  })

  const saveForm = useCallback(
    async (values: ProductFaqRow) => {
      if (!formMode) return
      try {
        if (formMode === 'create') {
          await faqsApi.createProductFaq(toPayload(values))
          notifySuccess('Đã thêm FAQ')
        } else {
          await faqsApi.updateProductFaq(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật FAQ')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được FAQ'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const confirmDelete = useCallback(async () => {
    if (!pendingDeleteId) return
    try {
      await faqsApi.deleteProductFaq(pendingDeleteId)
      setShowDeleteModal(false)
      setPendingDeleteId('')
      notifySuccess('Đã xóa FAQ')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được FAQ'))
    }
  }, [pendingDeleteId, notifySuccess, notifyError, reload])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    isLoading,
    isFormLoading,
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteId('')
    },
    confirmDelete,
    formMode,
    formValues,
    formConfig,
    openCreate: () => {
      setFormValues(getDefaultFaqValues(productId))
      setFormMode('create')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
  }
}
