import type { PaymentProvider } from '@/features/master-data/payment-providers/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultPaymentProviderValues(): PaymentProvider {
  return {
    id: '',
    code: '',
    name: '',
    paymentMethod: '',
    isActive: true,
    isDefault: false,
    sortOrder: 1,
    minAmount: null,
    maxAmount: null,
    currency: 'VND',
    logoUrl: '',
    description: '',
    createdAt: '',
    updatedAt: '',
  }
}

export const paymentProviderFormConfig: EntityFormConfig<PaymentProvider> = {
  entityName: 'cổng thanh toán',
  getDefaultValues: getDefaultPaymentProviderValues,
  fields: [
    { name: 'sortOrder', label: 'Thứ tự hiển thị', type: 'number', required: true, col: 6 },
    { name: 'minAmount', label: 'Số tiền tối thiểu', type: 'number', col: 6, placeholder: 'Không giới hạn' },
    { name: 'maxAmount', label: 'Số tiền tối đa', type: 'number', col: 6, placeholder: 'Không giới hạn' },
    { name: 'isDefault', label: 'Đặt làm mặc định', type: 'checkbox', col: 6 },
  ],
  viewFields: [
    { name: 'name', label: 'Tên', type: 'text' },
    { name: 'code', label: 'Mã', type: 'text', col: 6 },
    { name: 'paymentMethod', label: 'Phương thức', type: 'text', col: 6 },
    { name: 'currency', label: 'Tiền tệ', type: 'text', col: 6 },
    { name: 'sortOrder', label: 'Thứ tự', type: 'number', col: 6 },
    { name: 'minAmount', label: 'Tối thiểu', type: 'number', col: 6 },
    { name: 'maxAmount', label: 'Tối đa', type: 'number', col: 6 },
    { name: 'isDefault', label: 'Mặc định', type: 'checkbox', col: 6 },
    { name: 'isActive', label: 'Đang bật', type: 'checkbox', col: 6 },
    { name: 'logoUrl', label: 'Logo URL', type: 'url' },
    { name: 'description', label: 'Mô tả', type: 'textarea' },
  ],
}
