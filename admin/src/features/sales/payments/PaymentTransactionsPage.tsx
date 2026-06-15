import PaymentsTable from '@/features/sales/payments/components/PaymentsTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PaymentTransactionsPage = () => (
  <EntityPageLayout
    title="Giao dịch thanh toán"
    subtitle="Bán hàng"
    description="Danh sách đơn hàng và chi tiết giao dịch từ API thanh toán.">
    <PaymentsTable />
  </EntityPageLayout>
)

export default PaymentTransactionsPage
