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
import type { EsimPackageTableHandlers } from '@/features/products/esim-packages/columns'
import * as esimPackagesApi from '@/features/products/esim-packages/esim-packages.api'
import { buildEsimPackageFormConfig } from '@/features/products/esim-packages/formConfig'
import {
  fetchEsimPackageLookups,
  type EsimPackageLookups,
} from '@/features/products/esim-packages/lookups.api'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import { slugify } from '@/modules/crud/form/slugify'
import type { FormModalMode } from '@/modules/crud/form/types'

type UseEsimPackagesCrudParams = {
  buildColumns: (handlers: EsimPackageTableHandlers) => ColumnDef<EsimPackage>[]
  pageSize?: number
}

const emptyLookups: EsimPackageLookups = {
  countryOptions: [],
  carrierOptions: [],
  productVariantOptions: [],
}

function applySlugFromName(values: EsimPackage): EsimPackage {
  if (!values.slug.trim() && values.name.trim()) {
    return { ...values, slug: slugify(values.name) }
  }
  return values
}

function toCreatePayload(values: EsimPackage): esimPackagesApi.EsimPackageCreatePayload {
  return {
    productVariantId: values.productVariantId.trim(),
    countryId: values.countryId.trim(),
    carrierId: values.carrierId.trim(),
    name: values.name.trim(),
    slug: values.slug.trim(),
    dataAmount: values.dataAmount,
    dataUnit: values.dataUnit.trim(),
    validityDays: values.validityDays,
    price: values.price,
    currency: values.currency.trim(),
    isUnlimited: values.isUnlimited,
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

function toUpdatePayload(values: EsimPackage): esimPackagesApi.EsimPackageUpdatePayload {
  const { productVariantId: _ignored, ...payload } = toCreatePayload(values)
  return payload
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useEsimPackagesCrud({ buildColumns, pageSize = 10 }: UseEsimPackagesCrudParams) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<EsimPackage[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [lookups, setLookups] = useState<EsimPackageLookups>(emptyLookups)
  const [lookupsReady, setLookupsReady] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<string>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<EsimPackage | null>(null)
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

  useEffect(() => {
    void fetchEsimPackageLookups()
      .then(setLookups)
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được dữ liệu tham chiếu'))
      })
      .finally(() => setLookupsReady(true))
  }, [])

  const loadData = useCallback(async (pageIndex: number, size: number, keyword: string, seq: number) => {
    setIsLoading(true)
    try {
      const result = await esimPackagesApi.fetchEsimPackagesPage(pageIndex + 1, size, keyword || undefined)
      if (seq !== loadSeqRef.current) return
      setData(result.items)
      setTotalCount(result.totalCount)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách gói eSIM'))
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

  const formConfig = useMemo(
    () => buildEsimPackageFormConfig(lookups, formMode),
    [lookups, formMode],
  )

  const toggleActive = useCallback(
    async (row: EsimPackage) => {
      try {
        await esimPackagesApi.updateEsimPackage(row.id, {
          ...toUpdatePayload(row),
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
    setFormValues(formConfig.getDefaultValues())
    setFormMode('create')
  }, [formConfig])

  const openEdit = useCallback((row: EsimPackage) => {
    setFormValues({ ...row })
    setFormMode('edit')
  }, [])

  const openView = useCallback((row: EsimPackage) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    async (rawValues: EsimPackage) => {
      if (!formMode || formMode === 'view') return

      const values = applySlugFromName(rawValues)

      setIsSaving(true)
      try {
        if (formMode === 'create') {
          await esimPackagesApi.createEsimPackage(toCreatePayload(values))
          notifySuccess('Đã thêm gói eSIM thành công')
          if (pagination.pageIndex === 0) {
            reload()
          } else {
            setPagination((p) => ({ ...p, pageIndex: 0 }))
          }
        } else {
          await esimPackagesApi.updateEsimPackage(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật gói eSIM thành công')
          reload()
        }
        closeFormModal()
      } catch (e) {
        notifyError(getErrorMessage(e, 'Không lưu được gói eSIM'))
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
    formMode,
    formValues,
    openCreate,
    openView,
    closeFormModal,
    saveForm,
    formConfig,
    isLoading,
    isSaving,
    lookupsReady,
    reload,
  }
}
