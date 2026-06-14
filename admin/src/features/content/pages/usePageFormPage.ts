import { useCallback, useEffect, useState } from 'react'
import { useNavigate, useParams } from 'react-router'

import * as contentPagesApi from '@/apis/contentPagesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  applyPageSlug,
  getDefaultPageValues,
  toPageCreatePayload,
  toPageUpdatePayload,
  validatePageForm,
} from '@/features/content/pages/pageFormUtils'
import type { ContentPage } from '@/features/content/types'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function usePageFormPage() {
  const { pageId } = useParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()
  const isNew = pageId === 'new' || !pageId
  const [values, setValues] = useState<ContentPage>(getDefaultPageValues)
  const [errors, setErrors] = useState<Partial<Record<keyof ContentPage, string>>>({})
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [loadFailed, setLoadFailed] = useState(false)

  useEffect(() => {
    if (isNew || !pageId) return
    setIsLoading(true)
    void contentPagesApi
      .fetchContentPageById(pageId)
      .then(setValues)
      .catch(() => setLoadFailed(true))
      .finally(() => setIsLoading(false))
  }, [isNew, pageId])

  const updateField = useCallback(<K extends keyof ContentPage>(name: K, value: ContentPage[K]) => {
    setValues((prev) => (name === 'title' ? applyPageSlug({ ...prev, [name]: value }) : { ...prev, [name]: value }))
    setErrors((prev) => {
      if (!prev[name]) return prev
      const next = { ...prev }
      delete next[name]
      return next
    })
  }, [])

  const submit = useCallback(async () => {
    const draft = applyPageSlug(values)
    const nextErrors = validatePageForm(draft, isNew)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSaving(true)
    try {
      if (isNew) await contentPagesApi.createContentPage(toPageCreatePayload(draft))
      else await contentPagesApi.updateContentPage(draft.id, toPageUpdatePayload(draft))
      showNotification({ title: 'Thành công', message: 'Đã lưu trang', variant: 'success', delay: 2500 })
      navigate('/website/pages')
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không lưu được trang'),
        variant: 'danger',
        delay: 4000,
      })
    } finally {
      setIsSaving(false)
    }
  }, [values, isNew, navigate, showNotification])

  return {
    values,
    errors,
    isNew,
    isLoading,
    isSaving,
    loadFailed,
    pageTitle: isNew ? 'Thêm trang tĩnh' : 'Sửa trang tĩnh',
    submitLabel: isNew ? 'Tạo trang' : 'Lưu trang',
    updateField,
    cancel: () => navigate('/website/pages'),
    submit,
  }
}
