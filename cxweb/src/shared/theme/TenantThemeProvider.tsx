import { createContext, useCallback, useContext, useEffect, useLayoutEffect, useMemo, useRef, useState } from "react"
import type { ReactNode } from "react"

import { webThemeApi } from "@/features/web/services/web-theme-api"
import { getThemeSignature, resolveThemeMode, sanitizeTenantTheme } from "@/shared/theme/oklch"
import { defaultTenantTheme, type ThemeMode, type ThemePreference, type TenantTheme, themeTokenNames } from "@/shared/theme/tokens"

type TenantThemeContextValue = {
  modePreference: ThemePreference
  resolvedMode: ThemeMode
  setModePreference: (mode: ThemePreference) => void
  isLoaded: boolean
  theme: TenantTheme
}

const tenantThemeStorageKey = "cxweb.theme.mode"
const tenantThemeCacheKeyPrefix = "cxweb.theme.cache"

const tenantThemeMemoryCache = new Map<string, TenantTheme>()

const TenantThemeContext = createContext<TenantThemeContextValue | null>(null)

type TenantThemeProviderProps = {
  children: ReactNode
}

const getTenantHost = (): string => {
  if (typeof window === "undefined") {
    return "default"
  }

  return window.location.host.toLowerCase()
}

const cacheKeyForHost = (host: string): string => `${tenantThemeCacheKeyPrefix}.${host}`

const readCachedTheme = (host: string): TenantTheme | null => {
  if (typeof window === "undefined") {
    return null
  }

  const fromMemory = tenantThemeMemoryCache.get(host)
  if (fromMemory) {
    return fromMemory
  }

  const serialized = window.sessionStorage.getItem(cacheKeyForHost(host))
  if (!serialized) {
    return null
  }

  try {
    const parsed = JSON.parse(serialized) as unknown
    const sanitized = sanitizeTenantTheme(parsed)
    tenantThemeMemoryCache.set(host, sanitized)
    return sanitized
  } catch {
    return null
  }
}

const writeCachedTheme = (host: string, theme: TenantTheme): void => {
  tenantThemeMemoryCache.set(host, theme)

  if (typeof window === "undefined") {
    return
  }

  window.sessionStorage.setItem(cacheKeyForHost(host), JSON.stringify(theme))
}

const readStoredMode = (): ThemePreference | null => {
  if (typeof window === "undefined") {
    return null
  }

  const raw = window.localStorage.getItem(tenantThemeStorageKey)
  if (raw === "light" || raw === "dark" || raw === "system") {
    return raw
  }

  return null
}

const applyThemeTokens = (root: HTMLElement, mode: ThemeMode, theme: TenantTheme, appliedSignatureRef: { current: string | null }) => {
  const tokens = mode === "dark" ? theme.dark : theme.light
  const signature = getThemeSignature(mode, tokens)

  if (appliedSignatureRef.current === signature) {
    return
  }

  for (const tokenName of themeTokenNames) {
    root.style.setProperty(`--${tokenName}`, tokens[tokenName])
  }

  root.classList.toggle("dark", mode === "dark")
  root.style.colorScheme = mode
  appliedSignatureRef.current = signature
}

const getSystemDark = (): boolean => {
  if (typeof window === "undefined") {
    return false
  }

  return window.matchMedia("(prefers-color-scheme: dark)").matches
}

function TenantThemeProvider({ children }: TenantThemeProviderProps) {
  const storedModeRef = useRef<ThemePreference | null>(readStoredMode())
  const [tenantTheme, setTenantTheme] = useState<TenantTheme>(defaultTenantTheme)
  const [modePreference, setModePreferenceState] = useState<ThemePreference>(storedModeRef.current ?? "system")
  const [systemDark, setSystemDark] = useState<boolean>(() => getSystemDark())
  const [isLoaded, setIsLoaded] = useState(false)
  const appliedSignatureRef = useRef<string | null>(null)
  const hasStoredModeRef = useRef<boolean>(storedModeRef.current !== null)

  const resolvedMode = useMemo<ThemeMode>(() => resolveThemeMode(modePreference, systemDark), [modePreference, systemDark])

  const setModePreference = useCallback((nextMode: ThemePreference) => {
    setModePreferenceState(nextMode)
    hasStoredModeRef.current = true

    if (typeof window !== "undefined") {
      window.localStorage.setItem(tenantThemeStorageKey, nextMode)
    }
  }, [])

  useEffect(() => {
    if (typeof window === "undefined") {
      return
    }

    const media = window.matchMedia("(prefers-color-scheme: dark)")
    const listener = (event: MediaQueryListEvent) => {
      setSystemDark(event.matches)
    }

    media.addEventListener("change", listener)

    return () => {
      media.removeEventListener("change", listener)
    }
  }, [])

  useLayoutEffect(() => {
    if (typeof document === "undefined") {
      return
    }

    applyThemeTokens(document.documentElement, resolvedMode, tenantTheme, appliedSignatureRef)
  }, [resolvedMode, tenantTheme])

  useEffect(() => {
    let active = true
    const host = getTenantHost()

    const fromCache = readCachedTheme(host)
    if (fromCache && active) {
      setTenantTheme(fromCache)
      if (!hasStoredModeRef.current) {
        setModePreferenceState(fromCache.defaultMode)
      }
      setIsLoaded(true)
    }

    const load = async () => {
      try {
        const response = await webThemeApi.getTheme()
        if (!active) {
          return
        }

        const sanitized = sanitizeTenantTheme(response)
        writeCachedTheme(host, sanitized)
        setTenantTheme(sanitized)
        if (!hasStoredModeRef.current) {
          setModePreferenceState(sanitized.defaultMode)
        }
        setIsLoaded(true)
      } catch {
        if (!active) {
          return
        }

        const fallbackTheme = fromCache ?? defaultTenantTheme
        setTenantTheme(fallbackTheme)
        if (!hasStoredModeRef.current) {
          setModePreferenceState(fallbackTheme.defaultMode)
        }
        setIsLoaded(true)
      }
    }

    void load()

    return () => {
      active = false
    }
  }, [])

  const value = useMemo<TenantThemeContextValue>(
    () => ({
      modePreference,
      resolvedMode,
      setModePreference,
      isLoaded,
      theme: tenantTheme,
    }),
    [isLoaded, modePreference, resolvedMode, setModePreference, tenantTheme],
  )

  return <TenantThemeContext.Provider value={value}>{children}</TenantThemeContext.Provider>
}

const useTenantTheme = (): TenantThemeContextValue => {
  const context = useContext(TenantThemeContext)
  if (!context) {
    throw new Error("useTenantTheme must be used within TenantThemeProvider")
  }

  return context
}

export { TenantThemeProvider, useTenantTheme }
