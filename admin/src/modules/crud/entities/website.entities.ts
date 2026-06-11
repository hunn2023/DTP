import { defineAdminEntity } from '@/modules/crud/schema/defineEntity'
import { labels } from '@/modules/crud/entities/shared'
import { isActiveField, sortOrderField } from '@/modules/crud/entities/fieldHelpers'
import { countriesData } from '@/features/master-data/countries/data'
import type { Country } from '@/features/master-data/types'
import type { EntityFieldDef } from '@/modules/crud/types'
import type { SettingsEntityBase } from '@/modules/crud/types'

const contentSub = 'Website & SEO'

const countryOptions = countriesData.map((c: Country) => ({ value: String(c.id), label: c.name }))

export type Post = SettingsEntityBase & {
  title: string
  slug: string
  thumbnailUrl: string
  summary: string
  content: string
  status: string
  publishedAt: string
  seoTitle: string
  seoDescription: string
}

export type PostCategory = SettingsEntityBase & {
  name: string
  slug: string
  description: string
  sortOrder: number
}

export type StaticPage = SettingsEntityBase & {
  title: string
  slug: string
  content: string
  status: string
  seoTitle: string
  seoDescription: string
}

export type WebsiteFaq = SettingsEntityBase & {
  question: string
  answer: string
  targetType: string
  targetId: number
  sortOrder: number
}

export type Banner = SettingsEntityBase & {
  title: string
  imageDesktopUrl: string
  imageMobileUrl: string
  linkUrl: string
  position: string
  startDate: string
  endDate: string
}

export type MediaFile = SettingsEntityBase & {
  fileName: string
  url: string
  mimeType: string
  size: number
  folder: string
  createdAt: string
}

export type SeoSetting = SettingsEntityBase & {
  pageKey: string
  seoTitle: string
  seoDescription: string
  seoKeywords: string
  canonicalUrl: string
  schemaJson: string
}

export type CountrySeo = SettingsEntityBase & {
  countryId: number
  seoTitle: string
  seoDescription: string
  contentHtml: string
  canonicalUrl: string
}

export type ProductSeo = SettingsEntityBase & {
  productType: string
  productId: number
  seoTitle: string
  seoDescription: string
  canonicalUrl: string
}

export const postsEntity = defineAdminEntity<Post>({
  path: '/website/content/posts',
  title: 'Bài viết',
  breadcrumbSubtitle: contentSub,
  description: 'Blog SEO kéo traffic.',
  entityName: 'bài viết',
  labels: labels('bài viết', 'Tìm tiêu đề...', 'Thêm bài'),
  slugFromName: true,
  seedData: [
    {
      id: 1,
      title: 'Cách cài eSIM trên iPhone',
      slug: 'cach-cai-esim-iphone',
      thumbnailUrl: 'https://cdn.example/thumb/1.jpg',
      summary: 'Hướng dẫn cài eSIM nhanh trên iPhone.',
      content: '<p>Nội dung bài viết...</p>',
      status: 'Published',
      publishedAt: '2026-05-01',
      seoTitle: 'Cài eSIM iPhone',
      seoDescription: 'Hướng dẫn chi tiết cài eSIM.',
      isActive: true,
    },
  ],
  fields: [
    { name: 'title', label: 'Tiêu đề', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'slug', label: 'Slug', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'thumbnailUrl', label: 'Ảnh thumbnail', type: 'url', table: true, form: { col: 6 } },
    { name: 'summary', label: 'Tóm tắt', type: 'textarea', form: { col: 6 } },
    { name: 'content', label: 'Nội dung', type: 'textarea', form: { required: true } },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'publishedAt', label: 'Ngày đăng', type: 'text', table: true, form: { col: 6 } },
    { name: 'seoTitle', label: 'SEO title', type: 'text', form: { col: 6 } },
    { name: 'seoDescription', label: 'SEO description', type: 'textarea', form: { col: 6 } },
    isActiveField<Post>(),
  ],
})

export const postCategoriesEntity = defineAdminEntity<PostCategory>({
  path: '/website/content/post-categories',
  title: 'Danh mục bài viết',
  breadcrumbSubtitle: contentSub,
  description: 'Danh mục blog.',
  entityName: 'danh mục',
  labels: labels('danh mục', 'Tìm tên...', 'Thêm danh mục'),
  slugFromName: true,
  seedData: [{ id: 1, name: 'Hướng dẫn eSIM', slug: 'huong-dan-esim', description: '', sortOrder: 1, isActive: true }],
  fields: [
    { name: 'name', label: 'Tên', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'slug', label: 'Slug', type: 'text', form: { required: true } },
    { name: 'description', label: 'Mô tả', type: 'textarea' },
    sortOrderField<PostCategory>(),
    isActiveField<PostCategory>(),
  ],
})

export const pagesEntity = defineAdminEntity<StaticPage>({
  path: '/website/content/pages',
  title: 'Trang tĩnh',
  breadcrumbSubtitle: contentSub,
  description: 'Chính sách, điều khoản.',
  entityName: 'trang',
  labels: labels('trang', 'Tìm tiêu đề...', 'Thêm trang'),
  slugFromName: true,
  seedData: [
    {
      id: 1,
      title: 'Chính sách hoàn tiền',
      slug: 'chinh-sach-hoan-tien',
      content: '<p>Nội dung trang...</p>',
      status: 'Published',
      seoTitle: 'Chính sách hoàn tiền',
      seoDescription: 'Điều khoản hoàn tiền ezSim',
      isActive: true,
    },
  ],
  fields: [
    { name: 'title', label: 'Tiêu đề', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'slug', label: 'Slug', type: 'text', form: { required: true } },
    { name: 'content', label: 'Nội dung', type: 'textarea', form: { required: true } },
    { name: 'status', label: 'Trạng thái', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'seoTitle', label: 'SEO title', type: 'text', form: { col: 6 } },
    { name: 'seoDescription', label: 'SEO description', type: 'textarea' },
    isActiveField<StaticPage>(),
  ],
})

export const websiteFaqsEntity = defineAdminEntity<WebsiteFaq>({
  path: '/website/content/faqs',
  title: 'FAQ',
  breadcrumbSubtitle: contentSub,
  description: 'FAQ chung website.',
  entityName: 'FAQ',
  labels: labels('FAQ', 'Tìm câu hỏi...', 'Thêm FAQ'),
  seedData: [
    {
      id: 1,
      question: 'eSIM dùng được trên máy nào?',
      answer: 'iPhone XS trở lên, Samsung S20+...',
      targetType: 'Global',
      targetId: 0,
      sortOrder: 1,
      isActive: true,
    },
  ],
  fields: [
    { name: 'question', label: 'Câu hỏi', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'answer', label: 'Trả lời', type: 'textarea', form: { required: true } },
    { name: 'targetType', label: 'Loại đối tượng', type: 'text', table: true, form: { col: 6 } },
    { name: 'targetId', label: 'ID đối tượng', type: 'number', table: true, form: { col: 6 } },
    sortOrderField<WebsiteFaq>(),
    isActiveField<WebsiteFaq>(),
  ],
})

export const bannersEntity = defineAdminEntity<Banner>({
  path: '/website/content/banners',
  title: 'Banner',
  breadcrumbSubtitle: contentSub,
  description: 'Homepage / landing.',
  entityName: 'banner',
  labels: labels('banner', 'Tìm tiêu đề...', 'Thêm banner'),
  seedData: [
    {
      id: 1,
      title: 'Sale eSIM Nhật',
      imageDesktopUrl: 'https://cdn.example/banner-desktop.jpg',
      imageMobileUrl: 'https://cdn.example/banner-mobile.jpg',
      linkUrl: '/esim/japan',
      position: 'HomeHero',
      startDate: '2026-06-01',
      endDate: '2026-06-30',
      isActive: true,
    },
  ],
  fields: [
    { name: 'title', label: 'Tiêu đề', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'imageDesktopUrl', label: 'Ảnh desktop', type: 'url', table: true, form: { required: true } },
    { name: 'imageMobileUrl', label: 'Ảnh mobile', type: 'url', table: true, form: { col: 6 } },
    { name: 'linkUrl', label: 'Link', type: 'url', form: { required: true, col: 6 } },
    { name: 'position', label: 'Vị trí', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'startDate', label: 'Từ ngày', type: 'text', form: { col: 6 } },
    { name: 'endDate', label: 'Đến ngày', type: 'text', form: { col: 6 } },
    isActiveField<Banner>(),
  ],
})

export const mediaEntity = defineAdminEntity<MediaFile>({
  path: '/website/content/media',
  title: 'Media',
  breadcrumbSubtitle: contentSub,
  description: 'Thư viện file.',
  entityName: 'file',
  labels: labels('file', 'Tìm tên file...', 'Upload'),
  seedData: [
    {
      id: 1,
      fileName: 'japan-banner.webp',
      url: 'https://cdn.example/japan-banner.webp',
      mimeType: 'image/webp',
      size: 245760,
      folder: 'banners',
      createdAt: '2026-05-01 10:00',
      isActive: true,
    },
  ],
  fields: [
    { name: 'fileName', label: 'Tên file', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'url', label: 'URL', type: 'url', table: true, form: { required: true } },
    { name: 'mimeType', label: 'MIME type', type: 'text', table: true, form: { col: 6 } },
    { name: 'size', label: 'Dung lượng', type: 'number', table: true, form: { col: 6 } },
    { name: 'folder', label: 'Thư mục', type: 'text', table: true, form: { col: 6 } },
    { name: 'createdAt', label: 'Ngày tạo', type: 'text', table: true, form: false },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const seoHomepageEntity = defineAdminEntity<SeoSetting>({
  path: '/website/seo/homepage',
  title: 'SEO trang chủ',
  breadcrumbSubtitle: contentSub,
  description: 'SeoSettings — trang chủ.',
  entityName: 'cấu hình SEO',
  labels: labels('SEO', 'Tìm page key...', 'Lưu SEO'),
  seedData: [
    {
      id: 1,
      pageKey: 'homepage',
      seoTitle: 'ezSim - eSIM du lịch giá tốt',
      seoDescription: 'Mua eSIM quốc tế online, nhận QR ngay.',
      seoKeywords: 'esim, du lich, quoc te',
      canonicalUrl: 'https://ezsim.vn/',
      schemaJson: '{"@type":"WebSite"}',
      isActive: true,
    },
  ],
  fields: [
    { name: 'pageKey', label: 'Page key', type: 'text', table: { variant: 'code' }, form: { required: true } },
    { name: 'seoTitle', label: 'SEO title', type: 'text', form: { required: true } },
    { name: 'seoDescription', label: 'SEO description', type: 'textarea', form: { required: true } },
    { name: 'seoKeywords', label: 'SEO keywords', type: 'text', form: { col: 6 } },
    { name: 'canonicalUrl', label: 'Canonical URL', type: 'url', form: { col: 6 } },
    { name: 'schemaJson', label: 'Schema JSON', type: 'textarea' },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const seoCategoriesEntity = defineAdminEntity<SeoSetting>({
  path: '/website/seo/categories',
  title: 'SEO danh mục',
  breadcrumbSubtitle: contentSub,
  description: 'SEO danh mục sản phẩm.',
  entityName: 'SEO',
  labels: labels('SEO', 'Tìm...', 'Thêm'),
  seedData: seoHomepageEntity.seedData.map((s) => ({ ...s, id: 2, pageKey: 'category-esim' })),
  fields: seoHomepageEntity.fields as EntityFieldDef<SeoSetting>[],
})

export const seoCountriesEntity = defineAdminEntity<CountrySeo>({
  path: '/website/seo/countries',
  title: 'SEO quốc gia',
  breadcrumbSubtitle: contentSub,
  description: 'Landing SEO theo quốc gia.',
  entityName: 'SEO quốc gia',
  labels: labels('SEO', 'Tìm quốc gia...', 'Thêm SEO'),
  seedData: [
    {
      id: 1,
      countryId: 1,
      seoTitle: 'eSIM Nhật Bản giá rẻ',
      seoDescription: 'Gói data Nhật 5G, kích hoạt nhanh.',
      contentHtml: '<h1>eSIM Nhật Bản</h1>',
      canonicalUrl: 'https://ezsim.vn/esim/japan',
      isActive: true,
    },
  ],
  fields: [
    {
      name: 'countryId',
      label: 'Quốc gia',
      type: 'select',
      table: true,
      form: { required: true, parseAsNumber: true, options: countryOptions },
    },
    { name: 'seoTitle', label: 'SEO title', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'seoDescription', label: 'SEO description', type: 'textarea', form: { required: true } },
    { name: 'contentHtml', label: 'Nội dung (HTML)', type: 'textarea', form: { required: true } },
    { name: 'canonicalUrl', label: 'Canonical URL', type: 'url', table: true, form: { col: 6 } },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const seoProductsEntity = defineAdminEntity<ProductSeo>({
  path: '/website/seo/products',
  title: 'SEO sản phẩm',
  breadcrumbSubtitle: contentSub,
  description: 'SEO theo ProductType + ProductId.',
  entityName: 'SEO SP',
  labels: labels('SEO', 'Tìm ProductId...', 'Thêm SEO'),
  seedData: [
    {
      id: 1,
      productType: 'EsimPackage',
      productId: 1,
      seoTitle: 'eSIM Japan 5GB 7 days',
      seoDescription: 'Buy Japan eSIM online',
      canonicalUrl: 'https://ezsim.vn/esim/jp-5gb-7d',
      isActive: true,
    },
  ],
  fields: [
    { name: 'productType', label: 'Loại sản phẩm', type: 'text', table: true, form: { required: true, col: 6 } },
    { name: 'productId', label: 'ID sản phẩm', type: 'number', table: true, form: { required: true, col: 6 } },
    { name: 'seoTitle', label: 'SEO title', type: 'text', table: { variant: 'primary' }, form: { required: true } },
    { name: 'seoDescription', label: 'SEO description', type: 'textarea', form: { required: true } },
    { name: 'canonicalUrl', label: 'Canonical URL', type: 'url', table: true, form: { col: 6 } },
    { name: 'isActive', label: '—', type: 'checkbox', form: false, table: false },
  ],
})

export const websiteEntities = [
  postsEntity,
  postCategoriesEntity,
  pagesEntity,
  websiteFaqsEntity,
  bannersEntity,
  mediaEntity,
  seoHomepageEntity,
  seoCategoriesEntity,
  seoCountriesEntity,
  seoProductsEntity,
]
