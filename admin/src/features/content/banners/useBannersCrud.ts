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

import * as contentBannersApi from '@/apis/contentBannersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { BannerTableHandlers } from '@/features/content/banners/columns'
import type { ContentBanner } from '@/features/content/types'

type UseBannersCrudParams = {
  buildColumns: (handlers: BannerTableHandlers) => ColumnDef<ContentBanner>[]
  onEdit: (row: ContentBanner) => void
  pageSize?: number
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useBannersCrud({ buildColumns, onEdit, pageSize = 10 }: UseBannersCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<ContentBanner[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())

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
      const result = await contentBannersApi.fetchContentBannersPage(pageIndex + 1, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách banner'))
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
    async (row: ContentBanner) => {
      try {
        const updated = row.isActive
          ? await contentBannersApi.disableContentBanner(row.id)
          : await contentBannersApi.enableContentBanner(row.id)
        setData((prev) => prev.map((item) => (item.id === row.id ? updated : item)))
        notifySuccess(updated.isActive ? 'Đã bật hiển thị banner' : 'Đã ẩn banner')
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

  const handlers = useMemo(
    () => ({
      onToggleActive: toggleActive,
      onDeleteRequest: requestDelete,
      onEdit,
    }),
    [toggleActive, requestDelete, onEdit],
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
      await Promise.all([...pendingDeleteIds].map((id) => contentBannersApi.deleteContentBanner(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1 ? `Đã xóa ${pendingDeleteIds.size} banner` : 'Đã xóa banner thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được banner'))
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
    isLoading,
    reload,
  }
}
