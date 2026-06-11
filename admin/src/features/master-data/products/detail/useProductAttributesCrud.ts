import {
  type ColumnDef,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'
import type { AttributeTableHandlers } from '@/features/master-data/products/detail/attributeColumns'
import {
  buildAttributeFormConfig,
  getDefaultAttributeValues,
} from '@/features/master-data/products/detail/attributeFormConfig'
import * as attributesApi from '@/features/master-data/products/product-attributes.api'
import type { ProductAttributeRow } from '@/features/master-data/products/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  productId: string
  buildColumns: (handlers: AttributeTableHandlers) => ColumnDef<ProductAttributeRow>[]
}

function toPayload(values: ProductAttributeRow): attributesApi.ProductAttributePayload {
  return {
    productId: values.productId,
    key: values.key.trim(),
    displayName: values.displayName.trim() || undefined,
    value: values.value.trim(),
    sortOrder: values.sortOrder,
    isVisible: values.isActive,
  }
}

function toUpdatePayload(values: ProductAttributeRow): attributesApi.ProductAttributeUpdatePayload {
  const { productId: _, ...rest } = toPayload(values)
  return rest
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductAttributesCrud({ productId, buildColumns }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ProductAttributeRow[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ProductAttributeRow | null>(null)
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
      const items = await attributesApi.fetchProductAttributes(productId)
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được thuộc tính'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId])

  useEffect(() => {
    void reload()
  }, [productId, reload])

  const formConfig = useMemo(() => buildAttributeFormConfig(productId), [productId])

  const handlers = useMemo(
    () => ({
      onToggleActive: async () => {},
      onDeleteRequest: (id: string) => {
        setPendingDeleteId(id)
        setShowDeleteModal(true)
      },
      onEdit: (row: ProductAttributeRow) => {
        setFormValues({ ...row })
        setFormMode('edit')
      },
    }),
    [],
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
    async (values: ProductAttributeRow) => {
      if (!formMode) return
      try {
        if (formMode === 'create') {
          await attributesApi.createProductAttribute(toPayload(values))
          notifySuccess('Đã thêm thuộc tính')
        } else {
          await attributesApi.updateProductAttribute(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật thuộc tính')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được thuộc tính'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const confirmDelete = useCallback(async () => {
    if (!pendingDeleteId) return
    try {
      await attributesApi.deleteProductAttribute(pendingDeleteId)
      setShowDeleteModal(false)
      setPendingDeleteId('')
      notifySuccess('Đã xóa thuộc tính')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được thuộc tính'))
    }
  }, [pendingDeleteId, notifySuccess, notifyError, reload])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    isLoading,
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
      setFormValues(getDefaultAttributeValues(productId))
      setFormMode('create')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
  }
}
