import { Col, Row } from 'react-bootstrap'
import { LuBox, LuCreditCard } from 'react-icons/lu'

import {
  resolveCustomerSelectOption,
  searchCustomerSelectOptions,
} from '@/features/customers/customerSearchSelect'
import type { OrderFilterForm } from '@/features/sales/orders/orderFilterTypes'
import { ORDER_PAYMENT_STATUS_LABELS, ORDER_STATUS_LABELS } from '@/features/sales/shared/format'
import ApiFilterSearchSelect from '@/modules/crud/components/ApiFilterSearchSelect'

type OrderFiltersBarProps = {
  value: OrderFilterForm
  onChange: (next: OrderFilterForm) => void
}

type FilterOption = { value: string; label: string }

function buildEnumOptions(labels: Record<number, string>, allLabel: string): FilterOption[] {
  return [
    { value: '', label: allLabel },
    ...Object.entries(labels).map(([key, label]) => ({ value: key, label })),
  ]
}

const STATUS_OPTIONS = buildEnumOptions(ORDER_STATUS_LABELS, 'Tất cả trạng thái đơn')
const PAYMENT_STATUS_OPTIONS = buildEnumOptions(ORDER_PAYMENT_STATUS_LABELS, 'Tất cả thanh toán')

function FilterSelect({
  label,
  icon: Icon,
  value,
  options,
  onChange,
}: {
  label: string
  icon: typeof LuBox
  value: string
  options: FilterOption[]
  onChange: (value: string) => void
}) {
  return (
    <Col xs={12} md={4}>
      <label className="form-label text-muted fs-xxs text-uppercase mb-1">{label}</label>
      <div className="app-search">
        <select
          className="form-select form-control"
          value={value}
          onChange={(event) => onChange(event.target.value)}>
          {options.map((option) => (
            <option key={option.value || 'all'} value={option.value}>
              {option.label}
            </option>
          ))}
        </select>
        <Icon className="app-search-icon text-muted" />
      </div>
    </Col>
  )
}

const OrderFiltersBar = ({ value, onChange }: OrderFiltersBarProps) => {
  const patch = (partial: Partial<OrderFilterForm>) => onChange({ ...value, ...partial })

  return (
    <Row className="g-2 mb-3">
      <Col xs={12} md={6} lg={5}>
        <ApiFilterSearchSelect
          label="Khách hàng"
          value={value.customerId}
          onChange={(customerId) => patch({ customerId })}
          allLabel="Tất cả khách hàng"
          loadOptions={searchCustomerSelectOptions}
          resolveValue={resolveCustomerSelectOption}
          noOptionsMessage="Không tìm thấy khách hàng"
          className="customer-filter-select"
        />
      </Col>
      <FilterSelect
        label="Trạng thái đơn"
        icon={LuBox}
        value={value.status}
        options={STATUS_OPTIONS}
        onChange={(status) => patch({ status })}
      />
      <FilterSelect
        label="Thanh toán"
        icon={LuCreditCard}
        value={value.paymentStatus}
        options={PAYMENT_STATUS_OPTIONS}
        onChange={(paymentStatus) => patch({ paymentStatus })}
      />
    </Row>
  )
}

export default OrderFiltersBar
