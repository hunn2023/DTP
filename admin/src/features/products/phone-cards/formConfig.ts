import type { PhoneCardLookups } from '@/features/products/phone-cards/lookups.api'
import type { PhoneCard } from '@/features/products/phone-cards/types'
import type { EntityFormConfig, FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

const currencyOptions: FormFieldOption[] = [
  { value: 'VND', label: 'VND' },
  { value: 'USD', label: 'USD' },
]

export function getDefaultPhoneCardValues(): PhoneCard {
  return {
    id: '',
    isActive: true,
    productVariantId: '',
    productVariantName: '',
    providerId: '',
    providerName: '',
    name: '',
    slug: '',
    faceValue: 0,
    price: 0,
    currency: 'VND',
    sortOrder: 1,
  }
}

export function buildPhoneCardFormConfig(
  lookups: PhoneCardLookups,
  mode: FormModalMode | null,
): EntityFormConfig<PhoneCard> {
  const createOnlyFields =
    mode === 'create'
      ? [
          {
            name: 'productVariantId' as const,
            label: 'Biến thể sản phẩm',
            type: 'select' as const,
            required: true,
            col: 12 as const,
            options: lookups.productVariantOptions,
            hint: 'Chọn Product Variant gắn với thẻ',
          },
        ]
      : []

  return {
    entityName: 'thẻ viễn thông',
    slugFromName: true,
    getDefaultValues: getDefaultPhoneCardValues,
    viewFields: [
      { name: 'productVariantName', label: 'Biến thể SP', type: 'text', col: 6 },
      { name: 'providerName', label: 'Nhà cung cấp', type: 'text', col: 6 },
      { name: 'slug', label: 'Slug', type: 'text', col: 12 },
    ],
    fields: [
      ...createOnlyFields,
      { name: 'name', label: 'Tên thẻ', type: 'text', required: true, col: 12 },
      { name: 'slug', label: 'Slug', type: 'text', required: true, col: 12 },
      {
        name: 'providerId',
        label: 'Nhà cung cấp',
        type: 'select',
        required: true,
        col: 12,
        options: lookups.providerOptions,
      },
      { name: 'faceValue', label: 'Mệnh giá (FaceValue)', type: 'number', required: true, col: 6 },
      { name: 'price', label: 'Giá bán', type: 'number', required: true, col: 6 },
      { name: 'currency', label: 'Tiền tệ', type: 'select', required: true, col: 6, options: currencyOptions },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
    ],
  }
}
