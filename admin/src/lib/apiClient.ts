/**
 * API Client Core
 * Dùng chung cho tất cả API calls trong app
 */

const API_BASE_URL = import.meta.env.VITE_API_URL || '/api'

type HttpMethod = 'GET' | 'POST' | 'PUT' | 'PATCH' | 'DELETE'

type QueryParams = Record<string, string | number | boolean | null | undefined>

type RequestBody = Record<string, unknown> | FormData | null

interface ApiRequestConfig {
  method?: HttpMethod
  headers?: Record<string, string>
  body?: RequestBody
  timeout?: number
  includeAuth?: boolean
  _retry?: boolean
}

export interface ApiResponse<T = unknown> {
  success: boolean
  data: T
  message: string
  statusCode: number
}

export interface ApiError extends Error {
  status?: number
  data?: unknown
  response?: {
    status: number
    data: unknown
  }
}

class ApiClient {
  private baseUrl: string
  private defaultHeaders: Record<string, string>
  private authToken: string | null
  private refreshPromise: Promise<string> | null

  constructor(baseUrl: string = API_BASE_URL) {
    this.baseUrl = baseUrl

    this.defaultHeaders = {
      'Content-Type': 'application/json',
    }

    this.authToken = localStorage.getItem('accessToken')
    this.refreshPromise = null
  }

  setAuthToken(token: string): void {
    this.authToken = token
    localStorage.setItem('accessToken', token)
  }

  setRefreshToken(token: string): void {
    localStorage.setItem('refreshToken', token)
  }

  setTokens(accessToken: string, refreshToken?: string | null): void {
    this.setAuthToken(accessToken)

    if (refreshToken) {
      this.setRefreshToken(refreshToken)
    }
  }

  clearAuthToken(): void {
    this.authToken = null
    localStorage.removeItem('accessToken')
    localStorage.removeItem('refreshToken')
  }

  private _buildHeaders(
    includeAuth: boolean = true,
    customHeaders: Record<string, string> = {}
  ): Record<string, string> {
    const headers: Record<string, string> = {
      ...this.defaultHeaders,
      ...customHeaders,
      'X-Pinggy-No-Screen': 'true',
    }

    if (includeAuth && this.authToken) {
      headers.Authorization = `Bearer ${this.authToken}`
    }

    return headers
  }

  private async _parseResponse<T = unknown>(response: Response): Promise<T> {
    const contentType = response.headers.get('content-type')

    if (contentType && contentType.includes('application/json')) {
      return response.json() as Promise<T>
    }

    return response.text() as Promise<T>
  }

  private _handleError(response: Response, data: any): never {
    console.log('API ERROR RESPONSE:', data)

    const message =
      data?.message ||
      data?.errors?.[0] ||
      data?.title ||
      `HTTP error! status: ${response.status}`

    const error = new Error(message) as ApiError

    error.status = response.status
    error.data = data
    error.response = {
      status: response.status,
      data,
    }

    throw error
  }

  private _isAuthEndpoint(endpoint: string): boolean {
    return (
      endpoint.includes('/auth/login') ||
      endpoint.includes('/auth/refresh-token') ||
      endpoint.includes('/auth/logout')
    )
  }

  async refreshToken(): Promise<string> {
    const refreshToken = localStorage.getItem('refreshToken')

    if (!refreshToken) {
      throw new Error('Missing refresh token')
    }

    if (!this.refreshPromise) {
      this.refreshPromise = fetch(`${this.baseUrl}/auth/refresh-token`, {
        method: 'POST',
        headers: {
          ...this.defaultHeaders,
          'X-Pinggy-No-Screen': 'true',
        },
        body: JSON.stringify({ refreshToken }),
      })
        .then(async response => {
          const data: any = await this._parseResponse(response)

          if (!response.ok) {
            throw new Error(data?.message || 'Refresh token failed')
          }

          const result = data?.data || data

          const newAccessToken = result?.accessToken
          const newRefreshToken = result?.refreshToken

          if (!newAccessToken) {
            throw new Error('Invalid refresh token response')
          }

          this.setTokens(newAccessToken, newRefreshToken)

          return newAccessToken
        })
        .finally(() => {
          this.refreshPromise = null
        })
    }

    return this.refreshPromise
  }

  async request<T = unknown>(
    endpoint: string,
    config: ApiRequestConfig = {}
  ): Promise<ApiResponse<T>> {
    const {
      method = 'GET',
      headers = {},
      body = null,
      timeout = 30000,
      includeAuth = true,
      _retry = false,
    } = config

    const url = `${this.baseUrl}${endpoint}`

    const controller = new AbortController()
    const timeoutId = window.setTimeout(() => controller.abort(), timeout)

    try {
      const finalHeaders = this._buildHeaders(includeAuth, headers)

      const fetchConfig: RequestInit = {
        method,
        headers: finalHeaders,
        signal: controller.signal,
      }

      if (body && ['POST', 'PUT', 'PATCH'].includes(method)) {
        if (body instanceof FormData) {
          delete finalHeaders['Content-Type']
          fetchConfig.body = body
        } else {
          fetchConfig.body = JSON.stringify(body)
        }
      }

      const response = await fetch(url, fetchConfig)
      window.clearTimeout(timeoutId)

      const data: any = await this._parseResponse(response)

      if (!response.ok) {
        /**
         * 401 = chưa xác thực / access token hết hạn
         * => thử refresh token nếu không phải endpoint auth
         */
        if (
          response.status === 401 &&
          includeAuth &&
          !_retry &&
          !this._isAuthEndpoint(endpoint)
        ) {
          try {
            await this.refreshToken()

            return this.request<T>(endpoint, {
              ...config,
              _retry: true,
            })
          } catch (refreshError) {
            this.clearAuthToken()

            window.dispatchEvent(
              new CustomEvent('auth:unauthorized', {
                detail: {
                  message: 'Phiên đăng nhập đã hết hạn. Vui lòng đăng nhập lại.',
                },
              })
            )

            throw refreshError
          }
        }

        /**
         * 403 = đã đăng nhập nhưng không có quyền
         * Không refresh token
         */
        if (response.status === 403) {
          window.dispatchEvent(
            new CustomEvent('auth:forbidden', {
              detail: {
                message:
                  data?.message ||
                  'Bạn không có quyền thực hiện chức năng này.',
              },
            })
          )
        }

        this._handleError(response, data)
      }

      return {
        success: true,
        data,
        message: data?.message || 'Success',
        statusCode: response.status,
      }
    } catch (error: any) {
      window.clearTimeout(timeoutId)

      if (error?.name === 'AbortError') {
        throw new Error('Request timeout')
      }

      console.error(`API Error: ${method} ${endpoint}`, error)
      throw error
    }
  }

  get<T = unknown>(
    endpoint: string,
    params: QueryParams = {},
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    const cleanParams: Record<string, string> = {}

    Object.entries(params).forEach(([key, value]) => {
      if (value !== null && value !== undefined) {
        cleanParams[key] = String(value)
      }
    })

    const queryString = new URLSearchParams(cleanParams).toString()
    const url = queryString ? `${endpoint}?${queryString}` : endpoint

    return this.request<T>(url, {
      ...config,
      method: 'GET',
    })
  }

  post<T = unknown>(
    endpoint: string,
    data?: RequestBody,
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'POST',
      body: data ?? null,
    })
  }

  put<T = unknown>(
    endpoint: string,
    data?: RequestBody,
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'PUT',
      body: data ?? null,
    })
  }

  patch<T = unknown>(
    endpoint: string,
    data?: RequestBody,
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'PATCH',
      body: data ?? null,
    })
  }

  delete<T = unknown>(
    endpoint: string,
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'DELETE',
    })
  }

  postFormData<T = unknown>(
    endpoint: string,
    formData: FormData,
    config: Omit<ApiRequestConfig, 'method' | 'body'> = {}
  ): Promise<ApiResponse<T>> {
    return this.request<T>(endpoint, {
      ...config,
      method: 'POST',
      body: formData,
      includeAuth: config.includeAuth !== false,
      headers: {
        ...config.headers,
      },
    })
  }
}

export const apiClient = new ApiClient()

const api = {
  getAll: <T = unknown>(endpoint: string, params: QueryParams = {}) =>
    apiClient.get<T>(endpoint, params),

  getById: <T = unknown>(endpoint: string, id: string) =>
    apiClient.get<T>(`${endpoint}/${id}`),

  create: <T = unknown>(endpoint: string, data: RequestBody) =>
    apiClient.post<T>(endpoint, data),

  update: <T = unknown>(endpoint: string, id: string, data: RequestBody) =>
    apiClient.put<T>(`${endpoint}/${id}`, data),

  patch: <T = unknown>(endpoint: string, id: string, data: RequestBody) =>
    apiClient.patch<T>(`${endpoint}/${id}`, data),

  remove: <T = unknown>(endpoint: string, id: string) =>
    apiClient.delete<T>(`${endpoint}/${id}`),

  setToken: (token: string) => apiClient.setAuthToken(token),

  setRefreshToken: (token: string) => apiClient.setRefreshToken(token),

  setTokens: (accessToken: string, refreshToken?: string | null) =>
    apiClient.setTokens(accessToken, refreshToken),

  clearToken: () => apiClient.clearAuthToken(),

  instance: apiClient,
}

export default api