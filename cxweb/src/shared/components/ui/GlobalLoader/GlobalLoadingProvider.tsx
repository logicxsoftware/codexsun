import { createContext, type ReactNode, useCallback, useMemo, useSyncExternalStore } from "react"

import { GlobalLoader } from "@/shared/components/ui/GlobalLoader/GlobalLoader"
import {
  getGlobalLoadingSnapshot,
  setGlobalLoading,
  startGlobalLoading,
  stopGlobalLoading,
  subscribeGlobalLoading,
} from "@/shared/components/ui/GlobalLoader/loading-store"

type GlobalLoadingContextValue = {
  isLoading: boolean
  startLoading: () => void
  stopLoading: () => void
  setLoading: (isLoading: boolean) => void
}

export const GlobalLoadingContext = createContext<GlobalLoadingContextValue | null>(null)

type GlobalLoadingProviderProps = {
  children: ReactNode
}

export function GlobalLoadingProvider({ children }: GlobalLoadingProviderProps) {
  const isLoading = useSyncExternalStore(subscribeGlobalLoading, getGlobalLoadingSnapshot, () => false)

  const startLoading = useCallback(() => {
    startGlobalLoading()
  }, [])

  const stopLoading = useCallback(() => {
    stopGlobalLoading()
  }, [])

  const setLoading = useCallback((loading: boolean) => {
    setGlobalLoading(loading)
  }, [])

  const value = useMemo<GlobalLoadingContextValue>(
    () => ({
      isLoading,
      startLoading,
      stopLoading,
      setLoading,
    }),
    [isLoading, setLoading, startLoading, stopLoading],
  )

  return (
    <GlobalLoadingContext.Provider value={value}>
      {children}
      <GlobalLoader active={isLoading} />
    </GlobalLoadingContext.Provider>
  )
}
