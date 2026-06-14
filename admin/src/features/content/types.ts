export type ContentBannerPosition = 1 | 2 | 3 | 4 | 5 | 6

export type ContentBanner = {
  id: string
  isActive: boolean
  title: string
  imageUrl: string
  mobileImageUrl: string
  linkUrl: string
  description: string
  position: ContentBannerPosition
  startDate: string
  endDate: string
  sortOrder: number
}

export type ContentFaq = {
  id: string
  isActive: boolean
  question: string
  answer: string
  categoryCode: string
  sortOrder: number
}

export type ContentPublishStatus = 0 | 1 | 2

export type ContentArticleListItem = {
  id: string
  title: string
  slug: string
  summary: string
  thumbnailUrl: string
  authorName: string
  categoryCode: string
  tags: string
  status: ContentPublishStatus
  isFeatured: boolean
  sortOrder: number
  viewCount: number
  createdAt: string
  publishedAt: string
}

export type ContentArticle = ContentArticleListItem & {
  content: string
}

export type ContentPage = {
  id: string
  code: string
  title: string
  slug: string
  summary: string
  content: string
  thumbnailUrl: string
  status: ContentPublishStatus
  sortOrder: number
  createdAt: string
  publishedAt: string
}

export type SeoMetadata = {
  id: string
  entityType: string
  entityId: string
  routePath: string
  metaTitle: string
  metaDescription: string
  metaKeywords: string
  canonicalUrl: string
  ogTitle: string
  ogDescription: string
  ogImageUrl: string
  robots: string
  createdAt: string
  updatedAt: string
}
