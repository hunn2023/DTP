import {
  type ColumnDef,
  type ColumnFiltersState,
  getCoreRowModel,
  getSortedRowModel,
  type SortingState,
  useReactTable,
} from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { useNotificationContext } from '@/context/useNotificationContext'
import type { PhoneCardTableHandlers } from '@/features/products/phone-cards/columns'
import { buildPhoneCardFormConfig, getDefaultPhoneCardValues } from '@/features/products/phone-cards/formConfig'
import {
  fetchPhoneCardFilterOptions,
  fetchPhoneCardLookups,
  type PhoneCardLookups,
} from '@/features/products/phone-cards/lookups.api'
import * as phoneCardsApi from '@/features/products/phone-cards/phone-cards.api'
import type { PhoneCard } from '@/features/products/phone-cards/types'
import {
  activeFilterToBool,
  type ActiveFilterValue,
} from '@/modules/crud/components/ActiveFilterSelect'
import { slugify } from '@/modules/crud/form/slugify'
import type { FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

type UsePhoneCardsCrudParams = {
  buildColumns: (handlers: PhoneCardTableHandlers) => ColumnDef<PhoneCard>[]
  pageSize?: number
}

const emptyLookups: PhoneCardLookups = {
  productVariantOptions: [],
  providerOptions: [],
}

function applySlugFromName(values: PhoneCard): PhoneCard {
  if (!values.slug.trim() && values.name.trim()) {
    return { ...values, slug: slugify(values.name) }
  }
  return values
}

function toCreatePayload(values: PhoneCard): phoneCardsApi.PhoneCardCreatePayload {
  return {
    productVariantId: values.productVariantId.trim(),
    providerId: values.providerId.trim(),
    name: values.name.trim(),
    slug: values.slug.trim(),
    faceValue: values.faceValue,
    price: values.price,
    currency: values.currency.trim(),
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

function toUpdatePayload(values: PhoneCard): phoneCardsApi.PhoneCardUpdatePayload {
  const { productVariantId: _ignored, ...payload } = toCreatePayload(values)
  return payload
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function usePhoneCardsCrud({ buildColumns, pageSize = 10 }: UsePhoneCardsCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<PhoneCard[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [lookups, setLookups] = useState<PhoneCardLookups>(emptyLookups)
  const [providerFilterOptions, setProviderFilterOptions] = useState<FormFieldOption[]>([])
  const [variantFilterOptions, setVariantFilterOptions] = useState<FormFieldOption[]>([])
  const [filtersReady, setFiltersReady] = useState(false)
  const [isLoadingLookups, setIsLoadingLookups] = useState(false)
  const [providerFilter, setProviderFilter] = useState('')
  const [variantFilter, setVariantFilter] = useState('')
  const [activeFilter, setActiveFilter] = useState<ActiveFilterValue>('all')
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<PhoneCard | null>(null)
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
  const lookupsLoadedRef = useRef(false)
  const lookupsRef = useRef<PhoneCardLookups>(emptyLookups)

  useEffect(() => {
    void fetchPhoneCardFilterOptions()
      .then((opts) => setProviderFilterOptions(opts.providerOptions))
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được bộ lọc'))
      })
      .finally(() => setFiltersReady(true))
  }, [])

  const listFilters = useMemo(
    () => ({
      keyword: globalFilter || undefined,
      providerId: providerFilter || undefined,
      productVariantId: variantFilter || undefined,
      isActive: activeFilterToBool(activeFilter),
    }),
    [globalFilter, providerFilter, variantFilter, activeFilter],
  )

  const loadData = useCallback(
    async (pageIndex: number, size: number, filters: phoneCardsApi.PhoneCardListFilters, seq: number) => {
      setIsLoading(true)
      try {
        const result = await phoneCardsApi.fetchPhoneCardsPage(pageIndex + 1, size, filters)
        if (seq !== loadSeqRef.current) return
        setData(result.items)
        setTotalCount(result.totalCount)
      } catch (e) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách thẻ viễn thông'))
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

  const ensureLookups = useCallback(async () => {
    if (lookupsLoadedRef.current) return

    setIsLoadingLookups(true)
    try {
      const loaded = await fetchPhoneCardLookups()
      lookupsLoadedRef.current = true
      lookupsRef.current = loaded
      setLookups(loaded)
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không tải được dữ liệu tham chiếu'))
      throw e
    } finally {
      setIsLoadingLookups(false)
    }
  }, [notifyError])

  const formConfig = useMemo(() => buildPhoneCardFormConfig(lookups, formMode), [lookups, formMode])

  const toggleActive = useCallback(
    async (row: PhoneCard) => {
      try {
        await phoneCardsApi.updatePhoneCard(row.id, {
          ...toUpdatePayload(row),
          isActive: !row.isActive,
        })
        setData((prev) =>
          prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)),
        )
        notifySuccess(!row.isActive ? 'Đã bật hiển thị thẻ' : 'Đã ẩn thẻ')
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
    await ensureLookups()
    setFormValues({
      ...getDefaultPhoneCardValues(),
      productVariantId: lookupsRef.current.productVariantOptions[0]?.value ?? '',
      providerId: lookupsRef.current.providerOptions[0]?.value ?? '',
    })
    setFormMode('create')
  }, [ensureLookups])

  const openEdit = useCallback(async (row: PhoneCard) => {
    await ensureLookups()
    setFormValues({ ...row })
    setFormMode('edit')
  }, [ensureLookups])

  const openView = useCallback((row: PhoneCard) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    async (rawValues: PhoneCard) => {
      if (!formMode || formMode === 'view') return

      const values = applySlugFromName(rawValues)

      setIsSaving(true)
      try {
        if (formMode === 'create') {
          await phoneCardsApi.createPhoneCard(toCreatePayload(values))
          notifySuccess('Đã thêm thẻ viễn thông thành công')
          if (pagination.pageIndex === 0) {
            reload()
          } else {
            setPagination((p) => ({ ...p, pageIndex: 0 }))
          }
        } else {
          await phoneCardsApi.updatePhoneCard(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật thẻ viễn thông thành công')
          reload()
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được thẻ viễn thông'))
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
      onEdit: (row: PhoneCard) => void openEdit(row),
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

  const setProviderFilterAndReset = useCallback((value: string) => {
    setProviderFilter(value)
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
      const loaded = await fetchPhoneCardLookups()
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
      await Promise.all([...pendingDeleteIds].map((id) => phoneCardsApi.deletePhoneCard(id)))
      setRowSelection({})
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess(
        pendingDeleteIds.size > 1
          ? `Đã xóa ${pendingDeleteIds.size} thẻ viễn thông`
          : 'Đã xóa thẻ viễn thông thành công',
      )

      const remainingOnPage = data.length - pendingDeleteIds.size
      if (remainingOnPage <= 0 && pagination.pageIndex > 0) {
        setPagination((p) => ({ ...p, pageIndex: p.pageIndex - 1 }))
      } else {
        reload()
      }
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được thẻ viễn thông'))
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
    filtersReady,
    providerFilterOptions,
    variantFilterOptions,
    providerFilter,
    setProviderFilter: setProviderFilterAndReset,
    variantFilter,
    setVariantFilter: setVariantFilterAndReset,
    activeFilter,
    setActiveFilter: setActiveFilterAndReset,
    loadVariantFilterOptions,
    reload,
  }
}
