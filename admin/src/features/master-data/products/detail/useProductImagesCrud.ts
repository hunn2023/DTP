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
import type { ImageTableHandlers } from '@/features/master-data/products/detail/imageColumns'
import {
  buildImageFormConfig,
  getDefaultImageValues,
} from '@/features/master-data/products/detail/imageFormConfig'
import * as imagesApi from '@/features/master-data/products/product-images.api'
import type { ProductImageRow } from '@/features/master-data/products/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  productId: string
  buildColumns: (handlers: ImageTableHandlers) => ColumnDef<ProductImageRow>[]
}

function toPayload(values: ProductImageRow): imagesApi.ProductImagePayload {
  return {
    productId: values.productId,
    imageUrl: values.imageUrl.trim(),
    altText: values.altText.trim() || undefined,
    sortOrder: values.sortOrder,
    isThumbnail: values.isThumbnail,
  }
}

function toUpdatePayload(values: ProductImageRow): imagesApi.ProductImageUpdatePayload {
  const { productId: _, ...rest } = toPayload(values)
  return rest
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductImagesCrud({ productId, buildColumns }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ProductImageRow[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ProductImageRow | null>(null)
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

  const loadSeqRef = useRef(0)

  const reload = useCallback(async () => {
    const seq = ++loadSeqRef.current
    setIsLoading(true)
    try {
      const items = await imagesApi.fetchProductImages(productId)
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không tải được hình ảnh'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId, notifyError])

  useEffect(() => {
    void reload()
  }, [reload])

  const formConfig = useMemo(() => buildImageFormConfig(productId), [productId])

  const handlers = useMemo(
    () => ({
      onToggleActive: async () => {},
      onDeleteRequest: (id: string) => {
        setPendingDeleteId(id)
        setShowDeleteModal(true)
      },
      onEdit: (row: ProductImageRow) => {
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
    async (values: ProductImageRow) => {
      if (!formMode) return
      try {
        if (formMode === 'create') {
          await imagesApi.createProductImage(toPayload(values))
          notifySuccess('Đã thêm hình ảnh')
        } else {
          await imagesApi.updateProductImage(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật hình ảnh')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được hình ảnh'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const confirmDelete = useCallback(async () => {
    if (!pendingDeleteId) return
    try {
      await imagesApi.deleteProductImage(pendingDeleteId)
      setShowDeleteModal(false)
      setPendingDeleteId('')
      notifySuccess('Đã xóa hình ảnh')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được hình ảnh'))
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
      setFormValues(getDefaultImageValues(productId))
      setFormMode('create')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
  }
}
