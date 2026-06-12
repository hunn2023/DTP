import { apiClient } from '@/lib/apiClient'

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

const authApi = {
  login: (payload: LoginPayload) => {
    return apiClient.post<LoginResponse>('/auth/login', payload)
  },

  refreshToken: (payload: RefreshTokenPayload) => {
    return apiClient.post<RefreshTokenResponse>('/auth/refresh-token', payload)
  },

  logout: () => {
    return apiClient.post<void>('/auth/logout')
  },
}

export default authApi