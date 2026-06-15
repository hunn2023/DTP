import { Badge } from 'react-bootstrap'

import {
  enumLabel,
  ORDER_PAYMENT_STATUS_LABELS,
  ORDER_STATUS_LABELS,
} from '@/features/sales/shared/format'

const ORDER_STATUS_VARIANT: Record<number, string> = {
  1: 'secondary',
  2: 'warning',
  3: 'info',
  4: 'primary',
  5: 'success',
  6: 'dark',
  7: 'danger',
}

const PAYMENT_STATUS_VARIANT: Record<number, string> = {
  1: 'info',
  2: 'warning',
  3: 'success',
  4: 'danger',
  5: 'secondary',
}

type StatusBadgeProps = {
  label: string
  variant: string
}

function StatusBadge({ label, variant }: StatusBadgeProps) {
  return (
    <Badge bg={variant} className="rounded-pill px-2 py-1 fw-normal fs-xs">
      {label}
    </Badge>
  )
}

export function OrderStatusBadge({ status }: { status: number }) {
  return (
    <StatusBadge
      label={enumLabel(status, ORDER_STATUS_LABELS)}
      variant={ORDER_STATUS_VARIANT[status] ?? 'secondary'}
    />
  )
}

export function OrderPaymentStatusBadge({ paymentStatus }: { paymentStatus: number }) {
  return (
    <StatusBadge
      label={enumLabel(paymentStatus, ORDER_PAYMENT_STATUS_LABELS)}
      variant={PAYMENT_STATUS_VARIANT[paymentStatus] ?? 'secondary'}
    />
  )
}

export function OrderDetailStatusBadges({
  status,
  paymentStatus,
}: {
  status: number
  paymentStatus: number
}) {
  return (
    <div className="d-flex flex-wrap gap-2">
      <OrderStatusBadge status={status} />
      <OrderPaymentStatusBadge paymentStatus={paymentStatus} />
    </div>
  )
}
