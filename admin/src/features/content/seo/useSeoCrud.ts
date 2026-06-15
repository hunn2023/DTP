import { getCoreRowModel, useReactTable } from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import * as seoMetadataApi from '@/apis/seoMetadataApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { SeoMetadata } from '@/features/content/types'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useSeoCrud(pageSize = 12) {
  const { showNotification } = useNotificationContext()
  const [items, setItems] = useState<SeoMetadata[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [entityTypeFilter, setEntityTypeFilter] = useState('')
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const loadSeqRef = useRef(0)

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

  const listFilters = useMemo(
    () => ({
      keyword: globalFilter || undefined,
      entityType: entityTypeFilter || undefined,
    }),
    [globalFilter, entityTypeFilter],
  )

  const loadData = useCallback(
    async (pageIndex: number, size: number, filters: seoMetadataApi.SeoListFilters, seq: number) => {
      setIsLoading(true)
      try {
        const result = await seoMetadataApi.fetchSeoMetadataPage(pageIndex + 1, size, filters)
        if (seq !== loadSeqRef.current) return
        setItems(result.items)
        setTotalCount(result.totalCount)
      } catch (e) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách SEO'))
      } finally {
        if (seq === loadSeqRef.current) setIsLoading(false)
      }
    },
    [],
  )

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, listFilters, seq)
  }, [pagination.pageIndex, pagination.pageSize, listFilters, loadData])

  const reload = useCallback(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, listFilters, seq)
  }, [loadData, pagination.pageIndex, pagination.pageSize, listFilters])

  const resetPageIndex = useCallback(() => {
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const setGlobalFilterAndReset = useCallback(
    (value: string) => {
      setGlobalFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const setEntityTypeFilterAndReset = useCallback(
    (value: string) => {
      setEntityTypeFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const requestDelete = useCallback((id: string) => {
    setPendingDeleteIds(new Set([id]))
    setShowDeleteModal(true)
  }, [])

  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const table = useReactTable({
    data: items,
    columns: [],
    pageCount,
    state: { pagination },
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
  })

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + items.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, items.length])

  const confirmDelete = useCallback(async () => {
    try {
      await Promise.all([...pendingDeleteIds].map((id) => seoMetadataApi.deleteSeoMetadata(id)))
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess('Đã xóa cấu hình SEO')
      reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được SEO'))
    }
  }, [pendingDeleteIds, reload, notifySuccess, notifyError])

  return {
    items,
    globalFilter,
    setGlobalFilter: setGlobalFilterAndReset,
    entityTypeFilter,
    setEntityTypeFilter: setEntityTypeFilterAndReset,
    requestDelete,
    table,
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize: (size: number) => setPagination({ pageIndex: 0, pageSize: size }),
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
    },
    confirmDelete,
    pendingDeleteCount: pendingDeleteIds.size,
    isLoading,
  }
}
