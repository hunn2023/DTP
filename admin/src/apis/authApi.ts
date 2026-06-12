import { API_PATHS } from '@/shared/config/api'
import { clearAuthTokens, httpPost, setAuthTokens } from '@/shared/lib/http'

export interface LoginPayload {
  email: string
  password: string
}

export interface RefreshTokenPayload {
  refreshToken: string
}

export interface AuthUser {
  id: string
  email: string
  fullName?: string
  roles?: string[]
}

export interface LoginResponse {
  accessToken: string
  refreshToken: string
  user: AuthUser
}

export interface RefreshTokenResponse {
  accessToken: string
  refreshToken: string
}

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const result = await httpPost<LoginResponse>(API_PATHS.authLogin, payload, { includeAuth: false })
  setAuthTokens(result.accessToken, result.refreshToken)
  return result
}

export async function refreshAuthToken(payload: RefreshTokenPayload): Promise<RefreshTokenResponse> {
  const result = await httpPost<RefreshTokenResponse>(API_PATHS.authRefresh, payload, { includeAuth: false })
  setAuthTokens(result.accessToken, result.refreshToken)
  return result
}

export async function logout(): Promise<void> {
  try {
    await httpPost<void>(API_PATHS.authLogout, {})
  } finally {
    clearAuthTokens()
  }
}
