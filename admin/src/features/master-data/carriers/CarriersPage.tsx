import CarriersCrudTable from '@/features/master-data/carriers/CarriersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const CarriersPage = () => (
  <EntityPageLayout
    title="Nhà mạng"
    subtitle="Cấu hình hệ thống"
    description="Danh sách nhà mạng theo quốc gia. Dùng cho gói eSIM và sản phẩm liên quan.">
    <CarriersCrudTable />
  </EntityPageLayout>
)

export default CarriersPage
