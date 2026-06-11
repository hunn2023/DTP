import EsimPackagesCrudTable from '@/features/products/esim-packages/EsimPackagesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const EsimPackagesPage = () => (
  <EntityPageLayout
    title="Gói eSIM"
    subtitle="Sản phẩm bán"
    description="Quản lý gói eSIM du lịch. Liên kết Product Variant, Country, Carrier và giá bán.">
    <EsimPackagesCrudTable />
  </EntityPageLayout>
)

export default EsimPackagesPage
