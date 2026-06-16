import type { ContentArticle, ContentArticleListItem, ContentPublishStatus } from '@/features/content/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readBool,
  readDateString,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPatch, httpPost, httpPostForm, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ContentArticlePayload = {
  title: string
  slug: string
  summary?: string
  content: string
  thumbnailUrl?: string
  authorName?: string
  categoryCode?: string
  tags?: string
  status: ContentPublishStatus
  isFeatured: boolean
  sortOrder: number
}

export type ArticlesPageResult = {
  items: ContentArticleListItem[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const CONTENT_ARTICLE_PAGE_SIZE_OPTIONS = [6, 9, 12, 18] as const

export type ArticleListFilters = {
  keyword?: string
  categoryCode?: string
  status?: ContentPublishStatus
  isFeatured?: boolean
}

function normalizeListItem(raw: Raw): ContentArticleListItem {
  return {
    id: readString(raw, 'id', 'Id'),
    title: readString(raw, 'title', 'Title'),
    slug: readString(raw, 'slug', 'Slug'),
    summary: readString(raw, 'summary', 'Summary'),
    thumbnailUrl: readString(raw, 'thumbnailUrl', 'ThumbnailUrl'),
    authorName: readString(raw, 'authorName', 'AuthorName'),
    categoryCode: readString(raw, 'categoryCode', 'CategoryCode'),
    tags: readString(raw, 'tags', 'Tags'),
    status: readNumber(raw, 'status', 'Status') as ContentPublishStatus,
    isFeatured: readBool(raw, 'isFeatured', 'IsFeatured'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    viewCount: readNumber(raw, 'viewCount', 'ViewCount'),
    createdAt: readDateString(raw, 'createdAt', 'CreatedAt'),
    publishedAt: readDateString(raw, 'publishedAt', 'PublishedAt'),
  }
}

function normalizeArticle(raw: Raw): ContentArticle {
  return {
    ...normalizeListItem(raw),
    content: readString(raw, 'content', 'Content'),
  }
}

export async function fetchContentArticlesPage(
  pageIndex = 1,
  pageSize = 10,
  filters: ArticleListFilters = {},
): Promise<ArticlesPageResult> {
  const { keyword, categoryCode, status, isFeatured } = filters
  const data = await httpGet<Raw>(API_PATHS.adminContentArticles, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      categoryCode: categoryCode?.trim() || undefined,
      status,
      isFeatured,
    },
  })
  const paged = normalizePaged(data, normalizeListItem)
  return {
    items: paged.items,
    totalCount: paged.totalCount,
    pageIndex: paged.pageIndex,
    pageSize: paged.pageSize,
  }
}

export async function fetchContentArticleById(id: string): Promise<ContentArticle> {
  const data = await httpGet<Raw>(`${API_PATHS.adminContentArticles}/${id}`)
  return normalizeArticle(data)
}

export async function createContentArticle(payload: ContentArticlePayload): Promise<ContentArticle> {
  const data = await httpPost<Raw>(API_PATHS.adminContentArticles, payload)
  return normalizeArticle(data)
}

export async function updateContentArticle(
  id: string,
  payload: ContentArticlePayload,
): Promise<ContentArticle> {
  const data = await httpPut<Raw>(`${API_PATHS.adminContentArticles}/${id}`, payload)
  return normalizeArticle(data)
}

export async function publishContentArticle(id: string): Promise<void> {
  await httpPatch(`${API_PATHS.adminContentArticles}/${id}/publish`, {})
}

export async function hideContentArticle(id: string): Promise<void> {
  await httpPatch(`${API_PATHS.adminContentArticles}/${id}/hide`, {})
}

export async function featureContentArticle(id: string): Promise<void> {
  await httpPatch(`${API_PATHS.adminContentArticles}/${id}/featured`, {})
}

export async function unfeatureContentArticle(id: string): Promise<void> {
  await httpPatch(`${API_PATHS.adminContentArticles}/${id}/unfeatured`, {})
}

export async function uploadContentArticleThumbnail(
  articleId: string,
  file: File,
): Promise<ContentArticle> {
  const formData = new FormData()
  formData.append('file', file)

  const data = await httpPostForm<Raw>(
    `${API_PATHS.adminContentArticles}/${articleId}/thumbnail`,
    formData,
  )
  return normalizeArticle(data)
}

export async function deleteContentArticle(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminContentArticles}/${id}`)
}
