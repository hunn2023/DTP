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

export type PagedResult<T> = {
  items: T[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

type UsePagedListParams<T extends { id: string | number }> = {
  fetchPage: (pageIndex: number, pageSize: number, keyword?: string) => Promise<PagedResult<T>>
  buildColumns: () => ColumnDef<T>[]
  pageSize?: number
  reloadKey?: string | number
  emptyMessage?: string
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function usePagedList<T extends { id: string | number }>({
  fetchPage,
  buildColumns,
  pageSize = 10,
  reloadKey = '',
  emptyMessage = 'Không có dữ liệu',
}: UsePagedListParams<T>) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<T[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })

  const loadSeqRef = useRef(0)
  const fetchPageRef = useRef(fetchPage)
  fetchPageRef.current = fetchPage

  const notifyError = useCallback(
    (message: string) => {
      showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
    },
    [showNotification],
  )

  const notifyErrorRef = useRef(notifyError)
  notifyErrorRef.current = notifyError

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await fetchPageRef.current(pageIndex, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (error) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(error, 'Không tải được dữ liệu'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [pagination.pageIndex, pagination.pageSize, globalFilter, reloadKey, loadData])

  const columns = useMemo(() => buildColumns(), [buildColumns])

  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const table = useReactTable({
    data,
    columns,
    pageCount,
    state: { sorting, globalFilter, columnFilters, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getRowId: (row) => String(row.id),
    manualPagination: true,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + data.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, data.length])

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
    isLoading,
    emptyMessage,
  }
}
