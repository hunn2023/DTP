import { readString } from '@/shared/lib/dtoNormalize'
import { isResultEnvelope, unwrapResultEnvelope } from '@/shared/lib/result'

function asRecord(value: unknown): Record<string, unknown> | null {
  if (typeof value !== 'object' || value === null) return null
  return value as Record<string, unknown>
}

/** Parse id from POST create responses — plain guid, `{ id }`, or nested `Result<T>`. */
export function parseCreatedId(raw: unknown): string {
  if (typeof raw === 'string') return raw
  if (raw == null) return ''

  if (isResultEnvelope(raw)) {
    return parseCreatedId(unwrapResultEnvelope<unknown>(raw))
  }

  const obj = asRecord(raw)
  if (!obj) return String(raw)

  const nested = obj.id ?? obj.Id
  if (nested !== undefined) {
    const parsed = parseCreatedId(nested)
    if (parsed) return parsed
  }

  return readString(obj, 'id', 'Id')
}
