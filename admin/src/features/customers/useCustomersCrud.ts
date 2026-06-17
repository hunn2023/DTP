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

import * as customersApi from '@/apis/customersApi'
import type { CustomerDetail, CustomerRow } from '@/apis/customersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { CustomerTableHandlers } from '@/features/customers/columns'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import {
  activeFilterToBool,
  type ActiveFilterValue,
} from '@/modules/crud/components/ActiveFilterSelect'
type Params = {
  buildColumns: (handlers: CustomerTableHandlers) => ColumnDef<CustomerRow>[]
  pageSize?: number
  fixedIsActive?: boolean
}

export function useCustomersCrud({ buildColumns, pageSize = 20, fixedIsActive }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<CustomerRow[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [activeFilter, setActiveFilter] = useState<ActiveFilterValue>('all')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [detail, setDetail] = useState<CustomerDetail | null>(null)
  const [detailLoading, setDetailLoading] = useState(false)
  const [statusUpdating, setStatusUpdating] = useState(false)

  const loadSeqRef = useRef(0)

  const isActiveQuery = fixedIsActive !== undefined ? fixedIsActive : activeFilterToBool(activeFilter)
  const filterKey = `${fixedIsActive ?? activeFilter}`

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

  const loadData = useCallback(
    async (pageIndex: number, size: number, keyword: string, seq: number) => {
      setIsLoading(true)
      try {
        const result = await customersApi.fetchCustomersPage(pageIndex + 1, size, {
          keyword: keyword || undefined,
          isActive: isActiveQuery,
        })
        if (seq !== loadSeqRef.current) return
        setData(result.items)
        setTotalCount(result.totalCount)
      } catch (error) {
        if (seq !== loadSeqRef.current) return
        notifyError(getErrorMessage(error, 'Không tải được danh sách khách hàng'))
      } finally {
        if (seq === loadSeqRef.current) setIsLoading(false)
      }
    },
    [isActiveQuery, notifyError],
  )

  const reload = useCallback(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [loadData, pagination.pageIndex, pagination.pageSize, globalFilter])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [pagination.pageIndex, pagination.pageSize, globalFilter, filterKey, loadData])

  const openView = useCallback(
    async (row: CustomerRow) => {
      setDetailLoading(true)
      try {
        const customer = await customersApi.fetchCustomerDetail(row.userId)
        setDetail(customer)
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không tải được chi tiết khách hàng'))
      } finally {
        setDetailLoading(false)
      }
    },
    [notifyError],
  )

  const toggleLock = useCallback(
    async (row: CustomerRow) => {
      setStatusUpdating(true)
      try {
        if (row.isActive) {
          await customersApi.lockCustomer(row.userId)
          notifySuccess('Đã khóa khách hàng')
        } else {
          await customersApi.unlockCustomer(row.userId)
          notifySuccess('Đã mở khóa khách hàng')
        }
        reload()
        if (detail?.userId === row.userId) {
          setDetail((prev) =>
            prev ? { ...prev, isActive: !row.isActive, status: row.isActive ? 'Locked' : 'Active' } : prev,
          )
        }
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không cập nhật được trạng thái khách hàng'))
      } finally {
        setStatusUpdating(false)
      }
    },
    [detail?.userId, notifySuccess, notifyError, reload],
  )

  const handlers = useMemo<CustomerTableHandlers>(
    () => ({
      onView: (row) => {
        void openView(row)
      },
      onToggleLock: (row) => {
        void toggleLock(row)
      },
    }),
    [openView, toggleLock],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])
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
    getRowId: (row) => row.userId,
    manualPagination: true,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const setActiveFilterAndReset = useCallback((value: ActiveFilterValue) => {
    setActiveFilter(value)
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min((pageIndex + 1) * size, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount])

  const setPageSize = useCallback((size: number) => {
    setPagination({ pageIndex: 0, pageSize: size })
  }, [])

  const closeDetail = useCallback(() => {
    setDetail(null)
  }, [])

  return {
    table,
    isLoading,
    globalFilter,
    setGlobalFilter: setGlobalFilterAndReset,
    activeFilter,
    setActiveFilter: setActiveFilterAndReset,
    showActiveFilter: fixedIsActive === undefined,
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize,
    detail,
    detailLoading,
    statusUpdating,
    closeDetail,
    openView,
    toggleLock,
  }
}
