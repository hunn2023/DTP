import {
  type ColumnDef,
  type ColumnFiltersState,
  getCoreRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'
import { useNavigate } from 'react-router'

import { useNotificationContext } from '@/context/useNotificationContext'
import type { EsimPackageTableHandlers } from '@/features/products/esim-packages/columns'
import * as esimPackagesApi from '@/features/products/esim-packages/esim-packages.api'
import {
  fetchEsimFilterOptions,
  fetchEsimPackageLookups,
  type EsimPackageLookups,
} from '@/features/products/esim-packages/lookups.api'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import {
  mapPackageToForm,
  toPackagePayload,
} from '@/features/products/esim-wizard/mapPackageForm'
import {
  activeFilterToBool,
  type ActiveFilterValue,
} from '@/modules/crud/components/ActiveFilterSelect'
import type { FormFieldOption } from '@/modules/crud/form/types'

type UseEsimPackagesCrudParams = {
  buildColumns: (handlers: EsimPackageTableHandlers) => ColumnDef<EsimPackage>[]
  pageSize?: number
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useEsimPackagesCrud({ buildColumns, pageSize = 10 }: UseEsimPackagesCrudParams) {
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<EsimPackage[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [filterOptions, setFilterOptions] = useState<Pick<EsimPackageLookups, 'countryOptions' | 'carrierOptions'>>({
    countryOptions: [],
    carrierOptions: [],
  })
  const [variantFilterOptions, setVariantFilterOptions] = useState<FormFieldOption[]>([])
  const [filtersReady, setFiltersReady] = useState(false)
  const [countryFilter, setCountryFilter] = useState('')
  const [carrierFilter, setCarrierFilter] = useState('')
  const [variantFilter, setVariantFilter] = useState('')
  const [activeFilter, setActiveFilter] = useState<ActiveFilterValue>('all')
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

  useEffect(() => {
    void fetchEsimFilterOptions()
      .then(setFilterOptions)
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được bộ lọc'))
      })
      .finally(() => setFiltersReady(true))
  }, [])

  const listFilters = useMemo(
    () => ({
      keyword: globalFilter || undefined,
      countryId: countryFilter || undefined,
      carrierId: carrierFilter || undefined,
      productVariantId: variantFilter || undefined,
      isActive: activeFilterToBool(activeFilter),
    }),
    [globalFilter, countryFilter, carrierFilter, variantFilter, activeFilter],
  )

  const loadData = useCallback(
    async (pageIndex: number, size: number, filters: esimPackagesApi.EsimPackageListFilters, seq: number) => {
      setIsLoading(true)
      try {
        const result = await esimPackagesApi.fetchEsimPackagesPage(pageIndex + 1, size, filters)
        if (seq !== loadSeqRef.current) return
        setData(result.items)
        setTotalCount(result.totalCount)
      } catch (e) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách gói eSIM'))
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

  const toggleActive = useCallback(
    async (row: EsimPackage) => {
      try {
        await esimPackagesApi.updateEsimPackage(row.id, {
          ...toPackagePayload(mapPackageToForm(row)),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã bật hiển thị gói eSIM' : 'Đã ẩn gói eSIM')
        if (row.isActive) reload()
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
    navigate('/products/esim/wizard/new')
  }, [navigate])

  const openEdit = useCallback(
    (row: EsimPackage) => {
      navigate(
        `/products/esim/wizard/${row.productVariantId}?productId=${row.productId}&packageId=${row.id}`,
      )
    },
    [navigate],
  )

  const openView = useCallback(
    (row: EsimPackage) => {
      navigate(
        `/products/esim/wizard/${row.productVariantId}?tab=review&productId=${row.productId}&packageId=${row.id}`,
      )
    },
    [navigate],
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
    getRowId: (row) => String(row.id),
    enableRowSelection: true,
    manualPagination: true,
    manualFiltering: true,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const setCountryFilterAndReset = useCallback((value: string) => {
    setCountryFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const setCarrierFilterAndReset = useCallback((value: string) => {
    setCarrierFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const setVariantFilterAndReset = useCallback((value: string) => {
    setVariantFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const setActiveFilterAndReset = useCallback((value: ActiveFilterValue) => {
    setActiveFilter(value)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [])

  const loadVariantFilterOptions = useCallback(async () => {
    if (variantFilterOptions.length > 0) return
    try {
      const loaded = await fetchEsimPackageLookups()
      setVariantFilterOptions(loaded.productVariantOptions)
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không tải được biến thể'))
    }
  }, [variantFilterOptions.length, notifyError])

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
      await Promise.all([...pendingDeleteIds].map((id) => esimPackagesApi.deleteEsimPackage(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1
          ? `Đã xóa ${pendingDeleteIds.size} gói eSIM`
          : 'Đã xóa gói eSIM thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được gói eSIM'))
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
    openCreate,
    openView,
    isLoading,
    filtersReady,
    filterOptions,
    variantFilterOptions,
    countryFilter,
    setCountryFilter: setCountryFilterAndReset,
    carrierFilter,
    setCarrierFilter: setCarrierFilterAndReset,
    variantFilter,
    setVariantFilter: setVariantFilterAndReset,
    activeFilter,
    setActiveFilter: setActiveFilterAndReset,
    loadVariantFilterOptions,
    reload,
  }
}
