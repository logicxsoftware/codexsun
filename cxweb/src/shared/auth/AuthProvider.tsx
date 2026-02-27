import { createContext, useCallback, useContext, useMemo, useState } from "react"
import type { ReactNode } from "react"

type AuthContextValue = {
  isAuthenticated: boolean
  token: string | null
  setToken: (token: string | null) => void
  signOut: () => void
}

const authStorageKey = "cxweb.auth.token"

const AuthContext = createContext<AuthContextValue | null>(null)

type AuthProviderProps = {
  children: ReactNode
}

function readStoredToken(): string | null {
  if (typeof window === "undefined") {
    return null
  }

  const token = window.localStorage.getItem(authStorageKey)
  if (!token) {
    return null
  }

  const normalized = token.trim()
  return normalized.length > 0 ? normalized : null
}

function AuthProvider({ children }: AuthProviderProps) {
  const [token, setTokenState] = useState<string | null>(() => readStoredToken())

  const setToken = useCallback((nextToken: string | null) => {
    if (typeof window !== "undefined") {
      if (nextToken && nextToken.trim().length > 0) {
        window.localStorage.setItem(authStorageKey, nextToken)
      } else {
        window.localStorage.removeItem(authStorageKey)
      }
    }

    setTokenState(nextToken && nextToken.trim().length > 0 ? nextToken : null)
  }, [])

  const signOut = useCallback(() => {
    if (typeof window !== "undefined") {
      window.localStorage.removeItem(authStorageKey)
    }
    setTokenState(null)
  }, [])

  const value = useMemo<AuthContextValue>(
    () => ({
      isAuthenticated: Boolean(token),
      token,
      setToken,
      signOut,
    }),
    [setToken, signOut, token],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

function useAuth() {
  const context = useContext(AuthContext)
  if (!context) {
    throw new Error("useAuth must be used within AuthProvider")
  }
  return context
}

export { AuthProvider, useAuth }
