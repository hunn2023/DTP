import type { ProductPriceRow } from '@/features/master-data/products/types'
import type { EntityFormConfig, FormFieldOption } from '@/modules/crud/form/types'

export function getDefaultPriceValues(): ProductPriceRow {
  return {
    id: '',
    productId: '',
    productName: '',
    productVariantId: '',
    productVariantName: '',
    currency: 'VND',
    originalPrice: 0,
    salePrice: 0,
    costPrice: 0,
    startDate: '',
    endDate: '',
    note: '',
    isActive: true,
  }
}

export function buildPriceFormConfig(
  productOptions: FormFieldOption[],
  variantOptions: FormFieldOption[],
  isEdit: boolean,
): EntityFormConfig<ProductPriceRow> {
  const createFields = [
    {
      name: 'productId' as const,
      label: 'Sản phẩm',
      type: 'select' as const,
      required: true,
      options: productOptions,
    },
    {
      name: 'productVariantId' as const,
      label: 'Biến thể (tuỳ chọn)',
      type: 'select' as const,
      options: [{ value: '', label: '— Toàn sản phẩm —' }, ...variantOptions],
    },
    { name: 'currency', label: 'Tiền tệ', type: 'text' as const, required: true, col: 6 as const },
    { name: 'originalPrice', label: 'Giá gốc', type: 'number' as const, required: true, col: 6 as const },
    { name: 'salePrice', label: 'Giá bán', type: 'number' as const, required: true, col: 6 as const },
    { name: 'costPrice', label: 'Giá vốn', type: 'number' as const, required: true, col: 6 as const },
    { name: 'startDate', label: 'Từ ngày', type: 'date' as const, col: 6 as const },
    { name: 'endDate', label: 'Đến ngày', type: 'date' as const, col: 6 as const },
    { name: 'note', label: 'Ghi chú', type: 'textarea' as const, col: 12 as const },
  ]

  const editFields = [
    { name: 'currency', label: 'Tiền tệ', type: 'text' as const, required: true, col: 6 as const },
    { name: 'originalPrice', label: 'Giá gốc', type: 'number' as const, required: true, col: 6 as const },
    { name: 'salePrice', label: 'Giá bán', type: 'number' as const, required: true, col: 6 as const },
    { name: 'costPrice', label: 'Giá vốn', type: 'number' as const, required: true, col: 6 as const },
    { name: 'startDate', label: 'Từ ngày', type: 'date' as const, col: 6 as const },
    { name: 'endDate', label: 'Đến ngày', type: 'date' as const, col: 6 as const },
    { name: 'note', label: 'Ghi chú', type: 'textarea' as const, col: 12 as const },
    { name: 'isActive', label: 'Kích hoạt', type: 'checkbox' as const, col: 12 as const },
  ]

  return {
    entityName: 'bản ghi giá',
    getDefaultValues: getDefaultPriceValues,
    fields: (isEdit ? editFields : createFields) as EntityFormConfig<ProductPriceRow>['fields'],
    viewFields: [
      { name: 'productName', label: 'Sản phẩm', type: 'text' },
      { name: 'productVariantName', label: 'Biến thể', type: 'text' },
      ...(editFields as EntityFormConfig<ProductPriceRow>['fields']),
    ],
  }
}
