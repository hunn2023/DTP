import type { ContentFaq } from '@/features/content/types'
import { API_PATHS } from '@/shared/config/api'
import { normalizePaged, readBool, readNumber, readString } from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPatch, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ContentFaqDto = {
  id: string
  question: string
  answer: string
  categoryCode: string
  sortOrder: number
  isActive: boolean
}

export type ContentFaqPayload = {
  question: string
  answer: string
  categoryCode?: string
  sortOrder: number
  isActive: boolean
}

export type ContentFaqUpdatePayload = ContentFaqPayload

export type PagedContentFaqsDto = {
  items: ContentFaqDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export type ContentFaqsPageResult = {
  items: ContentFaq[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const CONTENT_FAQ_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

function normalizeDto(raw: Raw): ContentFaqDto {
  return {
    id: readString(raw, 'id', 'Id'),
    question: readString(raw, 'question', 'Question'),
    answer: readString(raw, 'answer', 'Answer'),
    categoryCode: readString(raw, 'categoryCode', 'CategoryCode'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function mapDto(dto: ContentFaqDto): ContentFaq {
  return { ...dto }
}

const inflightPages = new Map<string, Promise<ContentFaqsPageResult>>()

export async function fetchContentFaqsPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
  isActive?: boolean,
): Promise<ContentFaqsPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}:${isActive ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminContentFaqs(pageIndex, pageSize, keyword, isActive).then((paged) => ({
    items: paged.items.map(mapDto),
    totalCount: paged.totalCount,
    pageIndex: paged.pageIndex,
    pageSize: paged.pageSize,
  }))

  inflightPages.set(key, request)
  try {
    return await request
  } finally {
    inflightPages.delete(key)
  }
}

export async function fetchAdminContentFaqs(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
  isActive?: boolean,
): Promise<PagedContentFaqsDto> {
  const data = await httpGet<Raw>(API_PATHS.adminContentFaqs, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      isActive,
    },
  })
  return normalizePaged(data, normalizeDto)
}

export async function createContentFaq(payload: ContentFaqPayload): Promise<ContentFaq> {
  const dto = await httpPost<Raw>(API_PATHS.adminContentFaqs, payload)
  return mapDto(normalizeDto(dto))
}

export async function updateContentFaq(id: string, payload: ContentFaqUpdatePayload): Promise<ContentFaq> {
  const dto = await httpPut<Raw>(`${API_PATHS.adminContentFaqs}/${id}`, payload)
  return mapDto(normalizeDto(dto))
}

export async function enableContentFaq(id: string): Promise<ContentFaq> {
  const dto = await httpPatch<Raw>(`${API_PATHS.adminContentFaqs}/${id}/enable`, {})
  return mapDto(normalizeDto(dto))
}

export async function disableContentFaq(id: string): Promise<ContentFaq> {
  const dto = await httpPatch<Raw>(`${API_PATHS.adminContentFaqs}/${id}/disable`, {})
  return mapDto(normalizeDto(dto))
}

export async function deleteContentFaq(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminContentFaqs}/${id}`)
}
