import type { ContentBanner, ContentBannerPosition } from '@/features/content/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readBool,
  readDateString,
  readNumber,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPatch, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type ContentBannerDto = {
  id: string
  title: string
  imageUrl: string
  mobileImageUrl: string
  linkUrl: string
  description: string
  position: ContentBannerPosition
  startDate: string
  endDate: string
  sortOrder: number
  isActive: boolean
}

export type ContentBannerPayload = {
  title: string
  imageUrl: string
  mobileImageUrl?: string
  linkUrl?: string
  description?: string
  position: ContentBannerPosition
  startDate?: string
  endDate?: string
  sortOrder: number
  isActive: boolean
}

export type ContentBannerUpdatePayload = ContentBannerPayload

export type PagedContentBannersDto = {
  items: ContentBannerDto[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export type ContentBannersPageResult = {
  items: ContentBanner[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const CONTENT_BANNER_PAGE_SIZE_OPTIONS = [5, 10, 15, 20] as const

function normalizeDto(raw: Raw): ContentBannerDto {
  return {
    id: readString(raw, 'id', 'Id'),
    title: readString(raw, 'title', 'Title'),
    imageUrl: readString(raw, 'imageUrl', 'ImageUrl'),
    mobileImageUrl: readString(raw, 'mobileImageUrl', 'MobileImageUrl'),
    linkUrl: readString(raw, 'linkUrl', 'LinkUrl'),
    description: readString(raw, 'description', 'Description'),
    position: readNumber(raw, 'position', 'Position') as ContentBannerPosition,
    startDate: readDateString(raw, 'startDate', 'StartDate'),
    endDate: readDateString(raw, 'endDate', 'EndDate'),
    sortOrder: readNumber(raw, 'sortOrder', 'SortOrder'),
    isActive: readBool(raw, 'isActive', 'IsActive'),
  }
}

function mapDto(dto: ContentBannerDto): ContentBanner {
  return { ...dto }
}

const inflightPages = new Map<string, Promise<ContentBannersPageResult>>()

export async function fetchContentBannersPage(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
  isActive?: boolean,
): Promise<ContentBannersPageResult> {
  const key = `${pageIndex}:${pageSize}:${keyword ?? ''}:${isActive ?? ''}`
  const cached = inflightPages.get(key)
  if (cached) return cached

  const request = fetchAdminContentBanners(pageIndex, pageSize, keyword, isActive).then((paged) => ({
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

export async function fetchAdminContentBanners(
  pageIndex = 1,
  pageSize = 10,
  keyword?: string,
  isActive?: boolean,
): Promise<PagedContentBannersDto> {
  const data = await httpGet<Raw>(API_PATHS.adminContentBanners, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      isActive,
    },
  })
  return normalizePaged(data, normalizeDto)
}

export async function fetchContentBannerById(id: string): Promise<ContentBanner> {
  const data = await httpGet<Raw>(`${API_PATHS.adminContentBanners}/${id}`)
  return mapDto(normalizeDto(data))
}

export async function createContentBanner(payload: ContentBannerPayload): Promise<ContentBanner> {
  const dto = await httpPost<Raw>(API_PATHS.adminContentBanners, payload)
  return mapDto(normalizeDto(dto))
}

export async function updateContentBanner(
  id: string,
  payload: ContentBannerUpdatePayload,
): Promise<ContentBanner> {
  const dto = await httpPut<Raw>(`${API_PATHS.adminContentBanners}/${id}`, payload)
  return mapDto(normalizeDto(dto))
}

export async function enableContentBanner(id: string): Promise<ContentBanner> {
  const dto = await httpPatch<Raw>(`${API_PATHS.adminContentBanners}/${id}/enable`, {})
  return mapDto(normalizeDto(dto))
}

export async function disableContentBanner(id: string): Promise<ContentBanner> {
  const dto = await httpPatch<Raw>(`${API_PATHS.adminContentBanners}/${id}/disable`, {})
  return mapDto(normalizeDto(dto))
}

export async function deleteContentBanner(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminContentBanners}/${id}`)
}
