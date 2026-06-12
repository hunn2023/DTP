import {
  type ColumnDef,
  getCoreRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import {
  filterFormKey,
  toAuditLogsQueryFilters,
  type AuditLogFilterForm,
} from '@/features/system/audit/auditFilterTypes'
import { fetchAuditLogsPage, type AuditLogRow } from '@/apis/auditLogsApi'
import { useNotificationContext } from '@/context/useNotificationContext'

const FILTER_DEBOUNCE_MS = 400

type UseAuditLogsListParams = {
  filterForm: AuditLogFilterForm
  buildColumns: () => ColumnDef<AuditLogRow>[]
  pageSize?: number
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useAuditLogsList({
  filterForm,
  buildColumns,
  pageSize = 10,
}: UseAuditLogsListParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<AuditLogRow[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [sorting, setSorting] = useState<SortingState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })

  const loadSeqRef = useRef(0)
  const filterFormRef = useRef(filterForm)
  filterFormRef.current = filterForm

  const notifyErrorRef = useRef((message: string) => {
    showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
  })
  notifyErrorRef.current = (message: string) => {
    showNotification({ title: 'Lỗi', message, variant: 'danger', delay: 4000 })
  }

  const filterKey = filterFormKey(filterForm)
  const [queryKey, setQueryKey] = useState(filterKey)

  useEffect(() => {
    if (filterKey === queryKey) return

    const timer = window.setTimeout(() => {
      setQueryKey(filterKey)
      setPagination((prev) => ({ ...prev, pageIndex: 0 }))
    }, FILTER_DEBOUNCE_MS)

    return () => window.clearTimeout(timer)
  }, [filterKey, queryKey])

  const loadData = useCallback(async (pageIndex: number, size: number, seq: number) => {
    setIsLoading(true)
    try {
      const result = await fetchAuditLogsPage(
        pageIndex + 1,
        size,
        toAuditLogsQueryFilters(filterFormRef.current),
      )
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (error) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(error, 'Không tải được nhật ký'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, seq)
  }, [pagination.pageIndex, pagination.pageSize, queryKey, loadData])

  const columns = useMemo(() => buildColumns(), [buildColumns])
  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const table = useReactTable({
    data,
    columns,
    pageCount,
    state: { sorting, pagination },
    onSortingChange: setSorting,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getRowId: (row) => String(row.id),
    manualPagination: true,
  })

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
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize,
    isLoading,
  }
}
