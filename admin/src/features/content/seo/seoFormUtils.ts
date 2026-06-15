import type { SeoMetadataPayload } from '@/apis/seoMetadataApi'
import type { SeoMetadata } from '@/features/content/types'

export function getDefaultSeoValues(): SeoMetadata {
  return {
    id: '',
    entityType: '',
    entityId: '',
    routePath: '',
    metaTitle: '',
    metaDescription: '',
    metaKeywords: '',
    canonicalUrl: '',
    ogTitle: '',
    ogDescription: '',
    ogImageUrl: '',
    robots: 'index,follow',
    createdAt: '',
    updatedAt: '',
  }
}

export function toSeoPayload(values: SeoMetadata): SeoMetadataPayload {
  return {
    entityType: values.entityType.trim(),
    entityId: values.entityId.trim() || undefined,
    routePath: values.routePath.trim() || undefined,
    metaTitle: values.metaTitle.trim(),
    metaDescription: values.metaDescription.trim() || undefined,
    metaKeywords: values.metaKeywords.trim() || undefined,
    canonicalUrl: values.canonicalUrl.trim() || undefined,
    ogTitle: values.ogTitle.trim() || undefined,
    ogDescription: values.ogDescription.trim() || undefined,
    ogImageUrl: values.ogImageUrl.trim() || undefined,
    robots: values.robots.trim() || 'index,follow',
  }
}

export type SeoFormErrors = Partial<Record<keyof SeoMetadata, string>>

export function validateSeoForm(values: SeoMetadata): SeoFormErrors {
  const errors: SeoFormErrors = {}
  if (!values.entityType.trim()) errors.entityType = 'Nhập entity type'
  if (!values.metaTitle.trim()) errors.metaTitle = 'Nhập meta title'
  if (!values.entityId.trim() && !values.routePath.trim()) {
    errors.routePath = 'Cần entityId hoặc routePath'
  }
  return errors
}
