import type { ProductVariant } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultVariantValues(productId: string): ProductVariant {
  return {
    id: '',
    productId,
    sku: '',
    name: '',
    price: 0,
    originalPrice: null,
    durationDays: null,
    dataAmount: null,
    dataUnit: 'GB',
    isUnlimited: false,
    sortOrder: 1,
    isActive: true,
  }
}

export function buildVariantFormConfig(productId: string): EntityFormConfig<ProductVariant> {
  return {
    entityName: 'biến thể',
    getDefaultValues: () => getDefaultVariantValues(productId),
    fields: [
      { name: 'name', label: 'Tên biến thể', type: 'text', required: true },
      { name: 'sku', label: 'SKU', type: 'text', col: 6 },
      { name: 'price', label: 'Giá', type: 'number', required: true, col: 6 },
      { name: 'originalPrice', label: 'Giá gốc', type: 'number', col: 6 },
      { name: 'durationDays', label: 'Số ngày', type: 'number', col: 6 },
      { name: 'dataAmount', label: 'Dung lượng', type: 'number', col: 6 },
      { name: 'dataUnit', label: 'Đơn vị data', type: 'text', col: 6 },
      { name: 'isUnlimited', label: 'Không giới hạn', type: 'checkbox', col: 6 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Kích hoạt', type: 'checkbox', col: 6 },
    ],
  }
}
