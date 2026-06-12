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
import type { CountryTableHandlers } from '@/features/master-data/countries/columns'
import * as countriesApi from '@/apis/countriesApi'
import type { Country } from '@/features/master-data/types'
import { slugify } from '@/modules/crud/form/slugify'
import type { EntityFormConfig, FormModalMode } from '@/modules/crud/form/types'

type UseCountriesCrudParams = {
  buildColumns: (handlers: CountryTableHandlers) => ColumnDef<Country>[]
  formConfig: EntityFormConfig<Country>
  pageSize?: number
}

type CountryFormTab = 'info' | 'flag'

function applySlugFromName(values: Country, formConfig: EntityFormConfig<Country>): Country {
  if (!formConfig.slugFromName) return values
  if (!values.slug.trim() && values.name.trim()) {
    return { ...values, slug: slugify(values.name) }
  }
  return values
}

function toPayload(values: Country): countriesApi.CountryCreatePayload {
  return {
    name: values.name.trim(),
    slug: values.slug.trim(),
    code: values.isoCode.trim(),
    region: values.region.trim() || undefined,
    description: values.description.trim() || undefined,
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useCountriesCrud({ buildColumns, formConfig, pageSize = 10 }: UseCountriesCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<Country[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<Country | null>(null)
  const [formTab, setFormTab] = useState<CountryFormTab>('info')
  const [createdCountryId, setCreatedCountryId] = useState<string | null>(null)
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

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await countriesApi.fetchCountriesPage(pageIndex + 1, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách quốc gia'))
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

  const resetFormState = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
    setFormTab('info')
    setCreatedCountryId(null)
  }, [])

  const closeFormModal = useCallback(() => {
    resetFormState()
  }, [resetFormState])

  const toggleActive = useCallback(
    async (row: Country) => {
      try {
        await countriesApi.updateCountry(row.id, {
          ...toPayload(row),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã bật hiển thị quốc gia' : 'Đã ẩn quốc gia')
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
    setFormValues(formConfig.getDefaultValues())
    setFormMode('create')
    setFormTab('info')
    setCreatedCountryId(null)
  }, [formConfig])

  const openEdit = useCallback((row: Country) => {
    setFormValues({ ...row })
    setFormMode('edit')
    setFormTab('info')
    setCreatedCountryId(null)
  }, [])

  const openView = useCallback((row: Country) => {
    setFormValues({ ...row })
    setFormMode('view')
    setFormTab('info')
    setCreatedCountryId(null)
  }, [])

  const continueCreate = useCallback(
    async (rawValues: Country) => {
      let values = applySlugFromName(rawValues, formConfig)
      if (formConfig.onBeforeSave) {
        values = formConfig.onBeforeSave(values, 'create')
      }

      setIsSaving(true)
      try {
        const id = await countriesApi.createCountry(toPayload(values))
        setCreatedCountryId(id)
        setFormValues({ ...values, id })
        setFormTab('flag')
        notifySuccess('Đã tạo quốc gia. Vui lòng tải lên cờ.')
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không tạo được quốc gia'))
      } finally {
        setIsSaving(false)
      }
    },
    [formConfig, notifySuccess, notifyError],
  )

  const saveCountryInfo = useCallback(
    async (rawValues: Country) => {
      let values = applySlugFromName(rawValues, formConfig)
      if (formConfig.onBeforeSave) {
        values = formConfig.onBeforeSave(values, 'edit')
      }

      setIsSaving(true)
      try {
        await countriesApi.updateCountry(values.id, toPayload(values))
        setFormValues(values)
        notifySuccess('Đã cập nhật quốc gia thành công')
        reload()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không cập nhật được quốc gia'))
      } finally {
        setIsSaving(false)
      }
    },
    [formConfig, reload, notifySuccess, notifyError],
  )

  const saveCountryFlag = useCallback(
    async (file: File) => {
      const countryId = formMode === 'create' ? createdCountryId : formValues?.id
      if (!countryId) {
        notifyError('Không xác định được quốc gia để tải cờ')
        return
      }

      setIsSaving(true)
      try {
        await countriesApi.uploadCountryFlag(countryId, file)
        notifySuccess('Đã tải lên cờ quốc gia thành công')
        if (pagination.pageIndex === 0) {
          reload()
        } else {
          setPagination((p) => ({ ...p, pageIndex: 0 }))
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không tải lên được cờ quốc gia'))
      } finally {
        setIsSaving(false)
      }
    },
    [formMode, createdCountryId, formValues?.id, pagination.pageIndex, reload, closeFormModal, notifySuccess, notifyError],
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
      await Promise.all([...pendingDeleteIds].map((id) => countriesApi.deleteCountry(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1 ? `Đã xóa ${pendingDeleteIds.size} quốc gia` : 'Đã xóa quốc gia thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được quốc gia'))
    }
  }, [pendingDeleteIds, data.length, pagination.pageIndex, reload, notifySuccess, notifyError])

  const closeDeleteModal = useCallback(() => {
    setShowDeleteModal(false)
    setPendingDeleteIds(new Set())
  }, [])

  const setPageSize = useCallback((size: number) => {
    setPagination({ pageIndex: 0, pageSize: size })
  }, [])

  const countryIdForFlag = formMode === 'create' ? createdCountryId : formValues?.id ?? null

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
    formTab,
    countryIdForFlag,
    openCreate,
    openView,
    closeFormModal,
    setFormTab,
    continueCreate,
    saveCountryInfo,
    saveCountryFlag,
    isLoading,
    isSaving,
    reload,
  }
}
