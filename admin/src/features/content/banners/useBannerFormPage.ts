import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate, useParams } from 'react-router'

import * as contentBannersApi from '@/apis/contentBannersApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  getDefaultBannerValues,
  toBannerPayload,
  validateBannerForm,
  type BannerFormErrors,
} from '@/features/content/banners/bannerFormUtils'
import type { ContentBanner } from '@/features/content/types'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useBannerFormPage() {
  const { bannerId } = useParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = bannerId === 'new' || !bannerId
  const [values, setValues] = useState<ContentBanner>(getDefaultBannerValues)
  const [errors, setErrors] = useState<BannerFormErrors>({})
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [loadFailed, setLoadFailed] = useState(false)

  const notifyErrorRef = useRef(showNotification)
  notifyErrorRef.current = showNotification

  useEffect(() => {
    if (isNew || !bannerId) return

    setIsLoading(true)
    setLoadFailed(false)
    void contentBannersApi
      .fetchContentBannerById(bannerId)
      .then((banner) => setValues(banner))
      .catch((e) => {
        setLoadFailed(true)
        notifyErrorRef.current({
          title: 'Lỗi',
          message: getErrorMessage(e, 'Không tải được banner'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => setIsLoading(false))
  }, [isNew, bannerId])

  const updateField = useCallback(<K extends keyof ContentBanner>(name: K, value: ContentBanner[K]) => {
    setValues((prev) => ({ ...prev, [name]: value }))
    setErrors((prev) => {
      if (!prev[name]) return prev
      const next = { ...prev }
      delete next[name]
      return next
    })
  }, [])

  const cancel = useCallback(() => {
    navigate('/website/banners')
  }, [navigate])

  const submit = useCallback(async () => {
    const nextErrors = validateBannerForm(values)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSaving(true)
    try {
      const payload = toBannerPayload(values)
      if (isNew) {
        await contentBannersApi.createContentBanner(payload)
        showNotification({
          title: 'Thành công',
          message: 'Đã tạo banner thành công',
          variant: 'success',
          delay: 2500,
        })
      } else {
        await contentBannersApi.updateContentBanner(values.id, payload)
        showNotification({
          title: 'Thành công',
          message: 'Đã cập nhật banner thành công',
          variant: 'success',
          delay: 2500,
        })
      }
      navigate('/website/banners')
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không lưu được banner'),
        variant: 'danger',
        delay: 4000,
      })
    } finally {
      setIsSaving(false)
    }
  }, [values, isNew, navigate, showNotification])

  const pageTitle = isNew ? 'Thêm banner mới' : 'Sửa banner'
  const submitLabel = isNew ? 'Tạo mới Banner' : 'Lưu thay đổi'

  return {
    values,
    errors,
    isNew,
    isLoading,
    isSaving,
    loadFailed,
    pageTitle,
    submitLabel,
    updateField,
    cancel,
    submit,
  }
}
