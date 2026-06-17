import { createColumnHelper, type ColumnDef } from '@tanstack/react-table'
import { Button } from 'react-bootstrap'
import { TbEye, TbLock, TbLockOpen } from 'react-icons/tb'

import type { CustomerRow } from '@/apis/customersApi'
import { formatDisplayNumber } from '@/components/form/numberFieldUtils'

export type CustomerTableHandlers = {
  onView: (row: CustomerRow) => void
  onToggleLock: (row: CustomerRow) => void
}

const helper = createColumnHelper<CustomerRow>()

function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  if (Number.isNaN(date.getTime())) return value
  return date.toLocaleString('vi-VN')
}

export function buildCustomerColumns(handlers: CustomerTableHandlers) {
  return [
    helper.accessor('fullName', {
      header: 'Họ tên',
      cell: ({ row }) => (
        <div>
          <div className="fw-semibold">{row.original.fullName || '—'}</div>
          <div className="text-muted fs-xxs">{row.original.email}</div>
        </div>
      ),
    }),
    helper.accessor('phone', {
      header: 'SĐT',
      cell: ({ getValue }) => getValue() || '—',
    }),
    helper.accessor('status', {
      header: 'Trạng thái',
      cell: ({ row }) => (
        <span
          className={`badge ${row.original.isActive ? 'badge-soft-primary' : 'badge-soft-secondary'} fs-xxs`}>
          {row.original.isActive ? 'Hoạt động' : 'Đã khóa'}
        </span>
      ),
    }),
    helper.accessor('emailConfirmed', {
      header: 'Xác nhận email',
      cell: ({ getValue }) => (getValue() ? 'Đã xác nhận' : 'Chưa'),
    }),
    helper.accessor('totalOrders', {
      header: 'Đơn hàng',
      cell: ({ getValue }) => formatDisplayNumber(getValue()),
    }),
    helper.accessor('totalSpent', {
      header: 'Tổng chi tiêu',
      cell: ({ getValue }) => `${formatDisplayNumber(getValue())} đ`,
    }),
    helper.accessor('lastLoginAt', {
      header: 'Đăng nhập gần nhất',
      cell: ({ getValue }) => formatDateTime(getValue()),
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
