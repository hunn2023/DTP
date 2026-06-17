import { formatDisplayNumber } from '@/components/form/numberFieldUtils'
import { ORDER_STATUS_LABELS, enumLabel, formatCurrency } from '@/features/sales/shared/format'

export function formatReportMoney(value: number): string {
  return formatCurrency(value, 'VND')
}

export function formatReportCount(value: number): string {
  return formatDisplayNumber(value)
}

export function formatOrderStatusLabel(code: string, name: string): string {
  if (name.trim()) return name
  const parsed = Number(code)
  if (!Number.isNaN(parsed)) return enumLabel(parsed, ORDER_STATUS_LABELS)
  return code || '—'
}

export function formatReportDateLabel(label: string, date: string): string {
  if (label.trim()) return label
  if (!date) return '—'
  const parsed = new Date(date)
  if (Number.isNaN(parsed.getTime())) return date
  return parsed.toLocaleDateString('vi-VN')
}
