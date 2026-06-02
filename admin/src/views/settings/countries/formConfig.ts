import type { Country } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

const regionOptions = [
  { value: 'ASIA', label: 'Châu Á (ASIA)' },
  { value: 'EU', label: 'Châu Âu (EU)' },
  { value: 'NA', label: 'Bắc Mỹ (NA)' },
  { value: 'OC', label: 'Châu Đại Dương (OC)' },
  { value: 'AF', label: 'Châu Phi (AF)' },
  { value: 'SA', label: 'Nam Mỹ (SA)' },
]

export const countryFormConfig: SettingsFormConfig<Country> = {
  entityName: 'quốc gia',
  slugFromName: true,
  getDefaultValues: () => ({
    id: 0,
    name: '',
    englishName: '',
    slug: '',
    isoCode: '',
    flagEmoji: '',
    regionCode: 'ASIA',
    bannerUrl: '',
    seoTitle: '',
    seoDescription: '',
    sortOrder: 1,
    isActive: true,
  }),
  fields: [
    { name: 'name', label: 'Tên (Tiếng Việt)', type: 'text', required: true },
    { name: 'englishName', label: 'Tên tiếng Anh', type: 'text', required: true },
    { name: 'slug', label: 'Slug', type: 'text', required: true },
    { name: 'isoCode', label: 'Mã ISO', type: 'text', required: true, placeholder: 'JP', col: 6 },
    { name: 'flagEmoji', label: 'Cờ (emoji)', type: 'text', placeholder: '🇯🇵', col: 6 },
    { name: 'regionCode', label: 'Khu vực', type: 'select', required: true, options: regionOptions },
    { name: 'bannerUrl', label: 'URL banner', type: 'url' },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'seoTitle', label: 'Tiêu đề SEO', type: 'text' },
    { name: 'seoDescription', label: 'Mô tả SEO', type: 'textarea' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
