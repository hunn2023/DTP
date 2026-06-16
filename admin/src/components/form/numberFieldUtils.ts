export function toNumberInputValue(value: number, emptyWhenZero = true): string {
  if (emptyWhenZero && value === 0) return ''
  return String(value)
}

export function fromNumberInputValue(raw: string): number {
  const trimmed = raw.trim()
  if (trimmed === '') return 0
  const normalized = trimmed.replace(/\./g, '').replace(',', '.')
  const parsed = Number(normalized)
  return Number.isFinite(parsed) ? parsed : 0
}

export function formatDisplayNumber(value: number, decimalScale = 0): string {
  if (!Number.isFinite(value)) return ''
  return new Intl.NumberFormat('vi-VN', {
    minimumFractionDigits: decimalScale,
    maximumFractionDigits: decimalScale,
  }).format(value)
}
