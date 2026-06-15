import type { ContentBannerPayload } from '@/apis/contentBannersApi'
import type { ContentBanner } from '@/features/content/types'

export function getDefaultBannerValues(): ContentBanner {
  return {
    id: '',
    title: '',
    imageUrl: '',
    mobileImageUrl: '',
    linkUrl: '',
    description: '',
    position: 1,
    startDate: '',
    endDate: '',
    sortOrder: 0,
    isActive: true,
  }
}

export function toBannerPayload(values: ContentBanner): ContentBannerPayload {
  return {
    title: values.title.trim(),
    imageUrl: values.imageUrl.trim(),
    mobileImageUrl: values.mobileImageUrl.trim() || undefined,
    linkUrl: values.linkUrl.trim() || undefined,
    description: values.description.trim() || undefined,
    position: values.position,
    startDate: values.startDate.trim() || undefined,
    endDate: values.endDate.trim() || undefined,
    sortOrder: values.sortOrder,
    isActive: values.isActive,
  }
}

export type BannerFormErrors = Partial<Record<keyof ContentBanner, string>>

export function validateBannerForm(values: ContentBanner): BannerFormErrors {
  const errors: BannerFormErrors = {}
  if (!values.title.trim()) errors.title = 'Vui lòng nhập tiêu đề banner'
  if (!values.imageUrl.trim()) errors.imageUrl = 'Vui lòng nhập URL ảnh desktop'
  if (!values.mobileImageUrl.trim()) errors.mobileImageUrl = 'Vui lòng nhập URL ảnh mobile'
  if (!values.position) errors.position = 'Vui lòng chọn vị trí'
  if (Number.isNaN(values.sortOrder)) errors.sortOrder = 'Thứ tự không hợp lệ'
  return errors
}
