import { slugify } from '@/modules/crud/form/slugify'
import type { ContentPageCreatePayload, ContentPageUpdatePayload } from '@/apis/contentPagesApi'
import type { ContentPage } from '@/features/content/types'

export function getDefaultPageValues(): ContentPage {
  return {
    id: '',
    code: '',
    title: '',
    slug: '',
    summary: '',
    content: '',
    thumbnailUrl: '',
    status: 0,
    sortOrder: 0,
    createdAt: '',
    publishedAt: '',
  }
}

export function applyPageSlug(values: ContentPage): ContentPage {
  const next = { ...values }
  if (!next.slug.trim() && next.title.trim()) next.slug = slugify(next.title)
  if (!next.code.trim() && next.title.trim()) next.code = slugify(next.title).replace(/-/g, '_')
  return next
}

export function toPageCreatePayload(values: ContentPage): ContentPageCreatePayload {
  return {
    code: values.code.trim(),
    title: values.title.trim(),
    slug: values.slug.trim(),
    summary: values.summary.trim() || undefined,
    content: values.content,
    status: values.status,
    sortOrder: values.sortOrder,
  }
}

export function toPageUpdatePayload(values: ContentPage): ContentPageUpdatePayload {
  return {
    title: values.title.trim(),
    slug: values.slug.trim(),
    summary: values.summary.trim() || undefined,
    content: values.content,
    thumbnailUrl: values.thumbnailUrl.trim() || undefined,
    status: values.status,
    sortOrder: values.sortOrder,
  }
}

export function validatePageForm(values: ContentPage, isNew: boolean): Partial<Record<keyof ContentPage, string>> {
  const errors: Partial<Record<keyof ContentPage, string>> = {}
  if (isNew && !values.code.trim()) errors.code = 'Vui lòng nhập mã trang'
  if (!values.title.trim()) errors.title = 'Vui lòng nhập tiêu đề'
  if (!values.slug.trim()) errors.slug = 'Vui lòng nhập slug'
  if (!values.content.trim()) errors.content = 'Vui lòng nhập nội dung'
  return errors
}
