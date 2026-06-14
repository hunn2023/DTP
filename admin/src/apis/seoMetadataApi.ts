import type { SeoMetadata } from '@/features/content/types'
import { API_PATHS } from '@/shared/config/api'
import {
  normalizePaged,
  readDateString,
  readString,
} from '@/shared/lib/dtoNormalize'
import { httpDelete, httpGet, httpPost, httpPut } from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type SeoMetadataPayload = {
  entityType: string
  entityId?: string
  routePath?: string
  metaTitle: string
  metaDescription?: string
  metaKeywords?: string
  canonicalUrl?: string
  ogTitle?: string
  ogDescription?: string
  ogImageUrl?: string
  robots?: string
}

export type SeoMetadataPageResult = {
  items: SeoMetadata[]
  totalCount: number
  pageIndex: number
  pageSize: number
}

export const SEO_METADATA_PAGE_SIZE_OPTIONS = [6, 9, 12, 18] as const

export type SeoListFilters = {
  keyword?: string
  entityType?: string
}

function normalizeSeo(raw: Raw): SeoMetadata {
  return {
    id: readString(raw, 'id', 'Id'),
    entityType: readString(raw, 'entityType', 'EntityType'),
    entityId: readString(raw, 'entityId', 'EntityId'),
    routePath: readString(raw, 'routePath', 'RoutePath'),
    metaTitle: readString(raw, 'metaTitle', 'MetaTitle'),
    metaDescription: readString(raw, 'metaDescription', 'MetaDescription'),
    metaKeywords: readString(raw, 'metaKeywords', 'MetaKeywords'),
    canonicalUrl: readString(raw, 'canonicalUrl', 'CanonicalUrl'),
    ogTitle: readString(raw, 'ogTitle', 'OgTitle'),
    ogDescription: readString(raw, 'ogDescription', 'OgDescription'),
    ogImageUrl: readString(raw, 'ogImageUrl', 'OgImageUrl'),
    robots: readString(raw, 'robots', 'Robots') || 'index,follow',
    createdAt: readDateString(raw, 'createdAt', 'CreatedAt'),
    updatedAt: readDateString(raw, 'updatedAt', 'UpdatedAt'),
  }
}

export async function fetchSeoMetadataPage(
  pageIndex = 1,
  pageSize = 10,
  filters: SeoListFilters = {},
): Promise<SeoMetadataPageResult> {
  const { keyword, entityType } = filters
  const data = await httpGet<Raw>(API_PATHS.adminContentSeo, {
    params: {
      pageIndex,
      pageSize,
      keyword: keyword?.trim() || undefined,
      entityType: entityType?.trim() || undefined,
    },
  })
  const paged = normalizePaged(data, normalizeSeo)
  return {
    items: paged.items,
    totalCount: paged.totalCount,
    pageIndex: paged.pageIndex,
    pageSize: paged.pageSize,
  }
}

export async function fetchSeoMetadataById(id: string): Promise<SeoMetadata> {
  const data = await httpGet<Raw>(`${API_PATHS.adminContentSeo}/${id}`)
  return normalizeSeo(data)
}

export async function createSeoMetadata(payload: SeoMetadataPayload): Promise<SeoMetadata> {
  const data = await httpPost<Raw>(API_PATHS.adminContentSeo, payload)
  return normalizeSeo(data)
}

export async function updateSeoMetadata(id: string, payload: SeoMetadataPayload): Promise<SeoMetadata> {
  const data = await httpPut<Raw>(`${API_PATHS.adminContentSeo}/${id}`, payload)
  return normalizeSeo(data)
}

export async function deleteSeoMetadata(id: string): Promise<void> {
  await httpDelete(`${API_PATHS.adminContentSeo}/${id}`)
}
