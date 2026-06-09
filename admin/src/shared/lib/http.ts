import { API_REQUEST_BASE } from '@/shared/config/api'
import { unwrapResultEnvelope } from '@/shared/lib/result'

/**
 * HTTP client — tự kiểm tra Result .NET: { isSuccess, data, error }.
 */
const API_BASE = API_REQUEST_BASE

export type HttpConfig = RequestInit & {
  params?: Record<string, string | number | boolean | undefined>
}

function buildUrl(path: string, params?: HttpConfig['params']): string {
  const url = path.startsWith('http') ? path : `${API_BASE}${path}`
  if (!params) return url
  const search = new URLSearchParams()
  Object.entries(params).forEach(([key, value]) => {
    if (value !== undefined) search.set(key, String(value))
  })
  const qs = search.toString()
  return qs ? `${url}?${qs}` : url
}

function sanitizeErrorMessage(message: string): string {
  return message.replace(/^System\.\w+Exception:\s*/i, '').trim()
}

async function parseResponseError(res: Response): Promise<string> {
  const text = await res.text()
  if (!text) return `Yêu cầu thất bại (${res.status})`

  try {
    const json = JSON.parse(text) as unknown
    if (typeof json === 'object' && json !== null) {
      const raw = json as Record<string, unknown>
      if (raw.isSuccess === false || raw.IsSuccess === false) {
        const msg = raw.error ?? raw.Error
        if (typeof msg === 'string' && msg.trim()) return sanitizeErrorMessage(msg)
      }
      const msg = raw.detail ?? raw.title ?? raw.message ?? raw.error ?? raw.Error
      if (typeof msg === 'string' && msg.trim()) return sanitizeErrorMessage(msg)
    }
  } catch {
    // not JSON — parse plain text / HTML below
  }

  const exceptionMatch = text.match(/System\.\w+Exception:\s*([^\r\n<]+)/)
  if (exceptionMatch?.[1]) return sanitizeErrorMessage(exceptionMatch[1])

  const firstLine = text.split(/\r?\n/).map((line) => line.trim()).find(Boolean)
  if (firstLine && firstLine.length <= 200 && !firstLine.startsWith('<')) {
    return sanitizeErrorMessage(firstLine)
  }

  return `Yêu cầu thất bại (${res.status})`
}

async function assertOk(res: Response): Promise<void> {
  if (!res.ok) throw new Error(await parseResponseError(res))
}

async function parseJsonBody<T>(res: Response): Promise<T> {
  const text = await res.text()
  if (!text) return undefined as T

  let json: unknown
  try {
    json = JSON.parse(text)
  } catch {
    throw new Error(`Yêu cầu thất bại (${res.status})`)
  }

  return unwrapResultEnvelope<T>(json)
}

async function request<T>(url: string, init: RequestInit): Promise<T> {
  let res: Response
  try {
    res = await fetch(url, init)
  } catch {
    throw new Error('Không kết nối được API. Kiểm tra backend đang chạy và CORS.')
  }
  await assertOk(res)
  return parseJsonBody<T>(res)
}

export async function httpGet<T>(path: string, config?: HttpConfig): Promise<T> {
  return request<T>(buildUrl(path, config?.params), {
    ...config,
    method: 'GET',
    headers: { Accept: 'application/json', ...config?.headers },
  })
}

export async function httpPost<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  return request<T>(buildUrl(path, config?.params), {
    ...config,
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...config?.headers },
    body: JSON.stringify(body),
  })
}

export async function httpPut<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  return request<T>(buildUrl(path, config?.params), {
    ...config,
    method: 'PUT',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...config?.headers },
    body: JSON.stringify(body),
  })
}

export async function httpPatch<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  return request<T>(buildUrl(path, config?.params), {
    ...config,
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...config?.headers },
    body: JSON.stringify(body),
  })
}

export async function httpDelete(path: string, config?: HttpConfig): Promise<void> {
  await request<unknown>(buildUrl(path, config?.params), { ...config, method: 'DELETE' })
}

export async function httpPostForm<T>(
  path: string,
  formData: FormData,
  config?: HttpConfig,
): Promise<T> {
  return request<T>(buildUrl(path, config?.params), {
    ...config,
    method: 'POST',
    headers: { Accept: 'application/json', ...config?.headers },
    body: formData,
  })
}
