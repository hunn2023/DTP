import { slugify } from '@/modules/crud/form/slugify'
import type { ContentArticlePayload } from '@/apis/contentArticlesApi'
import type { ContentArticle } from '@/features/content/types'

export function getDefaultArticleValues(): ContentArticle {
  return {
    id: '',
    title: '',
    slug: '',
    summary: '',
    content: '',
    thumbnailUrl: '',
    authorName: '',
    categoryCode: '',
    tags: '',
    status: 0,
    isFeatured: false,
    sortOrder: 0,
    viewCount: 0,
    createdAt: '',
    publishedAt: '',
  }
}

export function applyArticleSlug(values: ContentArticle): ContentArticle {
  if (!values.slug.trim() && values.title.trim()) {
    return { ...values, slug: slugify(values.title) }
  }
  return values
}

export function toArticlePayload(values: ContentArticle): ContentArticlePayload {
  return {
    title: values.title.trim(),
    slug: values.slug.trim(),
    summary: values.summary.trim() || undefined,
    content: values.content,
    thumbnailUrl: values.thumbnailUrl.trim() || undefined,
    authorName: values.authorName.trim() || undefined,
    categoryCode: values.categoryCode.trim() || undefined,
    tags: values.tags.trim() || undefined,
    status: values.status,
    isFeatured: values.isFeatured,
    sortOrder: values.sortOrder,
  }
}

export type ArticleFormErrors = Partial<Record<keyof ContentArticle, string>>

export function validateArticleForm(values: ContentArticle): ArticleFormErrors {
  const errors: ArticleFormErrors = {}
  if (!values.title.trim()) errors.title = 'Vui lòng nhập tiêu đề'
  if (!values.slug.trim()) errors.slug = 'Vui lòng nhập slug'
  if (!values.content.trim()) errors.content = 'Vui lòng nhập nội dung bài viết'
  return errors
}
