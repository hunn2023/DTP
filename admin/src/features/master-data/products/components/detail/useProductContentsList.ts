import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import * as contentsApi from '@/apis/productContentsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import { PRODUCT_CONTENT_TYPE_OPTIONS } from '@/features/master-data/products/components/detail/contentFormConfig'
import type { ProductContentRow, ProductContentType } from '@/features/master-data/products/types'

type Params = {
  productId: string
  isTabActive: boolean
}

const ONLY_ACTIVE_OPTIONS = [
  { value: 'true', label: 'Chỉ đang hiển thị' },
] as const

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

function parseContentTypeFilter(value: string): ProductContentType | null {
  const num = Number(value)
  if (num >= 1 && num <= 6) return num as ProductContentType
  return null
}

export function useProductContentsList({ productId, isTabActive }: Params) {
  const { showNotification } = useNotificationContext()
  const [items, setItems] = useState<ProductContentRow[]>([])
  const [isLoading, setIsLoading] = useState(false)
  const [globalFilter, setGlobalFilter] = useState('')
  const [contentTypeFilter, setContentTypeFilter] = useState('')
  const [onlyActiveFilter, setOnlyActiveFilter] = useState('')
  const [showDeleteModal, setShowDeleteModal] = useState(false)
  const [pendingDeleteId, setPendingDeleteId] = useState('')

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

  const listParams = useMemo(
    (): contentsApi.ProductContentListParams => ({
      onlyActive: onlyActiveFilter === 'true' ? true : undefined,
    }),
    [onlyActiveFilter],
  )

  const reload = useCallback(async () => {
    const seq = ++loadSeqRef.current
    setIsLoading(true)
    try {
      const contentType = parseContentTypeFilter(contentTypeFilter)
      const data = contentType
        ? await contentsApi.fetchProductContentsByProductAndType(productId, contentType, listParams)
        : await contentsApi.fetchProductContentsByProduct(productId, listParams)

      if (seq !== loadSeqRef.current) return
      setItems(data)
    } catch (e) {
      if (seq !== loadSeqRef.current) return
      notifyErrorRef.current(getErrorMessage(e, 'Không tải được nội dung'))
    } finally {
      if (seq === loadSeqRef.current) setIsLoading(false)
    }
  }, [productId, contentTypeFilter, listParams])

  useEffect(() => {
    if (!isTabActive) return
    void reload()
  }, [isTabActive, reload])

  const filteredItems = useMemo(() => {
    const q = globalFilter.trim().toLowerCase()
    if (!q) return items
    return items.filter((item) => {
      const haystack = [item.title, item.summary, item.contentTypeName, String(item.contentType)]
        .join(' ')
        .toLowerCase()
      return haystack.includes(q)
    })
  }, [items, globalFilter])

  const requestDelete = useCallback((id: string) => {
    setPendingDeleteId(id)
    setShowDeleteModal(true)
  }, [])

  const confirmDelete = useCallback(async () => {
    if (!pendingDeleteId) return
    try {
      await contentsApi.deleteProductContent(pendingDeleteId)
      setShowDeleteModal(false)
      setPendingDeleteId('')
      notifySuccess('Đã xóa nội dung')
      await reload()
    } catch (e) {
      notifyError(getErrorMessage(e, 'Không xóa được nội dung'))
    }
  }, [pendingDeleteId, notifySuccess, notifyError, reload])

  return {
    items: filteredItems,
    globalFilter,
    setGlobalFilter,
    contentTypeFilter,
    setContentTypeFilter,
    onlyActiveFilter,
    setOnlyActiveFilter,
    contentTypeOptions: PRODUCT_CONTENT_TYPE_OPTIONS,
    onlyActiveOptions: ONLY_ACTIVE_OPTIONS,
    isLoading,
    reload,
    showDeleteModal,
    closeDeleteModal: () => {
      setShowDeleteModal(false)
      setPendingDeleteId('')
    },
    confirmDelete,
    requestDelete,
  }
}
