import { API_REQUEST_BASE } from '@/shared/config/api'
import { unwrapResultEnvelope } from '@/shared/lib/result'
import {
  buildAuthHeaders,
  clearAuthTokens,
  dispatchForbidden,
  dispatchUnauthorized,
  isAuthPath,
  refreshAccessToken,
} from '@/shared/lib/httpAuth'

export {
  clearAuthTokens,
  getAccessToken,
  setAccessToken,
  clearAccessToken,
  getRefreshToken,
  setRefreshToken,
  clearRefreshToken,
} from '@/shared/lib/httpAuth'

/**
 * HTTP client — Bearer auth, refresh token, unwrap Result .NET { isSuccess, data, error }.
 */
const API_BASE = API_REQUEST_BASE

export type HttpConfig = RequestInit & {
  params?: Record<string, string | number | boolean | undefined>
  includeAuth?: boolean
  _retry?: boolean
}

type RequestAuthOptions = {
  includeAuth?: boolean
  _retry?: boolean
}

function splitHttpConfig(config?: HttpConfig): {
  params?: HttpConfig['params']
  auth: RequestAuthOptions
  init: RequestInit
} {
  const { params, includeAuth, _retry, ...init } = config ?? {}
  return { params, auth: { includeAuth, _retry }, init }
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
    // not JSON
  }

  const exceptionMatch = text.match(/System\.\w+Exception:\s*([^\r\n<]+)/)
  if (exceptionMatch?.[1]) return sanitizeErrorMessage(exceptionMatch[1])

  const firstLine = text.split(/\r?\n/).map((line) => line.trim()).find(Boolean)
  if (firstLine && firstLine.length <= 200 && !firstLine.startsWith('<')) {
    return sanitizeErrorMessage(firstLine)
  }

  return `Yêu cầu thất bại (${res.status})`
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

async function handleUnauthorized(url: string, auth: RequestAuthOptions): Promise<boolean> {
  const includeAuth = auth.includeAuth !== false
  if (!includeAuth || auth._retry || isAuthPath(url)) return false

  try {
    await refreshAccessToken(API_BASE)
    return true
  } catch {
    clearAuthTokens()
    dispatchUnauthorized('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.')
    throw new Error('Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.')
  }
}

async function request<T>(url: string, init: RequestInit, auth: RequestAuthOptions = {}): Promise<T> {
  const includeAuth = auth.includeAuth !== false
  const headers = buildAuthHeaders(init.headers, includeAuth)

  let res: Response
  try {
    res = await fetch(url, { ...init, headers })
  } catch {
    throw new Error('Không kết nối được API. Kiểm tra backend đang chạy và CORS.')
  }

  if (!res.ok) {
    if (res.status === 401 && (await handleUnauthorized(url, auth))) {
      return request<T>(url, init, { ...auth, _retry: true })
    }

    const message = await parseResponseError(res)

    if (res.status === 403) {
      dispatchForbidden(message || 'Bạn không có quyền thực hiện chức năng này.')
    }

    throw new Error(message)
  }

  return parseJsonBody<T>(res)
}

export async function httpGet<T>(path: string, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  return request<T>(
    buildUrl(path, params),
    { ...init, method: 'GET', headers: { Accept: 'application/json', ...init.headers } },
    auth,
  )
}

export async function httpPost<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  return request<T>(
    buildUrl(path, params),
    {
      ...init,
      method: 'POST',
      headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...init.headers },
      body: JSON.stringify(body),
    },
    auth,
  )
}

export async function httpPut<T>(path: string, body?: unknown, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  const headers = new Headers({ Accept: 'application/json' })
  if (init.headers) {
    new Headers(init.headers).forEach((value, key) => headers.set(key, value))
  }
  const requestInit: RequestInit = { ...init, method: 'PUT', headers }
  if (body !== undefined) {
    headers.set('Content-Type', 'application/json')
    requestInit.body = JSON.stringify(body)
  }
  return request<T>(buildUrl(path, params), requestInit, auth)
}

export async function httpPatch<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  return request<T>(
    buildUrl(path, params),
    {
      ...init,
      method: 'PATCH',
      headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...init.headers },
      body: JSON.stringify(body),
    },
    auth,
  )
}

export async function httpDelete(path: string, config?: HttpConfig): Promise<void> {
  const { params, auth, init } = splitHttpConfig(config)
  await request<unknown>(buildUrl(path, params), { ...init, method: 'DELETE' }, auth)
}

export async function httpPostForm<T>(path: string, formData: FormData, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  return request<T>(
    buildUrl(path, params),
    { ...init, method: 'POST', headers: { Accept: 'application/json', ...init.headers }, body: formData },
    auth,
  )
}

export async function httpPutForm<T>(path: string, formData: FormData, config?: HttpConfig): Promise<T> {
  const { params, auth, init } = splitHttpConfig(config)
  return request<T>(
    buildUrl(path, params),
    { ...init, method: 'PUT', headers: { Accept: 'application/json', ...init.headers }, body: formData },
    auth,
  )
}
