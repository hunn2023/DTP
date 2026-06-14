import type { ContentPage, ContentPublishStatus } from '@/features/content/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readDateString,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ContentPageCreatePayload = {
  code: string
  title: string
  slug: string
  summary?: string
  content: string
  status: ContentPublishStatus
  sortOrder: number
}

export type ContentPageUpdatePayload = {
  title: string
  slug: string
  summary?: string
  content: string
  thumbnailUrl?: string
  status: ContentPublishStatus
  sortOrder: number
}

export type ContentPagesPageResult = {
  items: ContentPage[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const CONTENT_PAGE_PAGE_SIZE_OPTIONS = [6, 9, 12, 18] as const

export type PageListFilters = {
  keyword?: string
  status?: ContentPublishStatus
}

function normalizePage(raw: Raw): ContentPage {
  return {
    id: readString(raw, 'id', 'Id'),
    code: readString(raw, 'code', 'Code'),
    title: readString(raw, 'title', 'Title'),
    slug: readString(raw, 'slug', 'Slug'),
    summary: readString(raw, 'summary', 'Summary'),
    content: readString(raw, 'content', 'Content'),
    thumbnailUrl: readString(raw, 'thumbnailUrl', 'ThumbnailUrl'),
    status: readNumber(raw, 'status', 'Status') as ContentPublishStatus,
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    createdAt: readDateString(raw, 'createdAt', 'CreatedAt'),
    publishedAt: readDateString(raw, 'publishedAt', 'PublishedAt'),
  }
}

export async function fetchContentPagesPage(
  pageIndex = 1,
  pageSize = 10,
  filters: PageListFilters = {},
): Promise<ContentPagesPageResult> {
  const { keyword, status } = filters
  const data = await httpGet<Raw>(API_PATHS.adminContentPages, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      status,
    },
  })
  const paged = normalizePaged(data, normalizePage)
  return {
    items: paged.items,
    totalCount: paged.totalCount,
    pageIndex: paged.pageIndex,
    pageSize: paged.pageSize,
  }
}

export async function fetchContentPageById(id: string): Promise<ContentPage> {
  const data = await httpGet<Raw>(`${API_PATHS.adminContentPages}/${id}`)
  return normalizePage(data)
}

export async function createContentPage(payload: ContentPageCreatePayload): Promise<ContentPage> {
  const data = await httpPost<Raw>(API_PATHS.adminContentPages, payload)
  return normalizePage(data)
}

export async function updateContentPage(
  id: string,
  payload: ContentPageUpdatePayload,
): Promise<ContentPage> {
  const data = await httpPut<Raw>(`${API_PATHS.adminContentPages}/${id}`, payload)
  return normalizePage(data)
}

export async function deleteContentPage(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminContentPages}/${id}`)
}
