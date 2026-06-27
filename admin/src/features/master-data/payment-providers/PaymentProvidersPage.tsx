import PaymentProvidersCrudTable from '@/features/master-data/payment-providers/components/PaymentProvidersCrudTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PaymentProvidersPage = () => (
  <EntityPageLayout
    title="Thanh toán"
    subtitle="Cấu hình hệ thống"
    description="Cấu hình cổng thanh toán — bật/tắt, đặt mặc định, giới hạn số tiền và thứ tự hiển thị.">
    <PaymentProvidersCrudTable />
  </EntityPageLayout>
)

export default PaymentProvidersPage
