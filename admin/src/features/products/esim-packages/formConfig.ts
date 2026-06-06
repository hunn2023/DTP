import type { EsimPackageLookups } from '@/features/products/esim-packages/lookups.api'
import type { EsimPackage } from '@/features/products/esim-packages/types'
import type { EntityFormConfig, FormFieldOption, FormModalMode } from '@/modules/crud/form/types'

const dataUnitOptions: FormFieldOption[] = [
  { value: 'GB', label: 'GB' },
  { value: 'MB', label: 'MB' },
]

const currencyOptions: FormFieldOption[] = [
  { value: 'USD', label: 'USD' },
  { value: 'VND', label: 'VND' },
]

export function getDefaultEsimPackageValues(): EsimPackage {
  return {
    id: '',
    isActive: true,
    productVariantId: '',
    productVariantName: '',
    productName: '',
    countryId: '',
    countryName: '',
    carrierId: '',
    carrierName: '',
    name: '',
    slug: '',
    dataAmount: 1,
    dataUnit: 'GB',
    validityDays: 7,
    price: 0,
    currency: 'USD',
    isUnlimited: false,
    sortOrder: 1,
  }
}

export function buildEsimPackageFormConfig(
  lookups: EsimPackageLookups,
  mode: FormModalMode | null,
): EntityFormConfig<EsimPackage> {
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
            hint: 'Chọn Product Variant gắn với gói eSIM',
          },
        ]
      : []

  return {
    entityName: 'gói eSIM',
    slugFromName: true,
    getDefaultValues: getDefaultEsimPackageValues,
    viewFields: [
      { name: 'productName', label: 'Sản phẩm', type: 'text', col: 6 },
      { name: 'productVariantName', label: 'Biến thể', type: 'text', col: 6 },
      { name: 'countryName', label: 'Quốc gia', type: 'text', col: 6 },
      { name: 'carrierName', label: 'Nhà mạng', type: 'text', col: 6 },
    ],
    fields: [
      ...createOnlyFields,
      { name: 'name', label: 'Tên gói', type: 'text', required: true, col: 12 },
      { name: 'slug', label: 'Slug', type: 'text', required: true, col: 12 },
      {
        name: 'countryId',
        label: 'Quốc gia',
        type: 'select',
        required: true,
        col: 6,
        options: lookups.countryOptions,
      },
      {
        name: 'carrierId',
        label: 'Nhà mạng',
        type: 'select',
        required: true,
        col: 6,
        options: lookups.carrierOptions,
      },
      { name: 'dataAmount', label: 'Dung lượng', type: 'number', required: true, col: 6 },
      { name: 'dataUnit', label: 'Đơn vị', type: 'select', required: true, col: 6, options: dataUnitOptions },
      { name: 'validityDays', label: 'Số ngày', type: 'number', required: true, col: 6 },
      { name: 'price', label: 'Giá', type: 'number', required: true, col: 6 },
      { name: 'currency', label: 'Tiền tệ', type: 'select', required: true, col: 6, options: currencyOptions },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isUnlimited', label: 'Không giới hạn data', type: 'checkbox', col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 6 },
    ],
  }
}
