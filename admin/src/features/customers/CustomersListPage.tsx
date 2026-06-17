import CustomersCrudTable from '@/features/customers/components/CustomersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const CustomersListPage = () => (
  <EntityPageLayout
    title="Danh sách khách hàng"
    subtitle="Khách hàng"
    description="Quản lý tài khoản khách hàng — xem chi tiết, khóa/mở khóa.">
    <CustomersCrudTable />
  </EntityPageLayout>
)

export default CustomersListPage
