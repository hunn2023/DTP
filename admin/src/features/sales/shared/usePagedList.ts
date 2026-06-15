import {
  type ColumnDef,
  type ColumnFiltersState,
  type PaginationState,
  type Updater,
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
  filterKey?: string | number
  emptyMessage?: string
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function resolvePaginationState(
  updater: Updater<PaginationState>,
  previous: PaginationState,
): PaginationState {
  return typeof updater === 'function' ? updater(previous) : updater
}

function buildQuerySignature(
  pageIndex: number,
  pageSize: number,
  keyword: string,
  reloadKey: string | number,
  filterKey: string | number,
): string {
  return `${pageIndex}|${pageSize}|${keyword}|${reloadKey}|${filterKey}`
}

const inFlightRequests = new Map<string, Promise<PagedResult<unknown>>>()

export function usePagedList<T extends { id: string | number }>({
  fetchPage,
  buildColumns,
  pageSize = 10,
  reloadKey = '',
  filterKey = '',
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
  const prevFilterKeyRef = useRef(filterKey)

  const notifyError = useCallback(
    (message: string) => {
      showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
    },
    [showNotification],
  )

  const notifyErrorRef = useRef(notifyError)
  notifyErrorRef.current = notifyError

  const querySignature = useMemo(
    () =>
      buildQuerySignature(
        pagination.pageIndex,
        pagination.pageSize,
        globalFilter,
        reloadKey,
        filterKey,
      ),
    [pagination.pageIndex, pagination.pageSize, globalFilter, reloadKey, filterKey],
  )

  const loadData = useCallback(
    async (
      signature: string,
      pageIndex: number,
      size: number,
      keyword: string,
      seq: number,
    ) => {
      setIsLoading(true)
      try {
        let request = inFlightRequests.get(signature) as Promise<PagedResult<T>> | undefined

        if (!request) {
          request = fetchPageRef.current(pageIndex, size, keyword || undefined)
          inFlightRequests.set(signature, request as Promise<PagedResult<unknown>>)
          void request.finally(() => {
            if (inFlightRequests.get(signature) === request) {
              inFlightRequests.delete(signature)
            }
          })
        }

        const result = await request
        if (seq !== loadSeqRef.current) return
        setData(result.items)
        setTotalCount(result.totalCount)
      } catch (error) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(error, 'Không tải được dữ liệu'))
      } finally {
        if (seq === loadSeqRef.current) setIsLoading(false)
      }
    },
    [],
  )

  useEffect(() => {
    const filterChanged = prevFilterKeyRef.current !== filterKey
    if (filterChanged) {
      prevFilterKeyRef.current = filterKey
      if (pagination.pageIndex !== 0) {
        setPagination((prev) => ({ ...prev, pageIndex: 0 }))
        return
      }
    }

    const seq = ++loadSeqRef.current
    void loadData(
      querySignature,
      pagination.pageIndex,
      pagination.pageSize,
      globalFilter,
      seq,
    )
  }, [querySignature, filterKey, pagination.pageIndex, pagination.pageSize, globalFilter, loadData])

  const columns = useMemo(() => buildColumns(), [buildColumns])

  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const onPaginationChange = useCallback((updater: Updater<PaginationState>) => {
    setPagination((prev) => {
      const next = resolvePaginationState(updater, prev)
      if (next.pageIndex === prev.pageIndex && next.pageSize === prev.pageSize) {
        return prev
      }
      return next
    })
  }, [])

  const table = useReactTable({
    data,
    columns,
    pageCount,
    state: { sorting, globalFilter, columnFilters, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getRowId: (row) => String(row.id),
    manualPagination: true,
    autoResetPageIndex: false,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((prev) => (prev.pageIndex === 0 ? prev : { ...prev, pageIndex: 0 }))
  }, [])

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + data.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, data.length])

  const setPageSize = useCallback((size: number) => {
    setPagination((prev) =>
      prev.pageIndex === 0 && prev.pageSize === size ? prev : { pageIndex: 0, pageSize: size },
    )
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
