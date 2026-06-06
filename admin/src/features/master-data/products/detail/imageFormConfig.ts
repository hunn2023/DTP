import type { ProductImageRow } from '@/features/master-data/products/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultImageValues(productId: string): ProductImageRow {
  return {
    id: '',
    productId,
    imageUrl: '',
    altText: '',
    sortOrder: 1,
    isThumbnail: false,
    isActive: true,
  }
}

export function buildImageFormConfig(productId: string): EntityFormConfig<ProductImageRow> {
  return {
    entityName: 'hình ảnh',
    getDefaultValues: () => getDefaultImageValues(productId),
    fields: [
      { name: 'imageUrl', label: 'URL ảnh', type: 'url', required: true },
      { name: 'altText', label: 'Alt text', type: 'text' },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isThumbnail', label: 'Là thumbnail', type: 'checkbox', col: 6 },
    ],
  }
}
