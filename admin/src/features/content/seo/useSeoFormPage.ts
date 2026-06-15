import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate, useParams } from 'react-router'

import * as seoMetadataApi from '@/apis/seoMetadataApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  getDefaultSeoValues,
  toSeoPayload,
  validateSeoForm,
  type SeoFormErrors,
} from '@/features/content/seo/seoFormUtils'
import type { SeoMetadata } from '@/features/content/types'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useSeoFormPage() {
  const { seoId } = useParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = seoId === 'new' || !seoId
  const [values, setValues] = useState<SeoMetadata>(getDefaultSeoValues)
  const [errors, setErrors] = useState<SeoFormErrors>({})
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [loadFailed, setLoadFailed] = useState(false)

  const notifyErrorRef = useRef(showNotification)
  notifyErrorRef.current = showNotification

  useEffect(() => {
    if (isNew || !seoId) return
    setIsLoading(true)
    setLoadFailed(false)
    void seoMetadataApi
      .fetchSeoMetadataById(seoId)
      .then(setValues)
      .catch((e) => {
        setLoadFailed(true)
        notifyErrorRef.current({
          title: 'Lỗi',
          message: getErrorMessage(e, 'Không tải được cấu hình SEO'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => setIsLoading(false))
  }, [isNew, seoId])

  const updateField = useCallback(<K extends keyof SeoMetadata>(name: K, value: SeoMetadata[K]) => {
    setValues((prev) => ({ ...prev, [name]: value }))
    setErrors((prev) => {
      if (!prev[name]) return prev
      const next = { ...prev }
      delete next[name]
      return next
    })
  }, [])

  const cancel = useCallback(() => navigate('/website/seo'), [navigate])

  const submit = useCallback(async () => {
    const nextErrors = validateSeoForm(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSaving(true)
    try {
      const payload = toSeoPayload(values)
      if (isNew) await seoMetadataApi.createSeoMetadata(payload)
      else await seoMetadataApi.updateSeoMetadata(values.id, payload)
      showNotification({
        title: 'Thành công',
        message: isNew ? 'Đã tạo cấu hình SEO' : 'Đã cập nhật cấu hình SEO',
        variant: 'success',
        delay: 2500,
      })
      navigate('/website/seo')
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không lưu được SEO'),
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
    pageTitle: isNew ? 'Thêm cấu hình SEO' : 'Sửa cấu hình SEO',
    submitLabel: isNew ? 'Tạo SEO' : 'Lưu thay đổi',
    updateField,
    cancel,
    submit,
  }
}
