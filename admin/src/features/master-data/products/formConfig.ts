import type { CatalogProduct } from '@/features/master-data/products/types'
import type { EntityFormConfig, FormFieldOption } from '@/modules/crud/form/types'

export function getDefaultProductValues(): CatalogProduct {
  return {
    id: '',
    code: '',
    name: '',
    slug: '',
    categoryId: '',
    categoryName: '',
    shortDescription: '',
    description: '',
    thumbnailUrl: '',
    sortOrder: 1,
    isActive: true,
  }
}

export function buildProductFormConfig(
  categoryOptions: FormFieldOption[],
): EntityFormConfig<CatalogProduct> {
  return {
    entityName: 'sản phẩm',
    slugFromName: true,
    getDefaultValues: getDefaultProductValues,
    viewFields: [
      { name: 'code', label: 'Mã (Code)', type: 'text', col: 6 },
      { name: 'categoryName', label: 'Danh mục', type: 'text', col: 6 },
      { name: 'shortDescription', label: 'Mô tả ngắn', type: 'textarea' },
      { name: 'description', label: 'Mô tả chi tiết', type: 'textarea' },
      { name: 'thumbnailUrl', label: 'URL thumbnail', type: 'url' },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 6 },
    ],
    fields: [
      { name: 'name', label: 'Tên sản phẩm', type: 'text', required: true },
      { name: 'slug', label: 'Slug', type: 'text', required: true },
      { name: 'code', label: 'Mã (Code)', type: 'text', col: 6 },
      {
        name: 'categoryId',
        label: 'Danh mục',
        type: 'select',
        required: true,
        col: 6,
        options: categoryOptions,
      },
      { name: 'shortDescription', label: 'Mô tả ngắn', type: 'textarea' },
      { name: 'description', label: 'Mô tả chi tiết', type: 'textarea' },
      { name: 'thumbnailUrl', label: 'URL thumbnail', type: 'url' },
      { name: 'sortOrder', label: 'Thứ tự', type: 'number', required: true, col: 6 },
      { name: 'isActive', label: 'Hiển thị', type: 'checkbox', col: 6 },
    ],
  }
}
