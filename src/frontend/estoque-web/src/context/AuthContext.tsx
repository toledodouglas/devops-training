import {
  createContext,
  useCallback,
  useContext,
  useMemo,
  useState,
  type ReactNode,
} from 'react'
import * as authApi from '../api/authApi'
import { clearToken as clearStoredToken, setToken } from '../api/authStorage'
import type { RegisterRequest } from '../api/types'

type AuthContextValue = {
  isAuthenticated: boolean
  loading: boolean
  error?: string | null
  login: (email: string, senha: string) => Promise<void>
  register: (payload: RegisterRequest) => Promise<void>
  logout: () => void
  clearError: () => void
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState<string | null>(null)

  const [hasToken, setHasTokenState] = useState(() =>
    Boolean(localStorage.getItem('estoque_token'))
  )

  const logout = useCallback(() => {
    clearStoredToken()
    setHasTokenState(false)
  }, [])

  const login = useCallback(
    async (email: string, senha: string) => {
      setLoading(true)
      setError(null)
      try {
        const res = await authApi.login({
          email: email.trim(),
          senha,
        })
        setToken(res.accessToken)
        setHasTokenState(true)
      } catch (e) {
        const msg = e instanceof Error ? e.message : 'Erro ao entrar.'
        setError(msg)
        throw e
      } finally {
        setLoading(false)
      }
    },
    []
  )

  const registerFn = useCallback(async (payload: RegisterRequest) => {
    setLoading(true)
    setError(null)
      try {
        await authApi.register(payload)
      } catch (e) {
        const msg =
          e instanceof Error ? e.message : 'Erro ao cadastrar.'
        setError(msg)
        throw e
      } finally {
        setLoading(false)
      }
  }, [])

  const clearError = useCallback(() => setError(null), [])

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: hasToken,
      loading,
      error,
      login,
      register: registerFn,
      logout,
      clearError,
    }),
    [clearError, error, hasToken, loading, login, logout, registerFn]
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth(): AuthContextValue {
  const ctx = useContext(AuthContext)
  if (!ctx) {
    throw new Error('useAuth must be used inside AuthProvider')
  }
  return ctx
}
