import { BANNER_POSITION_OPTIONS } from '@/features/content/bannerPosition'
import { getDefaultBannerValues } from '@/features/content/banners/bannerFormUtils'
import type { ContentBanner } from '@/features/content/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export const bannerFormConfig: EntityFormConfig<ContentBanner> = {
  entityName: 'banner',
  getDefaultValues: getDefaultBannerValues,  fields: [
    { name: 'title', label: 'Tiêu đề', type: 'text', required: true },
    { name: 'imageUrl', label: 'Ảnh desktop', type: 'url', required: true },
    { name: 'mobileImageUrl', label: 'Ảnh mobile', type: 'url', col: 6 },
    { name: 'linkUrl', label: 'Link', type: 'url', col: 6 },
    { name: 'description', label: 'Mô tả', type: 'textarea' },
    {
      name: 'position',
      label: 'Vị trí',
      type: 'select',
      required: true,
      parseAsNumber: true,
      options: BANNER_POSITION_OPTIONS,
      col: 6,
    },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'startDate', label: 'Từ ngày', type: 'date', col: 6 },
    { name: 'endDate', label: 'Đến ngày', type: 'date', col: 6 },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
