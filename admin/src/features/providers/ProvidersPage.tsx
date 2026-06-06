import ProvidersCrudTable from '@/features/providers/ProvidersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const ProvidersPage = () => (
  <EntityPageLayout
    title="Danh sách nhà cung cấp"
    subtitle="Nguồn hàng & Provider"
    description="Provider là nguồn hàng/API (Airalo, ...). Dùng cho PhoneCards và mapping sản phẩm.">
    <ProvidersCrudTable />
  </EntityPageLayout>
)

export default ProvidersPage
