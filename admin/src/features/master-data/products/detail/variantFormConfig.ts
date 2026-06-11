import type { ProductVariant } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultVariantValues(productId: string): ProductVariant {
  return {
    id: '',
    productId,
    sku: '',
    name: '',
    shortName: '',
    description: '',
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
      { name: 'shortName', label: 'Tên ngắn', type: 'text', col: 6 },
      { name: 'description', label: 'Mô tả', type: 'textarea', col: 12 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Kích hoạt', type: 'checkbox', col: 6 },
    ],
  }
}
