export const ORDER_STATUS_LABELS: Record<number, string> = {
  1: 'Nháp',
  2: 'Chờ thanh toán',
  3: 'Đã thanh toán',
  4: 'Đang xử lý',
  5: 'Hoàn thành',
  6: 'Đã hủy',
  7: 'Lỗi',
}

export const ORDER_PAYMENT_STATUS_LABELS: Record<number, string> = {
  1: 'Chưa thanh toán',
  2: 'Đang chờ',
  3: 'Đã thanh toán',
  4: 'Thanh toán lỗi',
  5: 'Đã hoàn tiền',
}

export const DELIVERY_STATUS_LABELS: Record<number, string> = {
  1: 'Chờ giao',
  2: 'Đang giao',
  3: 'Đã giao',
  4: 'Lỗi',
  5: 'Đã hủy',
}

export const DELIVERY_TYPE_LABELS: Record<number, string> = {
  1: 'eSIM',
  2: 'Thẻ cào',
  3: 'Digital khác',
}

export function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString('vi-VN')
}

export function formatCurrency(amount: number, currency = 'VND'): string {
  if (currency === 'VND') {
    return `${amount.toLocaleString('vi-VN')}đ`
  }
  return `${amount.toLocaleString('vi-VN')} ${currency}`
}

export function enumLabel(
  value: number | string,
  labels: Record<number, string>,
): string {
  if (typeof value === 'number') return labels[value] ?? String(value)
  const parsed = Number(value)
  return Number.isNaN(parsed) ? value : labels[parsed] ?? value
}
