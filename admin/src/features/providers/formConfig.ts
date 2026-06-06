import type { CatalogProvider } from '@/features/providers/types'
import type { EntityFormConfig } from '@/modules/crud/form/types'

export function getDefaultProviderValues(): CatalogProvider {
  return {
    id: '',
    code: '',
    name: '',
    apiBaseUrl: '',
    apiKey: '',
    apiSecret: '',
    webhookUrl: '',
    supportEmail: '',
    isActive: true,
  }
}

export const providerFormConfig: EntityFormConfig<CatalogProvider> = {
  entityName: 'provider',
  getDefaultValues: getDefaultProviderValues,
  fields: [
    { name: 'name', label: 'Tên provider', type: 'text', required: true },
    { name: 'code', label: 'Mã (Code)', type: 'text', required: true, col: 6 },
    { name: 'supportEmail', label: 'Email hỗ trợ', type: 'text', col: 6 },
    { name: 'apiBaseUrl', label: 'API Base URL', type: 'url', col: 12 },
    { name: 'apiKey', label: 'API Key', type: 'password', col: 6 },
    { name: 'apiSecret', label: 'API Secret', type: 'password', col: 6 },
    { name: 'webhookUrl', label: 'Webhook URL', type: 'url', col: 12 },
    { name: 'isActive', label: 'Kích hoạt', type: 'checkbox', col: 12 },
  ],
}
