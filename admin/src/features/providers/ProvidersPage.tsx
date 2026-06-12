import ProvidersCrudTable from '@/features/providers/components/ProvidersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const ProvidersPage = () => (
  <EntityPageLayout
    title="Danh sách nhà cung cấp"
    subtitle="Nguồn hàng"
    description="Nhà cung cấp là nguồn hàng/API (Airalo, ...). Dùng cho thẻ viễn thông và mapping sản phẩm.">
    <ProvidersCrudTable />
  </EntityPageLayout>
)

export default ProvidersPage
