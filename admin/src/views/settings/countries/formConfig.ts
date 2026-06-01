import type { Country } from '@/views/settings/types'
import type { SettingsFormConfig } from '@/views/settings/form/types'

const regionOptions = [
  { value: 'ASIA', label: 'Asia (ASIA)' },
  { value: 'EU', label: 'Europe (EU)' },
  { value: 'NA', label: 'North America (NA)' },
  { value: 'OC', label: 'Oceania (OC)' },
  { value: 'AF', label: 'Africa (AF)' },
  { value: 'SA', label: 'South America (SA)' },
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
    { name: 'bannerUrl', label: 'Banner URL', type: 'url' },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    { name: 'seoTitle', label: 'SEO Title', type: 'text' },
    { name: 'seoDescription', label: 'SEO Description', type: 'textarea' },
    { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
  ],
}
