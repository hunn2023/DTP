export function toNumberInputValue(value: number, emptyWhenZero = true): string {
  if (emptyWhenZero && value === 0) return ''
  return String(value)
}

export function fromNumberInputValue(raw: string): number {
  const trimmed = raw.trim()
  if (trimmed === '') return 0
  const parsed = Number(trimmed)
  return Number.isFinite(parsed) ? parsed : 0
}
