import PaymentDetailModal from '@/features/sales/payments/components/PaymentDetailModal'
import { usePaymentTransactionsPage } from '@/features/sales/payments/usePaymentTransactionsPage'
import PagedListTable from '@/features/sales/shared/PagedListTable'
import EntityPageLayout from '@/modules/crud/components/EntityPageLayout'

const PaymentTransactionsPage = () => {
  const { list, selectedOrderId, paymentDetail, isLoadingDetail, closeDetail } =
    usePaymentTransactionsPage()

  return (
    <EntityPageLayout
      title="Giao dịch thanh toán"
      subtitle="Bán hàng"
      description="Danh sách đơn và chi tiết giao dịch từ API thanh toán.">
      <PagedListTable
        searchPlaceholder="Tìm mã đơn, khách..."
        loadingLabel="Đang tải giao dịch..."
        {...list}
      />
      <PaymentDetailModal
        payment={selectedOrderId ? paymentDetail : null}
        loading={isLoadingDetail}
        onClose={closeDetail}
      />
    </EntityPageLayout>
  )
}

export default PaymentTransactionsPage
