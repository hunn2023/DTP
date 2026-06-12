import EsimPackagesCrudTable from '@/features/products/esim-packages/EsimPackagesCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const EsimPackagesPage = () => (
  <EntityPageLayout
    title="Gói eSIM"
    subtitle="Sản phẩm bán"
    description="Quản lý gói eSIM du lịch, liên kết biến thể sản phẩm, quốc gia, nhà mạng và giá bán.">
    <EsimPackagesCrudTable />
  </EntityPageLayout>
)

export default EsimPackagesPage
