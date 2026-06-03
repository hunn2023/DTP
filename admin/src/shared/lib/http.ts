/**
 * HTTP client — thay bằng axios/fetch + auth khi nối API thật.
 */
const API_BASE = import.meta.env.VITE_API_BASE_URL ?? '/api'

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

export async function httpGet<T>(path: string, config?: HttpConfig): Promise<T> {
  const res = await fetch(buildUrl(path, config?.params), {
    ...config,
    method: 'GET',
    headers: { Accept: 'application/json', ...config?.headers },
  })
  if (!res.ok) throw new Error(`[httpGet] ${path} status=${res.status}`)
  return res.json() as Promise<T>
}

export async function httpPost<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  const res = await fetch(buildUrl(path, config?.params), {
    ...config,
    method: 'POST',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...config?.headers },
    body: JSON.stringify(body),
  })
  if (!res.ok) throw new Error(`[httpPost] ${path} status=${res.status}`)
  return res.json() as Promise<T>
}

export async function httpPatch<T>(path: string, body: unknown, config?: HttpConfig): Promise<T> {
  const res = await fetch(buildUrl(path, config?.params), {
    ...config,
    method: 'PATCH',
    headers: { 'Content-Type': 'application/json', Accept: 'application/json', ...config?.headers },
    body: JSON.stringify(body),
  })
  if (!res.ok) throw new Error(`[httpPatch] ${path} status=${res.status}`)
  return res.json() as Promise<T>
}

export async function httpDelete(path: string, config?: HttpConfig): Promise<void> {
  const res = await fetch(buildUrl(path, config?.params), { ...config, method: 'DELETE' })
  if (!res.ok) throw new Error(`[httpDelete] ${path} status=${res.status}`)
}
