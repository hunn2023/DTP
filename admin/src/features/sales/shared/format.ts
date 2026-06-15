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

export const ORDER_ITEM_TYPE_LABELS: Record<number, string> = {
  1: 'eSIM',
  2: 'Thẻ cào',
}

const PAYMENT_METHOD_LABELS: Record<string, string> = {
  banking: 'Chuyển khoản',
  vnpay: 'VNPay',
  momo: 'MoMo',
  cod: 'COD',
}

export function formatPaymentMethod(value: string): string {
  if (!value) return '—'
  const key = value.trim().toLowerCase()
  return PAYMENT_METHOD_LABELS[key] ?? value
}

export function formatDateTime(value: string): string {
  if (!value) return '—'
  const date = new Date(value)
  return Number.isNaN(date.getTime()) ? value : date.toLocaleString('vi-VN')
}

const ZERO_DECIMAL_CURRENCIES = new Set(['VND', 'JPY', 'KRW'])

/** Giữ nguyên mã tiền tệ từ API; chỉ fallback khi rỗng. */
export function resolveCurrencyUnit(currency?: string): string {
  const trimmed = (currency ?? '').trim()
  return trimmed || 'VND'
}

function fractionDigitsFor(unit: string): number {
  return ZERO_DECIMAL_CURRENCIES.has(unit.toUpperCase()) ? 0 : 2
}

/** Số tiền locale vi-VN + đơn vị đúng như API (vd. `3.000 VND`). */
export function formatCurrency(amount: number, currency = 'VND'): string {
  const unit = resolveCurrencyUnit(currency)
  const digits = fractionDigitsFor(unit)
  const formatted = amount.toLocaleString('vi-VN', {
    minimumFractionDigits: digits,
    maximumFractionDigits: digits,
  })
  return `${formatted} ${unit}`
}

export function enumLabel(
  value: number | string,
  labels: Record<number, string>,
): string {
  if (typeof value === 'number') return labels[value] ?? String(value)
  const parsed = Number(value)
  return Number.isNaN(parsed) ? value : labels[parsed] ?? value
}
