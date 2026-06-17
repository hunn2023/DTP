import { API_PATHS } from '@/shared/config/api'
import { readString } from '@/shared/lib/dtoNormalize'
import {
  clearAuthTokens,
  getAccessToken,
  getRefreshToken,
  httpGet,
  httpPost,
  setAccessToken,
  setRefreshToken,
} from '@/shared/lib/http'

type Raw = Record<string, unknown>

export type LoginPayload = {
  email: string
  password: string
}

export type AuthUser = {
  id: string
  email: string
  fullName: string
  phone?: string
  avatarUrl?: string
}

export type LoginResponse = {
  accessToken: string
  refreshToken: string
  user: AuthUser
  permissions?: string[]
}

let restoreSessionPromise: Promise<LoginResponse | null> | null = null

function normalizeUser(raw: Raw): AuthUser {
  return {
    id: readString(raw, 'id', 'Id'),
    email: readString(raw, 'email', 'Email'),
    fullName: readString(raw, 'fullName', 'FullName'),
    phone: readString(raw, 'phone', 'Phone') || undefined,
    avatarUrl: readString(raw, 'avatarUrl', 'AvatarUrl') || undefined,
  }
}

function normalizeLoginResponse(raw: Raw): LoginResponse {
  const userRaw = (raw.user ?? raw.User ?? {}) as Raw
  const permissionsRaw = raw.permissions ?? raw.Permissions
  return {
    accessToken: readString(raw, 'accessToken', 'AccessToken'),
    refreshToken: readString(raw, 'refreshToken', 'RefreshToken'),
    user: normalizeUser(userRaw),
    permissions: Array.isArray(permissionsRaw) ? permissionsRaw.map(String) : undefined,
  }
}

function persistSession(raw: Raw): LoginResponse {
  const result = normalizeLoginResponse(raw)
  setAccessToken(result.accessToken)
  setRefreshToken(result.refreshToken)
  return result
}

function buildSessionFromProfile(user: AuthUser): LoginResponse {
  const accessToken = getAccessToken()
  const refreshToken = getRefreshToken()
  if (!accessToken || !refreshToken) {
    throw new Error('Missing auth tokens')
  }
  return { accessToken, refreshToken, user }
}

async function loadSessionFromProfile(): Promise<LoginResponse | null> {
  if (!getRefreshToken()) return null

  try {
    const user = await fetchProfile()
    return buildSessionFromProfile(user)
  } catch {
    return null
  }
}

/** Khôi phục session khi reload — dùng profile trước, chỉ refresh khi http client gặp 401. */
export function restoreSession(): Promise<LoginResponse | null> {
  if (!getRefreshToken()) return Promise.resolve(null)
  if (restoreSessionPromise) return restoreSessionPromise

  restoreSessionPromise = loadSessionFromProfile().finally(() => {
    restoreSessionPromise = null
  })
  return restoreSessionPromise
}

export async function login(payload: LoginPayload): Promise<LoginResponse> {
  const raw = await httpPost<Raw>(
    API_PATHS.authLogin,
    {
      email: payload.email.trim(),
      password: payload.password,
    },
    { includeAuth: false },
  )
  return persistSession(raw)
}

export async function refreshSession(): Promise<LoginResponse> {
  const session = await restoreSession()
  if (!session) throw new Error('Missing refresh token')
  return session
}

export async function fetchProfile(): Promise<AuthUser> {
  const raw = await httpGet<Raw>(API_PATHS.authProfile)
  return normalizeUser(raw)
}

export async function logout(): Promise<void> {
  const refreshToken = getRefreshToken()
  try {
    if (refreshToken) {
      await httpPost<void>(API_PATHS.authLogout, { refreshToken })
    }
  } finally {
    clearAuthTokens()
  }
}
