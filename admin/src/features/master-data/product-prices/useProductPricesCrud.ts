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
import type { PriceTableHandlers } from '@/features/master-data/product-prices/columns'
import {
  buildPriceFormConfig,
  getDefaultPriceValues,
} from '@/features/master-data/product-prices/formConfig'
import * as pricesApi from '@/features/master-data/product-prices/product-prices.api'
import { fetchProductOptions } from '@/features/master-data/products/products.api'
import type { ProductPriceRow } from '@/features/master-data/products/types'
import type { FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

type Params = {
  buildColumns: (handlers: PriceTableHandlers) => ColumnDef<ProductPriceRow>[]
  pageSize?: number
}

function toCreatePayload(values: ProductPriceRow): pricesApi.ProductPricePayload {
  return {
    productId: values.productId.trim(),
    productVariantId: values.productVariantId.trim() || null,
    currency: values.currency.trim() || 'VND',
    originalPrice: values.originalPrice,
    salePrice: values.salePrice,
    costPrice: values.costPrice,
    startDate: values.startDate.trim() || null,
    endDate: values.endDate.trim() || null,
  }
}

function toUpdatePayload(values: ProductPriceRow): pricesApi.ProductPriceUpdatePayload {
  return {
    currency: values.currency.trim() || 'VND',
    originalPrice: values.originalPrice,
    salePrice: values.salePrice,
    costPrice: values.costPrice,
    startDate: values.startDate.trim() || null,
    endDate: values.endDate.trim() || null,
    isActive: values.isActive,
  }
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductPricesCrud({ buildColumns, pageSize = 10 }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ProductPriceRow[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ProductPriceRow | null>(null)
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [productOptions, setProductOptions] = useState<FormFieldOption[]>([])
  const [variantOptions, setVariantOptions] = useState<FormFieldOption[]>([])

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
      const items = await pricesApi.fetchProductPrices()
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không tải được bảng giá'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [notifyError])

  useEffect(() => {
    void reload()
  }, [reload])

  const ensureLookups = useCallback(async () => {
    try {
      const [products, variants] = await Promise.all([
        fetchProductOptions(),
        import('@/features/master-data/products/product-variants.api').then((m) =>
          m.fetchAllProductVariantOptions(),
        ),
      ])
      setProductOptions(products)
      setVariantOptions(variants)
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không tải được dữ liệu tham chiếu'))
    }
  }, [notifyError])

  const formConfig = useMemo(
    () => buildPriceFormConfig(productOptions, variantOptions, formMode === 'edit'),
    [productOptions, variantOptions, formMode],
  )

  const toggleActive = useCallback(
    async (row: ProductPriceRow) => {
      try {
        await pricesApi.updateProductPrice(row.id, {
          ...toUpdatePayload(row),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã kích hoạt giá' : 'Đã tắt giá')
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
      onEdit: (row: ProductPriceRow) => {
        void ensureLookups()
        setFormValues({ ...row })
        setFormMode('edit')
      },
    }),
    [toggleActive, ensureLookups],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, rowSelection, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onRowSelectionChange: setRowSelection,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => row.id,
    globalFilterFn: 'includesString',
  })

  const paginationInfo = useMemo(() => {
    const filtered = table.getFilteredRowModel().rows.length
    const { pageIndex, pageSize: size } = pagination
    if (filtered === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + table.getRowModel().rows.length, filtered)
    return { start, end, total: filtered }
  }, [table, pagination])

  const saveForm = useCallback(
    async (values: ProductPriceRow) => {
      if (!formMode || formMode === 'view') return
      try {
        if (formMode === 'create') {
          await pricesApi.createProductPrice(toCreatePayload(values))
          notifySuccess('Đã thêm bản ghi giá')
        } else {
          await pricesApi.updateProductPrice(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật bản ghi giá')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được bản ghi giá'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const confirmDelete = useCallback(async () => {
    try {
      await Promise.all([...pendingDeleteIds].map((id) => pricesApi.deleteProductPrice(id)))
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
      setRowSelection({})
      notifySuccess('Đã xóa bản ghi giá')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được bản ghi giá'))
    }
  }, [pendingDeleteIds, notifySuccess, notifyError, reload])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    isLoading,
    paginationInfo,
    pageCount: table.getPageCount(),
    pageSize: pagination.pageSize,
    setPageSize: (size: number) => setPagination({ pageIndex: 0, pageSize: size }),
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
    },
    confirmDelete,
    pendingDeleteCount: pendingDeleteIds.size,
    requestBulkDelete: () => {
      setPendingDeleteIds(new Set(Object.keys(rowSelection)))
      setShowDeleteModal(true)
    },
    selectedCount: Object.keys(rowSelection).length,
    formMode,
    formValues,
    formConfig,
    openCreate: () => {
      void ensureLookups()
      setFormValues(getDefaultPriceValues())
      setFormMode('create')
    },
    openView: (row: ProductPriceRow) => {
      setFormValues({ ...row })
      setFormMode('view')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
  }
}
