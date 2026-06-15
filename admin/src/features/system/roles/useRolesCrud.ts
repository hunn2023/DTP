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
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import * as rolesApi from '@/apis/rolesApi'
import type { RoleRow } from '@/apis/rolesApi'
import type { PermissionRow } from '@/apis/permissionsApi'
import { fetchPermissionsByModule } from '@/apis/permissionsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { RoleTableHandlers } from '@/features/system/roles/columns'
import { buildRoleEditFormConfig, roleFormConfig } from '@/features/system/roles/formConfig'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import type { RoleFormValues } from '@/features/system/roles/types'
import { getDefaultRoleValues, roleRowToFormValues } from '@/features/system/roles/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  buildColumns: (handlers: RoleTableHandlers) => ColumnDef<RoleRow>[]
  pageSize?: number
}

function toCreatePayload(values: RoleFormValues): rolesApi.RoleCreatePayload {
  return {
    code: values.code.trim(),
    name: values.name.trim(),
    description: values.description.trim() || undefined,
  }
}

function toUpdatePayload(values: RoleFormValues): rolesApi.RoleUpdatePayload {
  return {
    name: values.name.trim(),
    description: values.description.trim() || undefined,
    isActive: values.isActive,
  }
}

export function useRolesCrud({ buildColumns, pageSize = 10 }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<RoleRow[]>([])
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<RoleFormValues | null>(null)
  const [assignRole, setAssignRole] = useState<RoleRow | null>(null)
  const [assignSelectedIds, setAssignSelectedIds] = useState<string[]>([])
  const [assignGroupedOptions, setAssignGroupedOptions] = useState<Record<string, { value: string; label: string; hint?: string }[]>>({})
  const [assignLoading, setAssignLoading] = useState(false)
  const [assignSaving, setAssignSaving] = useState(false)

  const loadSeqRef = useRef(0)

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

  const reload = useCallback(async () => {
    const seq = ++loadSeqRef.current
    setIsLoading(true)
    try {
      const items = await rolesApi.fetchRoles()
      if (seq !== loadSeqRef.current) return
      setData(items)
    } catch (error) {
      notifyError(getErrorMessage(error, 'Không tải được vai trò'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [notifyError])

  useEffect(() => {
    void reload()
  }, [reload])

  const openAssignPermissions = useCallback(
    async (row: RoleRow) => {
      setAssignRole(row)
      setAssignLoading(true)
      try {
        const [grouped, detail] = await Promise.all([
          fetchPermissionsByModule(),
          rolesApi.fetchRoleById(row.id),
        ])
        const options: Record<string, { value: string; label: string; hint?: string }[]> = {}
        Object.entries(grouped).forEach(([module, items]) => {
          options[module] = items.map((item: PermissionRow) => ({
            value: item.id,
            label: item.name,
            hint: item.code,
          }))
        })
        setAssignGroupedOptions(options)
        setAssignSelectedIds(detail.permissions.map((item) => item.id))
      } catch (error) {
        setAssignRole(null)
        notifyError(getErrorMessage(error, 'Không tải được danh sách quyền'))
      } finally {
        setAssignLoading(false)
      }
    },
    [notifyError],
  )

  const saveAssignPermissions = useCallback(
    async (permissionIds: string[]) => {
      if (!assignRole) return
      setAssignSaving(true)
      try {
        await rolesApi.assignRolePermissions(assignRole.id, { permissionIds })
        notifySuccess('Đã gán quyền cho vai trò')
        setAssignRole(null)
        await reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không gán được quyền'))
      } finally {
        setAssignSaving(false)
      }
    },
    [assignRole, notifySuccess, notifyError, reload],
  )

  const toggleActive = useCallback(
    async (row: RoleRow) => {
      try {
        await rolesApi.updateRole(row.id, {
          name: row.name,
          description: row.description || undefined,
          isActive: !row.isActive,
        })
        notifySuccess(!row.isActive ? 'Đã kích hoạt vai trò' : 'Đã ngưng vai trò')
        await reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không cập nhật được trạng thái'))
      }
    },
    [notifySuccess, notifyError, reload],
  )

  const handlers = useMemo<RoleTableHandlers>(
    () => ({
      onEdit: (row) => {
        setFormValues(roleRowToFormValues(row))
        setFormMode('edit')
      },
      onToggleActive: (row) => {
        void toggleActive(row)
      },
      onAssignPermissions: (row) => {
        void openAssignPermissions(row)
      },
    }),
    [toggleActive, openAssignPermissions],
  )

  const columns = useMemo(() => buildColumns(handlers), [buildColumns, handlers])

  const table = useReactTable({
    data,
    columns,
    state: { sorting, globalFilter, columnFilters, pagination },
    onSortingChange: setSorting,
    onGlobalFilterChange: setGlobalFilter,
    onColumnFiltersChange: setColumnFilters,
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getPaginationRowModel: getPaginationRowModel(),
    getRowId: (row) => row.id,
    globalFilterFn: 'includesString',
  })

  const paginationInfo = useMemo(() => {
    const filtered = table.getFilteredRowModel().rows.length
    const { pageIndex, pageSize: size } = pagination
    if (filtered === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(start + size - 1, filtered)
    return { start, end, total: filtered }
  }, [table, pagination])

  const saveForm = useCallback(
    async (values: RoleFormValues) => {
      if (!formMode || formMode === 'view') return
      try {
        if (formMode === 'create') {
          await rolesApi.createRole(toCreatePayload(values))
          notifySuccess('Đã tạo vai trò')
        } else {
          await rolesApi.updateRole(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật vai trò')
        }
        setFormMode(null)
        setFormValues(null)
        await reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không lưu được vai trò'))
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const activeFormConfig = formMode === 'edit' ? buildRoleEditFormConfig() : roleFormConfig

  return {
    table,
    globalFilter,
    setGlobalFilter,
    isLoading,
    paginationInfo,
    pageCount: table.getPageCount(),
    pageSize: pagination.pageSize,
    setPageSize: (size: number) => setPagination({ pageIndex: 0, pageSize: size }),
    formMode,
    formValues,
    formConfig: activeFormConfig,
    openCreate: () => {
      setFormValues(getDefaultRoleValues())
      setFormMode('create')
    },
    openView: (row: RoleRow) => {
      setFormValues(roleRowToFormValues(row))
      setFormMode('view')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
    assignRole,
    assignSelectedIds,
    assignGroupedOptions,
    assignLoading,
    assignSaving,
    closeAssignModal: () => setAssignRole(null),
    saveAssignPermissions,
  }
}
