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
import { fetchCategoryOptions } from '@/features/master-data/categories/categories.api'
import { fetchCarrierOptions } from '@/features/master-data/carriers/carriers.api'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import type { ProductTableHandlers } from '@/features/master-data/products/columns'
import { toProductPayload } from '@/features/master-data/products/formConfig'
import * as productsApi from '@/features/master-data/products/products.api'
import type { CatalogProduct } from '@/features/master-data/products/types'
import {
  activeFilterToBool,
  type ActiveFilterValue,
} from '@/modules/crud/components/ActiveFilterSelect'

type UseProductsCrudParams = {
  buildColumns: (handlers: ProductTableHandlers) => ColumnDef<CatalogProduct>[]
  pageSize?: number
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductsCrud({ buildColumns, pageSize = 10 }: UseProductsCrudParams) {
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<CatalogProduct[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [categoryOptions, setCategoryOptions] = useState<{ value: string; label: string }[]>([])
  const [countryFilterOptions, setCountryFilterOptions] = useState<{ value: string; label: string }[]>([])
  const [carrierFilterOptions, setCarrierFilterOptions] = useState<{ value: string; label: string }[]>([])
  const [categoryFilter, setCategoryFilter] = useState('')
  const [countryFilter, setCountryFilter] = useState('')
  const [carrierFilter, setCarrierFilter] = useState('')
  const [activeFilter, setActiveFilter] = useState<ActiveFilterValue>('all')

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
    void Promise.all([fetchCategoryOptions(), fetchCountries(), fetchCarrierOptions()])
      .then(([categories, countries, carriers]) => {
        setCategoryOptions(categories)
        setCountryFilterOptions(
          countries.map((item) => ({
            value: item.id,
            label: `${item.isoCode} ${item.name}`,
          })),
        )
        setCarrierFilterOptions(carriers)
      })
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được bộ lọc'))
      })
  }, [])

  const categoryNameById = useMemo(
    () => new Map(categoryOptions.map((item) => [item.value, item.label])),
    [categoryOptions],
  )
  const categoryNameByIdRef = useRef(categoryNameById)
  categoryNameByIdRef.current = categoryNameById

  const listFilters = useMemo(
    () => ({
      keyword: globalFilter || undefined,
      categoryId: categoryFilter || undefined,
      countryId: countryFilter || undefined,
      carrierId: carrierFilter || undefined,
      isActive: activeFilterToBool(activeFilter),
    }),
    [globalFilter, categoryFilter, countryFilter, carrierFilter, activeFilter],
  )

  const loadData = useCallback(
    async (pageIndex: number, size: number, filters: productsApi.ProductListFilters, seq: number) => {
      setIsLoading(true)
      try {
        const result = await productsApi.fetchProductsPage(pageIndex + 1, size, filters)
        if (seq !== loadSeqRef.current) return
        setData(
          result.items.map((item) => ({
            ...item,
            categoryName:
              item.categoryName || categoryNameByIdRef.current.get(item.categoryId) || '',
          })),
        )
        setTotalCount(result.totalCount)
      } catch (e) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách sản phẩm'))
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

  useEffect(() => {
    if (categoryOptions.length === 0) return
    setData((prev) =>
      prev.map((item) => ({
        ...item,
        categoryName: item.categoryName || categoryNameById.get(item.categoryId) || '',
      })),
    )
  }, [categoryOptions, categoryNameById])

  const reload = useCallback(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, listFilters, seq)
  }, [loadData, pagination.pageIndex, pagination.pageSize, listFilters])

  const toggleActive = useCallback(
    async (row: CatalogProduct) => {
      try {
        await productsApi.updateProduct(row.id, {
          ...toProductPayload(row, false),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã kích hoạt sản phẩm' : 'Đã tắt sản phẩm')
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
    navigate('/settings/products/new')
  }, [navigate])

  const openEdit = useCallback(
    (row: CatalogProduct) => {
      navigate(`/settings/products/${row.id}`)
    },
    [navigate],
  )

  const openView = useCallback(
    (row: CatalogProduct) => {
      navigate(`/settings/products/${row.id}`)
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

  const setCategoryFilterAndReset = useCallback((value: string) => {
    setCategoryFilter(value)
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

  const setActiveFilterAndReset = useCallback((value: ActiveFilterValue) => {
    setActiveFilter(value)
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
      await Promise.all([...pendingDeleteIds].map((id) => productsApi.deleteProduct(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1
          ? `Đã xóa ${pendingDeleteIds.size} sản phẩm`
          : 'Đã xóa sản phẩm thành công',
      )
      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được sản phẩm'))
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
    categoryFilterOptions: categoryOptions,
    countryFilterOptions,
    carrierFilterOptions,
    categoryFilter,
    setCategoryFilter: setCategoryFilterAndReset,
    countryFilter,
    setCountryFilter: setCountryFilterAndReset,
    carrierFilter,
    setCarrierFilter: setCarrierFilterAndReset,
    activeFilter,
    setActiveFilter: setActiveFilterAndReset,
    reload,
  }
}
