import { getCoreRowModel, useReactTable } from '@tanstack/react-table'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import * as contentArticlesApi from '@/apis/contentArticlesApi'
import { fetchCategoryCodeLookup } from '@/apis/categoriesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import type { ContentArticleListItem, ContentPublishStatus } from '@/features/content/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

export type ArticleFeaturedFilter = 'all' | 'true' | 'false'
export type ArticleStatusFilter = '' | `${ContentPublishStatus}`

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function featuredFilterToBool(value: ArticleFeaturedFilter): boolean | undefined {
  if (value === 'true') return true
  if (value === 'false') return false
  return undefined
}

function statusFilterToStatus(value: ArticleStatusFilter): ContentPublishStatus | undefined {
  if (value === '') return undefined
  return Number(value) as ContentPublishStatus
}

export function useArticlesCrud(pageSize = 12) {
  const { showNotification } = useNotificationContext()
  const [items, setItems] = useState<ContentArticleListItem[]>([])
  const [totalCount, setTotalCount] = useState(0)
  const [isLoading, setIsLoading] = useState(true)
  const [categoryFilterOptions, setCategoryFilterOptions] = useState<FormFieldOption[]>([])
  const [categoryNameByCode, setCategoryNameByCode] = useState<Map<string, string>>(new Map())
  const [globalFilter, setGlobalFilter] = useState('')
  const [categoryFilter, setCategoryFilter] = useState('')
  const [statusFilter, setStatusFilter] = useState<ArticleStatusFilter>('')
  const [featuredFilter, setFeaturedFilter] = useState<ArticleFeaturedFilter>('all')
  const [pagination, setPagination] = useState({ pageIndex: 0, pageSize })
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
    void fetchCategoryCodeLookup(1, 100)
      .then(({ options, nameByCode }) => {
        setCategoryFilterOptions(options)
        setCategoryNameByCode(nameByCode)
      })
      .catch((e) => {
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh mục'))
      })
  }, [])

  const listFilters = useMemo(
    () => ({
      keyword: globalFilter || undefined,
      categoryCode: categoryFilter || undefined,
      status: statusFilterToStatus(statusFilter),
      isFeatured: featuredFilterToBool(featuredFilter),
    }),
    [globalFilter, categoryFilter, statusFilter, featuredFilter],
  )

  const loadData = useCallback(
    async (pageIndex: number, size: number, filters: contentArticlesApi.ArticleListFilters, seq: number) => {
      setIsLoading(true)
      try {
        const result = await contentArticlesApi.fetchContentArticlesPage(pageIndex + 1, size, filters)
        if (seq !== loadSeqRef.current) return
        setItems(result.items)
        setTotalCount(result.totalCount)
      } catch (e) {
        if (seq !== loadSeqRef.current) return
        notifyErrorRef.current(getErrorMessage(e, 'Không tải được danh sách bài viết'))
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

  const resetPageIndex = useCallback(() => {
    setPagination((prev) => ({ ...prev, pageIndex: 0 }))
  }, [])

  const setGlobalFilterAndReset = useCallback(
    (value: string) => {
      setGlobalFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const setCategoryFilterAndReset = useCallback(
    (value: string) => {
      setCategoryFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const setStatusFilterAndReset = useCallback(
    (value: ArticleStatusFilter) => {
      setStatusFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const setFeaturedFilterAndReset = useCallback(
    (value: ArticleFeaturedFilter) => {
      setFeaturedFilter(value)
      resetPageIndex()
    },
    [resetPageIndex],
  )

  const requestDelete = useCallback((id: string) => {
    setPendingDeleteIds(new Set([id]))
    setShowDeleteModal(true)
  }, [])

  const pageCount = Math.max(1, Math.ceil(totalCount / pagination.pageSize))

  const table = useReactTable({
    data: items,
    columns: [],
    pageCount,
    state: { pagination },
    onPaginationChange: setPagination,
    getCoreRowModel: getCoreRowModel(),
    manualPagination: true,
  })

  const paginationInfo = useMemo(() => {
    const { pageIndex, pageSize: size } = pagination
    if (totalCount === 0) return { start: 0, end: 0, total: 0 }
    const start = pageIndex * size + 1
    const end = Math.min(pageIndex * size + items.length, totalCount)
    return { start, end, total: totalCount }
  }, [pagination, totalCount, items.length])

  const confirmDelete = useCallback(async () => {
    try {
      await Promise.all([...pendingDeleteIds].map((id) => contentArticlesApi.deleteContentArticle(id)))
      setPendingDeleteIds(new Set())
      setShowDeleteModal(false)
      notifySuccess('Đã xóa bài viết thành công')
      reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được bài viết'))
    }
  }, [pendingDeleteIds, reload, notifySuccess, notifyError])

  return {
    items,
    categoryNameByCode,
    categoryFilterOptions,
    globalFilter,
    setGlobalFilter: setGlobalFilterAndReset,
    categoryFilter,
    setCategoryFilter: setCategoryFilterAndReset,
    statusFilter,
    setStatusFilter: setStatusFilterAndReset,
    featuredFilter,
    setFeaturedFilter: setFeaturedFilterAndReset,
    requestDelete,
    table,
    paginationInfo,
    pageCount,
    pageSize: pagination.pageSize,
    setPageSize: (size: number) => setPagination({ pageIndex: 0, pageSize: size }),
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteIds(new Set())
    },
    confirmDelete,
    pendingDeleteCount: pendingDeleteIds.size,
    isLoading,
  }
}
