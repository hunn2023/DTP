import {
  type ColumnDef,
  type ColumnFiltersState,
  getCoreRowModel,
  getFilteredRowModel,
  getPaginationRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'

type UseFetchedListParams<T extends { id: string | number }> = {
  fetchAll: () => Promise<T[]>
  buildColumns: () => ColumnDef<T>[]
  pageSize?: number
  reloadKey?: string | number
  emptyMessage?: string
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useFetchedList<T extends { id: string | number }>({
  fetchAll,
  buildColumns,
  pageSize = 10,
  reloadKey = '',
  emptyMessage = 'Không có dữ liệu',
}: UseFetchedListParams<T>) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<T[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })

  const loadSeqRef = useRef(0)
  const fetchAllRef = useRef(fetchAll)
  fetchAllRef.current = fetchAll

  const notifyErrorRef = useRef((message: string) => {
    showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
  })
  notifyErrorRef.current = (message: string) => {
    showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
  }

  const loadData = useCallback(async (seq: number) => {
    setIsLoading(true)
    try {
      const items = await fetchAllRef.current()
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (error) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(error, 'Không tải được dữ liệu'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(seq)
  }, [reloadKey, loadData])

  const columns = useMemo(() => buildColumns(), [buildColumns])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, columnFilters, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => String(row.id),
    globalFilterFn: 'includesString',
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const filteredTotal = table.getFilteredRowModel().rows.length
  const pageCount = Math.max(1, Math.ceil(filteredTotal / pagination.pageSize))

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (filteredTotal === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(start + size - 1, filteredTotal)
    return { start, end, total: filteredTotal }
  }, [pagination, filteredTotal])

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
