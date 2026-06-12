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

import * as rolesApi from '@/apis/rolesApi'
import * as usersApi from '@/apis/usersApi'
import type { UserRow } from '@/apis/usersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { UserTableHandlers } from '@/features/system/users/columns'
import { userCreateFormConfig, userEditFormConfig } from '@/features/system/users/formConfig'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'
import type { UserFormValues } from '@/features/system/users/types'
import {
  getDefaultUserValues,
  userDetailToFormValues,
  userRowToFormValues,
} from '@/features/system/users/types'
import type { FormModalMode } from '@/modules/crud/form/types'

type Params = {
  buildColumns: (handlers: UserTableHandlers) => ColumnDef<UserRow>[]
  pageSize?: number
}

function toCreatePayload(values: UserFormValues): usersApi.UserCreatePayload {
  return {
    email: values.email.trim(),
    fullName: values.fullName.trim(),
    phone: values.phone.trim() || undefined,
    password: values.password,
    roleIds: values.roleIds,
  }
}

function toUpdatePayload(values: UserFormValues): usersApi.UserUpdatePayload {
  return {
    fullName: values.fullName.trim(),
    phone: values.phone.trim() || undefined,
    avatarUrl: values.avatarUrl.trim() || undefined,
    isActive: values.isActive,
  }
}

export function useUsersCrud({ buildColumns, pageSize = 10 }: Params) {
  const { showNotification } = useNotificationContext()
  const [data, setData] = useState<UserRow[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [globalFilter, setGlobalFilter] = useState('')
  const [sorting, setSorting] = useState<SortingState>([])
  const [columnFilters, setColumnFilters] = useState<ColumnFiltersState>([])
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
  const [formMode, setFormMode] = useState<FormModalMode | null>(null)
  const [formValues, setFormValues] = useState<UserFormValues | null>(null)
  const [assignUser, setAssignUser] = useState<UserRow | null>(null)
  const [assignOptions, setAssignOptions] = useState<{ value: string; label: string; hint?: string }[]>([])
  const [assignSelectedIds, setAssignSelectedIds] = useState<string[]>([])
  const [assignLoading, setAssignLoading] = useState(false)
  const [assignSaving, setAssignSaving] = useState(false)
  const [createRoleOptions, setCreateRoleOptions] = useState<{ value: string; label: string; hint?: string }[]>([])
  const [createRolesLoading, setCreateRolesLoading] = useState(false)
  const [isSaving, setIsSaving] = useState(false)

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

  const loadData = useCallback(
    async (pageIndex: number, size: number, keyword: string, seq: number) => {
      setIsLoading(true)
      try {
        const result = await usersApi.fetchUsersPage(pageIndex + 1, size, keyword || undefined)
        if (seq !== loadSeqRef.current) return
        setData(result.items)
        setTotalCount(result.totalCount)
      } catch (error) {
        if (seq !== loadSeqRef.current) return
        notifyError(getErrorMessage(error, 'Không tải được tài khoản'))
      } finally {
        if (seq === loadSeqRef.current) setIsLoading(false)
      }
    },
    [notifyError],
  )

  const reload = useCallback(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [loadData, pagination.pageIndex, pagination.pageSize, globalFilter])

  useEffect(() => {
    const seq = ++loadSeqRef.current
    void loadData(pagination.pageIndex, pagination.pageSize, globalFilter, seq)
  }, [pagination.pageIndex, pagination.pageSize, globalFilter, loadData])

  const openAssignRoles = useCallback(
    async (row: UserRow) => {
      setAssignUser(row)
      setAssignLoading(true)
      try {
        const roles = await rolesApi.fetchRoles()
        setAssignOptions(
          roles.map((role) => ({
            value: role.id,
            label: role.name,
            hint: role.code,
          })),
        )
        setAssignSelectedIds([])
      } catch (error) {
        setAssignUser(null)
        notifyError(getErrorMessage(error, 'Không tải được danh sách vai trò'))
      } finally {
        setAssignLoading(false)
      }
    },
    [notifyError],
  )

  const saveAssignRoles = useCallback(
    async (roleIds: string[]) => {
      if (!assignUser) return
      setAssignSaving(true)
      try {
        await usersApi.assignUserRoles(assignUser.id, { roleIds })
        notifySuccess('Đã gán vai trò cho tài khoản')
        setAssignUser(null)
        reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không gán được vai trò'))
      } finally {
        setAssignSaving(false)
      }
    },
    [assignUser, notifySuccess, notifyError, reload],
  )

  const toggleLock = useCallback(
    async (row: UserRow) => {
      try {
        if (row.isActive) {
          await usersApi.lockUser(row.id)
          notifySuccess('Đã khóa tài khoản')
        } else {
          await usersApi.unlockUser(row.id)
          notifySuccess('Đã mở khóa tài khoản')
        }
        reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không cập nhật được trạng thái'))
      }
    },
    [notifySuccess, notifyError, reload],
  )

  const openEdit = useCallback(
    async (row: UserRow) => {
      try {
        const detail = await usersApi.fetchUserById(row.id)
        setFormValues(userDetailToFormValues(detail))
        setFormMode('edit')
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không tải được chi tiết tài khoản'))
      }
    },
    [notifyError],
  )

  const handlers = useMemo<UserTableHandlers>(
    () => ({
      onEdit: (row) => {
        void openEdit(row)
      },
      onToggleLock: (row) => {
        void toggleLock(row)
      },
      onAssignRoles: (row) => {
        void openAssignRoles(row)
      },
    }),
    [openEdit, toggleLock, openAssignRoles],
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
    getRowId: (row) => row.id,
    manualPagination: true,
  })

  const setGlobalFilterAndReset = useCallback((value: string) => {
    setGlobalFilter(value)
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + data.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, data.length])

  const saveForm = useCallback(
    async (values: UserFormValues) => {
      if (!formMode || formMode === 'view') return
      setIsSaving(true)
      try {
        if (formMode === 'create') {
          await usersApi.createUser(toCreatePayload(values))
          notifySuccess('Đã tạo tài khoản')
        } else {
          await usersApi.updateUser(values.id, toUpdatePayload(values))
          notifySuccess('Đã cập nhật tài khoản')
        }
        setFormMode(null)
        setFormValues(null)
        reload()
      } catch (error) {
        notifyError(getErrorMessage(error, 'Không lưu được tài khoản'))
      } finally {
        setIsSaving(false)
      }
    },
    [formMode, notifySuccess, notifyError, reload],
  )

  const openCreate = useCallback(async () => {
    setFormValues(getDefaultUserValues())
    setFormMode('create')
    setCreateRolesLoading(true)
    try {
      const roles = await rolesApi.fetchRoles()
      setCreateRoleOptions(
        roles.map((role) => ({
          value: role.id,
          label: role.name,
          hint: role.code,
        })),
      )
    } catch (error) {
      setFormMode(null)
      setFormValues(null)
      notifyError(getErrorMessage(error, 'Không tải được danh sách vai trò'))
    } finally {
      setCreateRolesLoading(false)
    }
  }, [notifyError])

  const activeFormConfig = formMode === 'create' ? userCreateFormConfig : userEditFormConfig

  return {
    table,
    globalFilter,
    setGlobalFilter: setGlobalFilterAndReset,
    isLoading,
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize: (size: number) => setPagination({ pageIndex: 0, pageSize: size }),
    formMode,
    formValues,
    formConfig: activeFormConfig,
    openCreate,
    openView: (row: UserRow) => {
      setFormValues(userRowToFormValues(row))
      setFormMode('view')
    },
    closeFormModal: () => {
      setFormMode(null)
      setFormValues(null)
    },
    saveForm,
    isSaving,
    createRoleOptions,
    createRolesLoading,
    assignUser,
    assignOptions,
    assignSelectedIds,
    assignLoading,
    assignSaving,
    closeAssignModal: () => setAssignUser(null),
    saveAssignRoles,
  }
}
