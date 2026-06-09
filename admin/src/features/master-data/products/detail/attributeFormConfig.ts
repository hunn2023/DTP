import type { ProductAttributeRow } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultAttributeValues(productId: string): ProductAttributeRow {
  return {
    id: '',
    productId,
    key: '',
    displayName: '',
    value: '',
    sortOrder: 1,
    isVisible: true,
    isActive: true,
  }
}

export function buildAttributeFormConfig(productId: string): EntityFormConfig<ProductAttributeRow> {
  return {
    entityName: 'thuộc tính',
    getDefaultValues: () => getDefaultAttributeValues(productId),
    fields: [
      { name: 'key', label: 'Key', type: 'text', required: true, col: 6 },
      { name: 'displayName', label: 'Tên hiển thị', type: 'text', col: 6 },
      { name: 'value', label: 'Giá trị', type: 'text', required: true, col: 6 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 12 },
    ],
  }
}
