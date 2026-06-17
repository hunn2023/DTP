import CustomersCrudTable from '@/features/customers/components/CustomersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const CustomersBlockedPage = () => (
  <EntityPageLayout
    title="Khách hàng bị khóa"
    subtitle="Khách hàng"
    description="Danh sách khách hàng đã bị khóa tài khoản.">
    <CustomersCrudTable isActiveFilter={false} searchPlaceholder="Tìm khách bị khóa..." />
  </EntityPageLayout>
)

export default CustomersBlockedPage
