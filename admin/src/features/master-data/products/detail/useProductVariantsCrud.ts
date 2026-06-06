import {
  type ColumnDef,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'
import type { VariantTableHandlers } from '@/features/master-data/products/detail/variantColumns'
import {
  buildVariantFormConfig,
  getDefaultVariantValues,
} from '@/features/master-data/products/detail/variantFormConfig'
import * as variantsApi from '@/features/master-data/products/product-variants.api'
import type { ProductVariant } from '@/features/master-data/products/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  productId: string
  buildColumns: (handlers: VariantTableHandlers) => ColumnDef<ProductVariant>[]
}

function toPayload(values: ProductVariant): variantsApi.ProductVariantPayload {
  return {
    productId: values.productId,
    sku: values.sku.trim() || undefined,
    name: values.name.trim(),
    price: values.price,
    originalPrice: values.originalPrice,
    durationDays: values.durationDays,
    dataAmount: values.dataAmount,
    dataUnit: values.dataUnit.trim() || undefined,
    isUnlimited: values.isUnlimited,
    sortOrder: values.sortOrder,
  }
}

function toUpdatePayload(values: ProductVariant): variantsApi.ProductVariantUpdatePayload {
  const { productId: _, ...rest } = toPayload(values)
  return { ...rest, isActive: values.isActive }
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductVariantsCrud({ productId, buildColumns }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ProductVariant[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ProductVariant | null>(null)

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
      const items = await variantsApi.fetchProductVariants(productId)
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyError(getErrorMessage(e, 'Không tải được biến thể'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId, notifyError])

  useEffect(() => {
    void reload()
  }, [reload])

  const formConfig = useMemo(() => {
    const config = buildVariantFormConfig(productId)
    if (formMode === 'create') {
      return { ...config, fields: config.fields.filter((f) => f.name !== 'isActive') }
    }
    return config
  }, [productId, formMode])

  const toggleActive = useCallback(
    async (row: ProductVariant) => {
      try {
        await variantsApi.updateProductVariant(row.id, {
          ...toUpdatePayload(row),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã kích hoạt biến thể' : 'Đã tắt biến thể')
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không cập nhật được trạng thái'))
      }
    },
    [notifySuccess, notifyError],
  )

  const handlers = useMemo(
    () => ({
      onToggleActive: toggleActive,
      onDeleteRequest: (id: string) => {
        setPendingDeleteIds(new Set([id]))
        setShowDeleteModal(true)
      },
      onEdit: (row: ProductVariant) => {
        setFormValues({ ...row })
        setFormMode('edit')
      },
    }),
    [toggleActive],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, rowSelection, pagination: { pageIndex: 0, pageSize: 10 } },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onRowSelectionChange: setRowSelection,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => row.id,
    globalFilterFn: 'includesString',
  })

  const saveForm = useCallback(
    async (values: ProductVariant) => {
      if (!formMode || formMode === 'view') return
      try {
        if (formMode === 'create') {
          await variantsApi.createProductVariant(toPayload(values))
          notifySuccess('Đã thêm biến thể')
        } else {
          await variantsApi.updateProductVariant(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật biến thể')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được biến thể'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const confirmDelete = useCallback(async () => {
    try {
      await Promise.all([...pendingDeleteIds].map((id) => variantsApi.deleteProductVariant(id)))
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
      setRowSelection({})
      notifySuccess('Đã xóa biến thể')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được biến thể'))
    }
  }, [pendingDeleteIds, notifySuccess, notifyError, reload])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    isLoading,
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
    },
    confirmDelete,
    pendingDeleteCount: pendingDeleteIds.size,
    formMode,
    formValues,
    formConfig,
    openCreate: () => {
      setFormValues(getDefaultVariantValues(productId))
      setFormMode('create')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
  }
}
