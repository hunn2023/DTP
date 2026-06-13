import PagedListTable from '@/features/sales/shared/PagedListTable'
import PromptModal from '@/features/sales/shared/PromptModal'
import DeliveryDetailModal from '@/features/sales/deliveries/components/DeliveryDetailModal'
import { useDeliveriesCrud } from '@/features/sales/deliveries/useDeliveriesCrud'
import type { DeliveriesQueryFilters } from '@/apis/deliveriesApi'

type DeliveriesTableProps = {
  filters?: DeliveriesQueryFilters
  searchPlaceholder?: string
}

const DeliveriesTable = ({
  filters = {},
  searchPlaceholder = 'Tìm mã đơn, khách...',
}: DeliveriesTableProps) => {
  const crud = useDeliveriesCrud({ filters })

  return (
    <>
      <PagedListTable
        searchPlaceholder={searchPlaceholder}
        loadingLabel="Đang tải giao hàng..."
        onRowClick={(row) => void crud.openDetail(row)}
        {...crud.list}
      />

      <DeliveryDetailModal
        delivery={crud.detail}
        loading={crud.detailLoading}
        onClose={crud.closeDetail}
      />

      <PromptModal
        show={crud.promptAction === 'delivered'}
        title={`Đánh dấu đã giao — ${crud.actionRow?.orderCode ?? ''}`}
        label="Ghi chú (tuỳ chọn)"
        placeholder="Giao thành công..."
        required={false}
        confirmLabel="Xác nhận đã giao"
        isSaving={crud.isSaving}
        onHide={crud.closePrompt}
        onConfirm={(value) => void crud.submitPrompt(value)}
      />

      <PromptModal
        show={crud.promptAction === 'failed'}
        title={`Đánh dấu lỗi — ${crud.actionRow?.orderCode ?? ''}`}
        label="Mô tả lỗi"
        placeholder="Nhập lý do / thông báo lỗi..."
        required
        confirmLabel="Xác nhận lỗi"
        isSaving={crud.isSaving}
        onHide={crud.closePrompt}
        onConfirm={(value) => void crud.submitPrompt(value)}
      />
    </>
  )
}

export default DeliveriesTable
