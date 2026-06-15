import { Button, Card, CardHeader, Form, InputGroup } from 'react-bootstrap'

import PaymentDetailModal from '@/features/sales/payments/components/PaymentDetailModal'
import OrderFiltersBar from '@/features/sales/orders/components/OrderFiltersBar'
import { usePaymentsCrud } from '@/features/sales/payments/usePaymentsCrud'
import PagedListTable from '@/features/sales/shared/PagedListTable'

const PaymentsTable = () => {
  const crud = usePaymentsCrud()

  return (
    <>
      <Card className="mb-3">
        <CardHeader className="border-light py-3">
          <div className="d-flex flex-wrap align-items-center gap-2">
            <span className="text-muted fs-sm">Tra cứu theo mã giao dịch:</span>
            <InputGroup size="sm" style={{ maxWidth: '420px' }}>
              <Form.Control
                placeholder="Payment ID (GUID)..."
                value={crud.paymentIdLookup}
                onChange={(e) => crud.setPaymentIdLookup(e.target.value)}
              />
              <Button variant="primary" onClick={() => void crud.loadPaymentById()}>
                Tra cứu
              </Button>
            </InputGroup>
          </div>
        </CardHeader>
      </Card>

      <PagedListTable
        searchPlaceholder="Tìm mã đơn, khách..."
        loadingLabel="Đang tải giao dịch..."
        onRowClick={(row) => void crud.openDetailFromOrder(row)}
        toolbar={<OrderFiltersBar value={crud.filterForm} onChange={crud.setFilterForm} />}
        {...crud.list}
      />

      <PaymentDetailModal
        open={crud.detailOpen}
        payment={crud.paymentDetail}
        loading={crud.detailLoading}
        onClose={crud.closeDetail}
      />
    </>
  )
}

export default PaymentsTable
