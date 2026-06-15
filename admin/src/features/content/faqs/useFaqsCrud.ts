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

import * as contentFaqsApi from '@/apis/contentFaqsApi'
import { fetchCategoryCodeLookup } from '@/apis/categoriesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { FaqTableHandlers } from '@/features/content/faqs/columns'
import { buildFaqFormConfig } from '@/features/content/faqs/formConfig'
import type { ContentFaq } from '@/features/content/types'
import type { FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

type UseFaqsCrudParams = {
  buildColumns: (
    handlers: FaqTableHandlers,
    categoryNameByCode?: Map<string, string>,
  ) => ColumnDef<ContentFaq>[]
  pageSize?: number
}
function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function toPayload(values: ContentFaq): contentFaqsApi.ContentFaqPayload {
  return {
    question: values.question.trim(),
    answer: values.answer.trim(),
    categoryCode: values.categoryCode.trim() || undefined,
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

export function useFaqsCrud({
  buildColumns,
  pageSize = 10,
}: UseFaqsCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ContentFaq[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [categoryOptions, setCategoryOptions] = useState<FormFieldOption[]>([])
  const [categoryNameByCode, setCategoryNameByCode] = useState<Map<string, string>>(new Map())
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<ContentFaq | null>(null)
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

  const formConfig = useMemo(() => buildFaqFormConfig(categoryOptions), [categoryOptions])

  useEffect(() => {
    void fetchCategoryCodeLookup(1, 10)
      .then(({ options, nameByCode }) => {
        setCategoryOptions(options)
        setCategoryNameByCode(nameByCode)
      })
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh mục'))
      })
  }, [])

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await contentFaqsApi.fetchContentFaqsPage(pageIndex + 1, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách FAQ'))
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
    async (row: ContentFaq) => {
      try {
        const updated = row.isActive
          ? await contentFaqsApi.disableContentFaq(row.id)
          : await contentFaqsApi.enableContentFaq(row.id)
        setData((prev) => prev.map((item) => (item.id === row.id ? updated : item)))
        notifySuccess(updated.isActive ? 'Đã bật hiển thị FAQ' : 'Đã ẩn FAQ')
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

  const openEdit = useCallback((row: ContentFaq) => {
    setFormValues({ ...row })
    setFormMode('edit')
  }, [])

  const openView = useCallback((row: ContentFaq) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    async (rawValues: ContentFaq) => {
      if (!formMode || formMode === 'view') return

      let values = rawValues
      if (formConfig.onBeforeSave) {
        values = formConfig.onBeforeSave(values, formMode)
      }

      setIsSaving(true)
      try {
        const payload = toPayload(values)
        if (formMode === 'create') {
          await contentFaqsApi.createContentFaq(payload)
          notifySuccess('Đã thêm FAQ thành công')
          if (pagination.pageIndex === 0) reload()
          else setPagination((p) => ({ ...p, pageIndex: 0 }))
        } else {
          await contentFaqsApi.updateContentFaq(values.id, payload)
          notifySuccess('Đã cập nhật FAQ thành công')
          reload()
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được FAQ'))
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

  const columns = useMemo(
    () => buildColumns(handlers, categoryNameByCode),
    [buildColumns, handlers, categoryNameByCode],
  )

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
      await Promise.all([...pendingDeleteIds].map((id) => contentFaqsApi.deleteContentFaq(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1 ? `Đã xóa ${pendingDeleteIds.size} FAQ` : 'Đã xóa FAQ thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được FAQ'))
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
