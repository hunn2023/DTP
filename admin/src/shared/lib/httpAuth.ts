import { API_PATHS } from '@/shared/config/api'
import { unwrapResultEnvelope } from '@/shared/lib/result'

const ACCESS_TOKEN_KEY = 'accessToken'
const REFRESH_TOKEN_KEY = 'refreshToken'

let refreshPromise: Promise<string> | null = null

export function getAccessToken(): string | null {
  return localStorage.getItem(ACCESS_TOKEN_KEY)
}

export function setAuthTokens(accessToken: string, refreshToken?: string | null): void {
  localStorage.setItem(ACCESS_TOKEN_KEY, accessToken)
  if (refreshToken) {
    localStorage.setItem(REFRESH_TOKEN_KEY, refreshToken)
  }
}

export function clearAuthTokens(): void {
  localStorage.removeItem(ACCESS_TOKEN_KEY)
  localStorage.removeItem(REFRESH_TOKEN_KEY)
}

export function isAuthPath(path: string): boolean {
  return (
    path.includes(API_PATHS.authLogin) ||
    path.includes(API_PATHS.authRefresh) ||
    path.includes(API_PATHS.authLogout)
  )
}

export function buildAuthHeaders(init?: HeadersInit, includeAuth = true): Headers {
  const headers = new Headers(init)
  headers.set('X-Pinggy-No-Screen', 'true')
  if (includeAuth) {
    const token = getAccessToken()
    if (token) headers.set('Authorization', `Bearer ${token}`)
  }
  return headers
}

type RefreshTokenResponse = {
  accessToken: string
  refreshToken?: string
}

async function parseRefreshBody(res: Response): Promise<RefreshTokenResponse> {
  const text = await res.text()
  if (!text) throw new Error('Invalid refresh token response')

  const json = JSON.parse(text) as unknown
  const data = unwrapResultEnvelope<RefreshTokenResponse>(json)
  if (!data?.accessToken) throw new Error('Invalid refresh token response')
  return data
}

export async function refreshAccessToken(apiBase: string): Promise<string> {
  const storedRefresh = localStorage.getItem(REFRESH_TOKEN_KEY)
  if (!storedRefresh) throw new Error('Missing refresh token')

  if (!refreshPromise) {
    refreshPromise = (async () => {
      const url = `${apiBase}${API_PATHS.authRefresh}`
      const res = await fetch(url, {
        method: 'POST',
        headers: buildAuthHeaders({ 'Content-Type': 'application/json', Accept: 'application/json' }, false),
        body: JSON.stringify({ refreshToken: storedRefresh }),
      })
      if (!res.ok) throw new Error('Refresh token failed')

      const data = await parseRefreshBody(res)
      setAuthTokens(data.accessToken, data.refreshToken)
      return data.accessToken
    })().finally(() => {
      refreshPromise = null
    })
  }

  return refreshPromise
}

export function dispatchUnauthorized(message: string): void {
  window.dispatchEvent(
    new CustomEvent('auth:unauthorized', {
      detail: { message },
    }),
  )
}

export function dispatchForbidden(message: string): void {
  window.dispatchEvent(
    new CustomEvent('auth:forbidden', {
      detail: { message },
    }),
  )
}
