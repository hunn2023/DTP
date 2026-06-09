import {
  type ColumnDef,
  type ColumnFiltersState,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'
import * as categoriesApi from '@/features/master-data/categories/categories.api'
import type { Category } from '@/features/master-data/types'
import { slugify } from '@/modules/crud/form/slugify'
import type { EntityFormConfig, FormModalMode } from '@/modules/crud/form/types'
import type { CategoryTableHandlers } from '@/features/master-data/categories/columns'

type UseCategoriesCrudParams = {
  buildColumns: (handlers: CategoryTableHandlers) => ColumnDef<Category>[]
  formConfig: EntityFormConfig<Category>
  pageSize?: number
}

function applySlugFromName(values: Category, formConfig: EntityFormConfig<Category>): Category {
  if (!formConfig.slugFromName) return values
  if (!values.slug.trim() && values.name.trim()) {
    return { ...values, slug: slugify(values.name) }
  }
  return values
}

function toPayload(values: Category): categoriesApi.CategoryPayload {
  return {
    name: values.name.trim(),
    code: values.code.trim() || undefined,
    slug: values.slug.trim(),
    description: values.description.trim() || undefined,
    isActive: values.isActive,
    sortOrder: values.sortOrder,
  }
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useCategoriesCrud({ buildColumns, formConfig, pageSize = 10 }: UseCategoriesCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<Category[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<Category | null>(null)
  const [isSaving, setIsSaving] = useState(false)

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

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await categoriesApi.fetchCategoriesPage(pageIndex + 1, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách danh mục'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [pagination.pageIndex, pagination.pageSize, globalFilter, loadData])

  const reload = useCallback(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [loadData, pagination.pageIndex, pagination.pageSize, globalFilter])

  const toggleActive = useCallback(
    async (row: Category) => {
      try {
        const updated = await categoriesApi.updateCategory(row.id, {
          ...toPayload(row),
          isActive: !row.isActive,
        })
        setData((prev) => prev.map((item) => (item.id === row.id ? updated : item)))
        notifySuccess(updated.isActive ? 'Đã bật hiển thị danh mục' : 'Đã ẩn danh mục')
        if (!updated.isActive) reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không cập nhật được trạng thái'))
      }
    },
    [reload, notifySuccess, notifyError],
  )

  const requestDelete = useCallback((id: string) => {
    setPendingDeleteIds(new Set([id]))
    setShowDeleteModal(true)
  }, [])

  const openCreate = useCallback(() => {
    setFormValues(formConfig.getDefaultValues())
    setFormMode('create')
  }, [formConfig])

  const openEdit = useCallback((row: Category) => {
    setFormValues({ ...row })
    setFormMode('edit')
  }, [])

  const openView = useCallback((row: Category) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    async (rawValues: Category) => {
      if (!formMode || formMode === 'view') return

      let values = applySlugFromName(rawValues, formConfig)
      if (formConfig.onBeforeSave) {
        values = formConfig.onBeforeSave(values, formMode)
      }

      setIsSaving(true)
      try {
        const payload = toPayload(values)
        if (formMode === 'create') {
          await categoriesApi.createCategory(payload)
          notifySuccess('Đã thêm danh mục thành công')
          if (pagination.pageIndex === 0) {
            reload()
          } else {
            setPagination((p) => ({ ...p, pageIndex: 0 }))
          }
        } else {
          await categoriesApi.updateCategory(values.id, payload)
          notifySuccess('Đã cập nhật danh mục thành công')
          reload()
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được danh mục'))
      } finally {
        setIsSaving(false)
      }
    },
    [formMode, formConfig, closeFormModal, reload, pagination.pageIndex, notifySuccess, notifyError],
  )

  const handlers = useMemo(
    () => ({
      onToggleActive: toggleActive,
      onDeleteRequest: requestDelete,
      onEdit: openEdit,
    }),
    [toggleActive, requestDelete, openEdit],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const table = useReactTable({
    data,
    columns,
    pageCount,
    state: { sorting, globalFilter, columnFilters, pagination, rowSelection },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    onRowSelectionChange: setRowSelection,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getRowId: (row) => String(row.id),
    globalFilterFn: 'includesString',
    enableRowSelection: true,
    manualPagination: true,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + data.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, data.length])

  const requestBulkDelete = useCallback(() => {
    setPendingDeleteIds(new Set(Object.keys(rowSelection)))
    setShowDeleteModal(true)
  }, [rowSelection])

  const confirmDelete = useCallback(async () => {
    try {
      await Promise.all([...pendingDeleteIds].map((id) => categoriesApi.deleteCategory(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1 ? `Đã xóa ${pendingDeleteIds.size} danh mục` : 'Đã xóa danh mục thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được danh mục'))
    }
  }, [pendingDeleteIds, data.length, pagination.pageIndex, reload, notifySuccess, notifyError])

  const closeDeleteModal = useCallback(() => {
    setShowDeleteModal(false)
    setPendingDeleteIds(new Set())
  }, [])

  const setPageSize = useCallback((size: number) => {
    setPagination({ pageIndex: 0, pageSize: size })
  }, [])

  return {
    table,
    globalFilter,
    setGlobalFilter: setGlobalFilterAndReset,
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize,
    showDeleteModal,
    closeDeleteModal,
    confirmDelete,
    requestBulkDelete,
    selectedCount: Object.keys(rowSelection).length,
    pendingDeleteCount: pendingDeleteIds.size,
    formMode,
    formValues,
    openCreate,
    openView,
    closeFormModal,
    saveForm,
    formConfig,
    isLoading,
    isSaving,
    reload,
  }
}
