import PagedListTable from '@/features/sales/shared/PagedListTable'
import ConfirmModal from '@/features/sales/shared/ConfirmModal'
import PromptModal from '@/features/sales/shared/PromptModal'
import OrderDetailModal from '@/features/sales/orders/components/OrderDetailModal'
import OrderFiltersBar from '@/features/sales/orders/components/OrderFiltersBar'
import type { OrderPageConfig } from '@/features/sales/orders/orderFilters'
import { useOrdersCrud } from '@/features/sales/orders/useOrdersCrud'

type OrdersTableProps = {
  config: OrderPageConfig
}

const OrdersTable = ({ config }: OrdersTableProps) => {
  const crud = useOrdersCrud(config)

  return (
    <>
      <PagedListTable
        searchPlaceholder={config.searchPlaceholder}
        loadingLabel="Đang tải đơn hàng..."
        onRowClick={(row) => void crud.openDetail(row)}
        toolbar={<OrderFiltersBar value={crud.filterForm} onChange={crud.setFilterForm} />}
        {...crud.list}
      />

      <OrderDetailModal
        order={crud.detail}
        loading={crud.detailLoading}
        isSaving={crud.isSaving}
        handlers={crud.handlers}
        onClose={crud.closeDetail}
      />

      <PromptModal
        show={crud.promptAction === 'markPaid'}
        title={`Đánh dấu đã thanh toán — ${crud.actionRow?.orderCode ?? ''}`}
        label="Mã giao dịch thanh toán"
        placeholder="Nhập PaymentTransactionId..."
        required
        confirmLabel="Xác nhận thanh toán"
        isSaving={crud.isSaving}
        onHide={crud.closePrompt}
        onConfirm={(value) => void crud.submitPrompt(value)}
      />

      <PromptModal
        show={crud.promptAction === 'cancel'}
        title={`Hủy đơn — ${crud.actionRow?.orderCode ?? ''}`}
        label="Lý do hủy"
        placeholder="Nhập lý do hủy đơn..."
        required
        confirmLabel="Xác nhận hủy"
        isSaving={crud.isSaving}
        onHide={crud.closePrompt}
        onConfirm={(value) => void crud.submitPrompt(value)}
      />

      <ConfirmModal
        show={crud.confirmCompleteRow !== null}
        title={`Hoàn thành đơn — ${crud.confirmCompleteRow?.orderCode ?? ''}`}
        message="Xác nhận đánh dấu đơn hàng là hoàn thành?"
        confirmLabel="Hoàn thành"
        isSaving={crud.isSaving}
        onHide={() => crud.setConfirmCompleteRow(null)}
        onConfirm={() => void crud.confirmComplete()}
      />
    </>
  )
}

export default OrdersTable
