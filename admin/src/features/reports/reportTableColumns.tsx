import { formatPaymentMethod } from '@/features/sales/shared/format'
import type { ReportTableColumn } from '@/features/reports/components/ReportDataTable'
import {
  formatOrderStatusLabel,
  formatReportCount,
  formatReportDateLabel,
  formatReportMoney,
} from '@/features/reports/reportFormat'
import type { ReportTimeSeriesPoint, ReportTopItem } from '@/features/reports/reportTypes'

export const topItemMoneyColumns: ReportTableColumn<ReportTopItem>[] = [
  {
    key: 'name',
    header: 'Tên',
    cell: (row) => (
      <div>
        <div className="fw-medium">{row.name || '—'}</div>
        {row.code && <div className="text-muted fs-xxs">{row.code}</div>}
      </div>
    ),
  },
  {
    key: 'value',
    header: 'Giá trị',
    cell: (row) => <span className="report-value-money text-nowrap">{formatReportMoney(row.value)}</span>,
    className: 'text-end',
  },
  {
    key: 'count',
    header: 'Số lượng',
    cell: (row) => formatReportCount(row.count),
    className: 'text-end',
  },
]

export const timeSeriesMoneyColumns: ReportTableColumn<ReportTimeSeriesPoint>[] = [
  {
    key: 'label',
    header: 'Thời gian',
    cell: (row) => formatReportDateLabel(row.label, row.date),
  },
  {
    key: 'value',
    header: 'Giá trị',
    cell: (row) => <span className="report-value-money text-nowrap">{formatReportMoney(row.value)}</span>,
    className: 'text-end',
  },
  {
    key: 'count',
    header: 'Số lượng',
    cell: (row) => formatReportCount(row.count),
    className: 'text-end',
  },
]

export const timeSeriesCountColumns: ReportTableColumn<ReportTimeSeriesPoint>[] = [
  {
    key: 'label',
    header: 'Thời gian',
    cell: (row) => formatReportDateLabel(row.label, row.date),
  },
  {
    key: 'count',
    header: 'Số lượng',
    cell: (row) => formatReportCount(row.count),
    className: 'text-end',
  },
]

export const orderStatusColumns: ReportTableColumn<ReportTopItem>[] = [
  {
    key: 'name',
    header: 'Trạng thái',
    cell: (row) => formatOrderStatusLabel(row.code, row.name),
  },
  {
    key: 'value',
    header: 'Tổng tiền',
    cell: (row) => <span className="report-value-money text-nowrap">{formatReportMoney(row.value)}</span>,
    className: 'text-end',
  },
  {
    key: 'count',
    header: 'Số đơn',
    cell: (row) => formatReportCount(row.count),
    className: 'text-end',
  },
]

export const paymentMethodColumns: ReportTableColumn<ReportTopItem>[] = [
  {
    key: 'name',
    header: 'Phương thức',
    cell: (row) => formatPaymentMethod(row.name || row.code),
  },
  {
    key: 'value',
    header: 'Giá trị',
    cell: (row) => <span className="report-value-money text-nowrap">{formatReportMoney(row.value)}</span>,
    className: 'text-end',
  },
  {
    key: 'count',
    header: 'Số giao dịch',
    cell: (row) => formatReportCount(row.count),
    className: 'text-end',
  },
]

export function topItemRowKey(row: ReportTopItem, index: number): string {
  return row.id ?? row.code ?? `${row.name}-${index}`
}

export function timeSeriesRowKey(row: ReportTimeSeriesPoint, index: number): string {
  return row.label || row.date || String(index)
}
