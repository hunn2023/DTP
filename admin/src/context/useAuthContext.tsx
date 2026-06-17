import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react'

import * as authApi from '@/apis/authApi'
import type { AuthUser, LoginPayload } from '@/apis/authApi'
import { clearAuthTokens } from '@/shared/lib/http'
import { getErrorMessage } from '@/features/system/shared/getErrorMessage'

type AuthContextValue = {
  user: AuthUser | null
  isAuthenticated: boolean
  isBootstrapping: boolean
  login: (payload: LoginPayload) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

type AuthProviderProps = {
  children: ReactNode
}

export function AuthProvider({ children }: AuthProviderProps) {
  const [user, setUser] = useState<AuthUser | null>(null)
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [isBootstrapping, setIsBootstrapping] = useState(true)

  const resetSession = useCallback(() => {
    clearAuthTokens()
    setUser(null)
    setIsAuthenticated(false)
  }, [])

  const bootstrap = useCallback(async () => {
    try {
      const session = await authApi.restoreSession()
      if (!session) {
        resetSession()
        return
      }
      setUser(session.user)
      setIsAuthenticated(true)
    } catch {
      resetSession()
    } finally {
      setIsBootstrapping(false)
    }
  }, [resetSession])

  useEffect(() => {
    void bootstrap()
  }, [bootstrap])

  useEffect(() => {
    const onUnauthorized = () => {
      resetSession()
    }

    window.addEventListener('auth:unauthorized', onUnauthorized)
    return () => window.removeEventListener('auth:unauthorized', onUnauthorized)
  }, [resetSession])

  const login = useCallback(async (payload: LoginPayload) => {
    const session = await authApi.login(payload)
    setUser(session.user)
    setIsAuthenticated(true)
  }, [])

  const logout = useCallback(async () => {
    try {
      await authApi.logout()
    } catch (error) {
      throw new Error(getErrorMessage(error, 'Không đăng xuất được'))
    } finally {
      resetSession()
    }
  }, [resetSession])

  const value = useMemo(
    () => ({
      user,
      isAuthenticated,
      isBootstrapping,
      login,
      logout,
    }),
    [user, isAuthenticated, isBootstrapping, login, logout],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider')
  }
  return context
}
