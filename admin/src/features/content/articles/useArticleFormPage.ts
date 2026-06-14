import { useCallback, useEffect, useRef, useState } from 'react'
import { useNavigate, useParams } from 'react-router'

import * as contentArticlesApi from '@/apis/contentArticlesApi'
import { fetchCategoryCodeLookup } from '@/apis/categoriesApi'
import { useNotificationContext } from '@/context/useNotificationContext'
import {
  applyArticleSlug,
  getDefaultArticleValues,
  toArticlePayload,
  validateArticleForm,
  type ArticleFormErrors,
} from '@/features/content/articles/articleFormUtils'
import type { ContentArticle } from '@/features/content/types'
import type { FormFieldOption } from '@/modules/crud/form/types'

function getErrorMessage(error: unknown, fallback: string): string {
  return error instanceof Error ? error.message : fallback
}

export function useArticleFormPage() {
  const { articleId } = useParams()
  const navigate = useNavigate()
  const { showNotification } = useNotificationContext()

  const isNew = articleId === 'new' || !articleId
  const [values, setValues] = useState<ContentArticle>(getDefaultArticleValues)
  const [errors, setErrors] = useState<ArticleFormErrors>({})
  const [categoryOptions, setCategoryOptions] = useState<FormFieldOption[]>([])
  const [isLoading, setIsLoading] = useState(!isNew)
  const [isSaving, setIsSaving] = useState(false)
  const [loadFailed, setLoadFailed] = useState(false)

  const notifyErrorRef = useRef(showNotification)
  notifyErrorRef.current = showNotification

  useEffect(() => {
    void fetchCategoryCodeLookup(1, 100).then(({ options }) => setCategoryOptions(options))
  }, [])

  useEffect(() => {
    if (isNew || !articleId) return
    setIsLoading(true)
    setLoadFailed(false)
    void contentArticlesApi
      .fetchContentArticleById(articleId)
      .then(setValues)
      .catch((e) => {
        setLoadFailed(true)
        notifyErrorRef.current({
          title: 'Lỗi',
          message: getErrorMessage(e, 'Không tải được bài viết'),
          variant: 'danger',
          delay: 4000,
        })
      })
      .finally(() => setIsLoading(false))
  }, [isNew, articleId])

  const updateField = useCallback(<K extends keyof ContentArticle>(name: K, value: ContentArticle[K]) => {
    setValues((prev) => {
      const next = { ...prev, [name]: value }
      return name === 'title' ? applyArticleSlug(next) : next
    })
    setErrors((prev) => {
      if (!prev[name]) return prev
      const next = { ...prev }
      delete next[name]
      return next
    })
  }, [])

  const cancel = useCallback(() => navigate('/website/articles'), [navigate])

  const [isActioning, setIsActioning] = useState(false)

  const reloadArticle = useCallback(async () => {
    if (isNew || !values.id) return
    const article = await contentArticlesApi.fetchContentArticleById(values.id)
    setValues(article)
  }, [isNew, values.id])

  const runPatchAction = useCallback(
    async (action: () => Promise<void>, successMessage: string, errorMessage: string) => {
      if (isNew || !values.id) return
      setIsActioning(true)
      try {
        await action()
        await reloadArticle()
        showNotification({
          title: 'Thành công',
          message: successMessage,
          variant: 'success',
          delay: 2500,
        })
      } catch (e) {
        showNotification({
          title: 'Lỗi',
          message: getErrorMessage(e, errorMessage),
          variant: 'danger',
          delay: 4000,
        })
      } finally {
        setIsActioning(false)
      }
    },
    [isNew, values.id, reloadArticle, showNotification],
  )

  const publish = useCallback(
    () =>
      runPatchAction(
        () => contentArticlesApi.publishContentArticle(values.id),
        'Đã đăng bài viết',
        'Không đăng được bài viết',
      ),
    [runPatchAction, values.id],
  )

  const hide = useCallback(
    () =>
      runPatchAction(
        () => contentArticlesApi.hideContentArticle(values.id),
        'Đã ẩn bài viết',
        'Không ẩn được bài viết',
      ),
    [runPatchAction, values.id],
  )

  const feature = useCallback(
    () =>
      runPatchAction(
        () => contentArticlesApi.featureContentArticle(values.id),
        'Đã đánh dấu nổi bật',
        'Không cập nhật nổi bật được',
      ),
    [runPatchAction, values.id],
  )

  const unfeature = useCallback(
    () =>
      runPatchAction(
        () => contentArticlesApi.unfeatureContentArticle(values.id),
        'Đã bỏ nổi bật',
        'Không bỏ nổi bật được',
      ),
    [runPatchAction, values.id],
  )

  const submit = useCallback(async () => {
    const draft = applyArticleSlug(values)
    const nextErrors = validateArticleForm(draft)
    setErrors(nextErrors)
    if (Object.keys(nextErrors).length > 0) return

    setIsSaving(true)
    try {
      const payload = toArticlePayload(draft)
      if (isNew) await contentArticlesApi.createContentArticle(payload)
      else await contentArticlesApi.updateContentArticle(draft.id, payload)
      showNotification({
        title: 'Thành công',
        message: isNew ? 'Đã tạo bài viết' : 'Đã cập nhật bài viết',
        variant: 'success',
        delay: 2500,
      })
      navigate('/website/articles')
    } catch (e) {
      showNotification({
        title: 'Lỗi',
        message: getErrorMessage(e, 'Không lưu được bài viết'),
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
    categoryOptions,
    isNew,
    isLoading,
    isSaving,
    isActioning: isNew ? false : isActioning,
    loadFailed,
    pageTitle: isNew ? 'Thêm bài viết mới' : 'Sửa bài viết',
    submitLabel: isNew ? 'Tạo bài viết' : 'Lưu thay đổi',
    updateField,
    cancel,
    submit,
    publish,
    hide,
    feature,
    unfeature,
  }
}
