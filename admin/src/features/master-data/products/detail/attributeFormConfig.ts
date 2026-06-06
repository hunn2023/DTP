import type { ProductAttributeRow } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultAttributeValues(productId: string): ProductAttributeRow {
  return {
    id: '',
    productId,
    name: '',
    value: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function buildAttributeFormConfig(productId: string): EntityFormConfig<ProductAttributeRow> {
  return {
    entityName: 'thuộc tính',
    getDefaultValues: () => getDefaultAttributeValues(productId),
    fields: [
      { name: 'name', label: 'Tên thuộc tính', type: 'text', required: true, col: 6 },
      { name: 'value', label: 'Giá trị', type: 'text', required: true, col: 6 },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
    ],
  }
}
