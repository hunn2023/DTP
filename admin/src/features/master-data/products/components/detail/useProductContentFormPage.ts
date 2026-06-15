import { useCallback, useEffect, useRef, useState } from 'react'

import * as contentsApi from '@/apis/productContentsApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  getDefaultProductContentValues,
  toProductContentPayload,
  toProductContentUpdatePayload,
  validateProductContentForm,
  type ProductContentFormErrors,
} from '@/features/master-data/products/components/detail/productContentFormUtils'
import type { ProductContentRow } from '@/features/master-data/products/types'

type Params = {
  productId: string
  contentId: string
  onCancel: () => void
  onSaved: () => void
}

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useProductContentFormPage({ productId, contentId, onCancel, onSaved }: Params) {
  const { showNotification } = useNotificationContext()
  const isNew = contentId === 'new'

  const [values, setValues] = useState<ProductContentRow>(getDefaultProductContentValues(productId))
  const [errors, setErrors] = useState<ProductContentFormErrors>({})
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [loadFailed, setLoadFailed] = useState(false)
  const [isEditorReady, setIsEditorReady] = useState(isNew)

  const notifyErrorRef = useRef(showNotification)
  notifyErrorRef.current = showNotification

  useEffect(() => {
    if (isNew) {
      setValues(getDefaultProductContentValues(productId))
      setLoadFailed(false)
      setIsLoading(false)
      return
    }

    setIsLoading(true)
    setLoadFailed(false)
    void contentsApi
      .fetchProductContentById(contentId)
      .then((detail) => {
        if (!detail) {
          setLoadFailed(true)
          return
        }
        setValues(detail)
      })
      .catch((e) => {
        setLoadFailed(true)
        notifyErrorRef.current({
          title: 'Lỗi',
          message: getErrorMessage(e, 'Không tải được nội dung'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => setIsLoading(false))
  }, [isNew, contentId, productId])

  useEffect(() => {
    if (isLoading) {
      setIsEditorReady(false)
      return
    }

    const idleId = requestIdleCallback(() => setIsEditorReady(true), { timeout: 500 })
    return () => cancelIdleCallback(idleId)
  }, [isLoading, contentId])

  const updateField = useCallback(<K extends keyof ProductContentRow>(name: K, value: ProductContentRow[K]) => {
    setValues((prev) => ({ ...prev, [name]: value }))
    setErrors((prev) => {
      if (!prev[name as keyof ProductContentFormErrors]) return prev
      const next = { ...prev }
      delete next[name as keyof ProductContentFormErrors]
      return next
    })
  }, [])

  const submit = useCallback(async () => {
    const nextErrors = validateProductContentForm(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSaving(true)
    try {
      if (isNew) {
        await contentsApi.createProductContent(toProductContentPayload(values))
        showNotification({
          title: 'Thành công',
          message: 'Đã tạo nội dung sản phẩm',
          variant: 'success',
          delay: 2500,
        })
      } else {
        await contentsApi.updateProductContent(values.id, toProductContentUpdatePayload(values))
        showNotification({
          title: 'Thành công',
          message: 'Đã cập nhật nội dung sản phẩm',
          variant: 'success',
          delay: 2500,
        })
      }
      onSaved()
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không lưu được nội dung'),
        variant: 'danger',
        delay: 4000,
      })
    } finally {
      setIsSaving(false)
    }
  }, [values, isNew, showNotification, onSaved])

  return {
    isNew,
    values,
    errors,
    isLoading,
    isSaving,
    loadFailed,
    isEditorReady,
    pageTitle: isNew ? 'Thêm nội dung sản phẩm' : 'Sửa nội dung sản phẩm',
    submitLabel: isNew ? 'Tạo nội dung' : 'Lưu thay đổi',
    updateField,
    submit,
    cancel: onCancel,
  }
}
