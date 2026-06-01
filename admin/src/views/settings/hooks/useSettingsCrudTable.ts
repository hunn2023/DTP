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
import { useCallback, useMemo, useState } from 'react'

import { getNextId } from '@/views/settings/form/getNextId'
import { slugify } from '@/views/settings/form/slugify'
import type { FormModalMode, SettingsFormConfig } from '@/views/settings/form/types'
import type { SettingsEntityBase } from '@/views/settings/types'

export type SettingsTableHandlers<T extends SettingsEntityBase> = {
  onToggleActive: (row: T) => void
  onDeleteRequest: (rowId: string) => void
  onEdit: (row: T) => void
}

type UseSettingsCrudTableParams<T extends SettingsEntityBase> = {
  initialData: T[]
  buildColumns: (handlers: SettingsTableHandlers<T>) => ColumnDef<T>[]
  formConfig: SettingsFormConfig<T>
  pageSize?: number
}

function applySlugFromName<T extends SettingsEntityBase>(values: T, formConfig: SettingsFormConfig<T>): T {
  if (!formConfig.slugFromName) return values
  if (!('name' in values) || !('slug' in values)) return values
  const name = String((values as T & { name: string }).name)
  const slug = String((values as T & { slug: string }).slug)
  if (!slug.trim() && name.trim()) {
    return { ...values, slug: slugify(name) } as T
  }
  return values
}

export function useSettingsCrudTable<T extends SettingsEntityBase>({
  initialData,
  buildColumns,
  formConfig,
  pageSize = 10,
}: UseSettingsCrudTableParams<T>) {
  const [data, setData] = useState<T[]>(() => [...initialData])
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [rowSelection, setRowSelection] = useState<Record<string, boolean>>({})
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteIds, setPendingDeleteIds] = useState<Set<number>>(new Set())
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<T | null>(null)

  const toggleActive = useCallback((row: T) => {
    setData((prev) => prev.map((item) => (item.id === row.id ? { ...item, isActive: !item.isActive } : item)))
  }, [])

  const requestDelete = useCallback((id: string) => {
    setPendingDeleteIds(new Set([Number(id)]))
    setShowDeleteModal(true)
  }, [])

  const openCreate = useCallback(() => {
    setFormValues(formConfig.getDefaultValues())
    setFormMode('create')
  }, [formConfig])

  const openEdit = useCallback((row: T) => {
    setFormValues({ ...row })
    setFormMode('edit')
  }, [])

  const openView = useCallback((row: T) => {
    setFormValues({ ...row })
    setFormMode('view')
  }, [])

  const closeFormModal = useCallback(() => {
    setFormMode(null)
    setFormValues(null)
  }, [])

  const saveForm = useCallback(
    (rawValues: T) => {
      if (!formMode || formMode === 'view') return

      let values = applySlugFromName(rawValues, formConfig)
      if (formConfig.onBeforeSave) {
        values = formConfig.onBeforeSave(values, formMode)
      }

      if (formMode === 'create') {
        const newRow = { ...values, id: getNextId(data) } as T
        setData((prev) => [...prev, newRow])
      } else {
        setData((prev) => prev.map((item) => (item.id === values.id ? values : item)))
      }
      closeFormModal()
    },
    [formMode, formConfig, data, closeFormModal],
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

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, columnFilters, pagination, rowSelection },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    onRowSelectionChange: setRowSelection,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => String(row.id),
    globalFilterFn: 'includesString',
    enableRowSelection: true,
  })

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = table.getState().pagination
    const total = table.getFilteredRowModel().rows.length
    const start = total === 0 ? 0 : pageIndex * size + 1
    const end = Math.min(start + size - 1, total)
    return { start, end, total }
  }, [table])

  const requestBulkDelete = useCallback(() => {
    setPendingDeleteIds(new Set(Object.keys(rowSelection).map(Number)))
    setShowDeleteModal(true)
  }, [rowSelection])

  const confirmDelete = useCallback(() => {
    setData((prev) => prev.filter((item) => !pendingDeleteIds.has(item.id)))
    setRowSelection({})
    setPendingDeleteIds(new Set())
    setShowDeleteModal(false)
    setPagination((p) => ({ ...p, pageIndex: 0 }))
  }, [pendingDeleteIds])

  const closeDeleteModal = useCallback(() => {
    setShowDeleteModal(false)
    setPendingDeleteIds(new Set())
  }, [])

  return {
    table,
    globalFilter,
    setGlobalFilter,
    paginationInfo,
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
  }
}
