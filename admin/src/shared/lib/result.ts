type Raw = Record<string, unknown>

function isRecord(value: unknown): value is Raw {
  return typeof value === 'object' && value !== null && !Array.isArray(value)
}

export function isResultEnvelope(raw: unknown): raw is Raw {
  if (!isRecord(raw)) return false
  return raw.isSuccess !== undefined || raw.IsSuccess !== undefined
}

function getResultError(raw: Raw): string {
  const error = raw.error ?? raw.Error
  return typeof error === 'string' && error.trim() ? error : 'Yêu cầu thất bại'
}

/** Bóc Result<T> / Result từ .NET — throw nếu isSuccess === false. */
export function unwrapResultEnvelope<T>(raw: unknown): T {
  if (!isResultEnvelope(raw)) {
    return raw as T
  }

  const isSuccess = Boolean(raw.isSuccess ?? raw.IsSuccess)
  if (!isSuccess) {
    throw new Error(getResultError(raw))
  }

  if ('data' in raw || 'Data' in raw) {
    return (raw.data ?? raw.Data) as T
  }

  return undefined as T
}

/** @deprecated HTTP client đã tự unwrap — chỉ dùng khi parse body thủ công. */
export function unwrapResult<T>(raw: Raw): T {
  return unwrapResultEnvelope<T>(raw)
}
