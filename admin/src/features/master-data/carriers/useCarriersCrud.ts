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
import type { CarrierTableHandlers } from '@/features/master-data/carriers/columns'
import * as carriersApi from '@/features/master-data/carriers/carriers.api'
import { buildCarrierFormConfig, getDefaultCarrierValues } from '@/features/master-data/carriers/formConfig'
import { fetchCountries } from '@/features/master-data/countries/countries.api'
import type { Carrier } from '@/features/master-data/types'
import { slugify } from '@/modules/crud/form/slugify'
import type { FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

type UseCarriersCrudParams = {
  buildColumns: (handlers: CarrierTableHandlers) => ColumnDef<Carrier>[]
  pageSize?: number
}

function applySlugFromName(values: Carrier): Carrier {
  if (!values.slug.trim() && values.name.trim()) {
    return { ...values, slug: slugify(values.name) }
  }
  return values
}

function toCreatePayload(values: Carrier): carriersApi.CarrierPayload {
  return {
    name: values.name.trim(),
    slug: values.slug.trim(),
    code: values.code.trim() || undefined,
    countryId: values.countryId.trim(),
    logoUrl: values.logoUrl.trim() || undefined,
    sortOrder: values.sortOrder,
  }
}

function toUpdatePayload(values: Carrier): carriersApi.CarrierUpdatePayload {
  return { ...toCreatePayload(values), isActive: values.isActive }
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function buildCountryLookups(countries: Awaited<ReturnType<typeof fetchCountries>>) {
  const countryNameById = new Map(countries.map((c) => [c.id, c.name]))
  const countryOptions: FormFieldOption[] = countries.map((c) => ({
    value: c.id,
    label: `${c.isoCode} ${c.name}`,
  }))
  return { countryNameById, countryOptions }
}

function enrichCountryNames(items: Carrier[], countryNameById: Map<string, string>): Carrier[] {
  return items.map((item) => ({
    ...item,
    countryName: countryNameById.get(item.countryId) ?? item.countryName,
  }))
}

export function useCarriersCrud({ buildColumns, pageSize = 10 }: UseCarriersCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<Carrier[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [countryOptions, setCountryOptions] = useState<FormFieldOption[]>([])
  const [countryNameById, setCountryNameById] = useState<Map<string, string>>(new Map())
  const [isLoadingLookups, setIsLoadingLookups] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<Carrier | null>(null)
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
  const countryNameByIdRef = useRef(countryNameById)
  countryNameByIdRef.current = countryNameById
  const countryLookupsLoadedRef = useRef(false)
  const countryOptionsRef = useRef<FormFieldOption[]>([])

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await carriersApi.fetchCarriersPage(
        pageIndex + 1,
        size,
        keyword || undefined,
        countryNameByIdRef.current,
      )
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách nhà mạng'))
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

  const loadCountryLookups = useCallback(async () => {
    if (countryLookupsLoadedRef.current) return

    setIsLoadingLookups(true)
    try {
      const countries = await fetchCountries()
      const lookups = buildCountryLookups(countries)
      countryLookupsLoadedRef.current = true
      countryOptionsRef.current = lookups.countryOptions
      countryNameByIdRef.current = lookups.countryNameById
      setCountryOptions(lookups.countryOptions)
      setCountryNameById(lookups.countryNameById)
      setData((prev) => enrichCountryNames(prev, lookups.countryNameById))
    } catch (e) {
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách quốc gia'))
      throw e
    } finally {
      setIsLoadingLookups(false)
    }
  }, [])

  useEffect(() => {
    void loadCountryLookups()
  }, [loadCountryLookups])

  const ensureCountryLookups = useCallback(async () => {
    await loadCountryLookups()
  }, [loadCountryLookups])

  const formConfig = useMemo(() => buildCarrierFormConfig(countryOptions), [countryOptions])

  const toggleActive = useCallback(
    async (row: Carrier) => {
      try {
        await carriersApi.updateCarrier(row.id, {
          ...toUpdatePayload(row),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã bật hiển thị nhà mạng' : 'Đã ẩn nhà mạng')
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

  const openCreate = useCallback(async () => {
    await ensureCountryLookups()
    setFormValues({
      ...getDefaultCarrierValues(),
      countryId: countryOptionsRef.current[0]?.value ?? '',
    })
    setFormMode('create')
  }, [ensureCountryLookups])

  const openEdit = useCallback(async (row: Carrier) => {
    await ensureCountryLookups()
    setFormValues({ ...row })
    setFormMode('edit')
  }, [ensureCountryLookups])

  const openView = useCallback((row: Carrier) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    async (rawValues: Carrier) => {
      if (!formMode || formMode === 'view') return

      let values = applySlugFromName(rawValues)
      const countryName = countryNameByIdRef.current.get(values.countryId) ?? ''
      values = { ...values, countryName }

      setIsSaving(true)
      try {
        if (formMode === 'create') {
          await carriersApi.createCarrier(toCreatePayload(values))
          notifySuccess('Đã thêm nhà mạng thành công')
          if (pagination.pageIndex === 0) {
            reload()
          } else {
            setPagination((p) => ({ ...p, pageIndex: 0 }))
          }
        } else {
          await carriersApi.updateCarrier(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật nhà mạng thành công')
          reload()
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được nhà mạng'))
      } finally {
        setIsSaving(false)
      }
    },
    [formMode, closeFormModal, reload, pagination.pageIndex, notifySuccess, notifyError],
  )

  const handlers = useMemo(
    () => ({
      onToggleActive: toggleActive,
      onDeleteRequest: requestDelete,
      onEdit: (row: Carrier) => void openEdit(row),
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
      await Promise.all([...pendingDeleteIds].map((id) => carriersApi.deleteCarrier(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1
          ? `Đã xóa ${pendingDeleteIds.size} nhà mạng`
          : 'Đã xóa nhà mạng thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được nhà mạng'))
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
    openCreate: () => void openCreate(),
    openView,
    closeFormModal,
    saveForm,
    formConfig,
    isLoading,
    isSaving,
    isLoadingLookups,
    reload,
  }
}
