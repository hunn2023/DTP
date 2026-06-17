import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Badge, Button } from 'react-bootstrap'
import { TbEye, TbLock, TbLockOpen } from 'react-icons/tb'

import type { CustomerRow } from '@/apis/customersApi'
import { formatDisplayNumber } from '@/components/form/numberFieldUtils'
import CustomerAvatar from '@/features/customers/components/CustomerAvatar'

export type CustomerTableHandlers = {
  onView: (row: CustomerRow) => void
  onToggleLock: (row: CustomerRow) => void
}

const helper = createColumnHelper<CustomerRow>()

function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('vi-VN', { day: '2-digit', month: '2-digit', year: 'numeric', hour: '2-digit', minute: '2-digit' })
}

export function buildCustomerColumns(handlers: CustomerTableHandlers) {
  return [
    helper.display({
      id: 'customer',
      header: 'Khách hàng',
      cell: ({ row }) => (
        <div className="d-flex align-items-center gap-2">
          <CustomerAvatar
            fullName={row.original.fullName}
            avatarUrl={row.original.avatarUrl}
            size="sm"
          />
          <div className="min-w-0">
            <div className="fw-semibold text-truncate">{row.original.fullName || '—'}</div>
            <div className="text-muted fs-xxs text-truncate">{row.original.email}</div>
          </div>
        </div>
      ),
    }),
    helper.accessor('phone', {
      header: 'SĐT',
      cell: ({ getValue }) => <span className="text-nowrap">{getValue() || '—'}</span>,
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ row }) => (
        <Badge
          bg={row.original.isActive ? 'success' : 'secondary'}
          className="rounded-pill px-2 py-1 fw-normal fs-xxs">
          {row.original.isActive ? 'Hoạt động' : 'Đã khóa'}
        </Badge>
      ),
    }),
    helper.accessor('emailConfirmed', {
      header: 'Email',
      cell: ({ getValue }) => (
        <Badge
          bg={getValue() ? 'info' : 'warning'}
          className="rounded-pill px-2 py-1 fw-normal fs-xxs">
          {getValue() ? 'Đã xác nhận' : 'Chưa xác nhận'}
        </Badge>
      ),
    }),
    helper.accessor('totalOrders', {
      header: 'Đơn hàng',
      cell: ({ getValue }) => (
        <span className="fw-medium">{formatDisplayNumber(getValue())}</span>
      ),
    }),
    helper.accessor('totalSpent', {
      header: 'Tổng chi tiêu',
      cell: ({ getValue }) => (
        <span className="fw-medium text-nowrap">{formatDisplayNumber(getValue())} đ</span>
      ),
    }),
    helper.accessor('lastLoginAt', {
      header: 'Đăng nhập gần nhất',
      cell: ({ getValue }) => (
        <span className="text-muted fs-sm text-nowrap">{formatDateTime(getValue())}</span>
      ),
    }),
    {
      id: 'actions',
      header: 'Thao tác',
      enableSorting: false,
      cell: ({ row }: { row: { original: CustomerRow } }) => (
        <div className="d-flex gap-1" onClick={(e) => e.stopPropagation()}>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title="Xem chi tiết"
            onClick={() => handlers.onView(row.original)}>
            <TbEye className="fs-lg" />
          </Button>
          <Button
            variant="light"
            size="sm"
            className="btn-icon rounded-circle"
            title={row.original.isActive ? 'Khóa tài khoản' : 'Mở khóa'}
            onClick={() => handlers.onToggleLock(row.original)}>
            {row.original.isActive ? <TbLock className="fs-lg" /> : <TbLockOpen className="fs-lg" />}
          </Button>
        </div>
      ),
    },
  ] as ColumnDef<CustomerRow>[]
}
