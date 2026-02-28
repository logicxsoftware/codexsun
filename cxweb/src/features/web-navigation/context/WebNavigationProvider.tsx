import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react"
import type { ReactNode } from "react"

import { menuApi } from "@/features/menu-admin/services/menu-api"
import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"
import { navigationApi } from "@/features/web-navigation/services/navigation-api"
import type { NavigationConfigDto, ResolvedWebNavigationState } from "@/features/web-navigation/types/navigation-types"
import {
  defaultFooterBehavior,
  defaultFooterComponent,
  defaultFooterLayout,
  defaultFooterStyle,
  defaultHeaderBehavior,
  defaultHeaderComponent,
  defaultHeaderLayout,
  defaultHeaderStyle,
  parseFooterBehavior,
  parseFooterComponent,
  parseFooterLayout,
  parseFooterStyle,
  parseHeaderBehavior,
  parseHeaderComponent,
  parseHeaderLayout,
  parseHeaderStyle,
} from "@/features/web-navigation/utils/navigation-config"

type WebNavigationContextValue = {
  isLoading: boolean
  menus: MenuRenderGroupDto[]
  headerConfig: NavigationConfigDto | null
  footerConfig: NavigationConfigDto | null
  resolved: ResolvedWebNavigationState
  refresh: () => Promise<void>
}

const WebNavigationContext = createContext<WebNavigationContextValue | null>(null)

type WebNavigationProviderProps = {
  children: ReactNode
}

function WebNavigationProvider({ children }: WebNavigationProviderProps) {
  const [isLoading, setIsLoading] = useState(true)
  const [menus, setMenus] = useState<MenuRenderGroupDto[]>([])
  const [headerConfig, setHeaderConfig] = useState<NavigationConfigDto | null>(null)
  const [footerConfig, setFooterConfig] = useState<NavigationConfigDto | null>(null)

  const load = useCallback(async () => {
    const [menuResponse, headerResponse, footerResponse] = await Promise.allSettled([
      menuApi.getRenderMenus(),
      navigationApi.getPublicNavigationConfig(),
      navigationApi.getPublicFooterConfig(),
    ])

    if (menuResponse.status === "fulfilled") {
      setMenus(menuResponse.value)
    } else {
      setMenus([])
    }

    setHeaderConfig(headerResponse.status === "fulfilled" ? headerResponse.value : null)
    setFooterConfig(footerResponse.status === "fulfilled" ? footerResponse.value : null)
  }, [])

  useEffect(() => {
    let active = true

    const run = async () => {
      setIsLoading(true)
      await load()
      if (active) {
        setIsLoading(false)
      }
    }

    void run()

    return () => {
      active = false
    }
  }, [load])

  const refresh = useCallback(async () => {
    setIsLoading(true)
    await load()
    setIsLoading(false)
  }, [load])

  const resolved = useMemo<ResolvedWebNavigationState>(
    () => ({
      headerLayout: headerConfig ? parseHeaderLayout(headerConfig) : defaultHeaderLayout,
      headerStyle: headerConfig ? parseHeaderStyle(headerConfig) : defaultHeaderStyle,
      headerBehavior: headerConfig ? parseHeaderBehavior(headerConfig) : defaultHeaderBehavior,
      headerComponent: headerConfig ? parseHeaderComponent(headerConfig) : defaultHeaderComponent,
      footerLayout: footerConfig ? parseFooterLayout(footerConfig) : defaultFooterLayout,
      footerStyle: footerConfig ? parseFooterStyle(footerConfig) : defaultFooterStyle,
      footerBehavior: footerConfig ? parseFooterBehavior(footerConfig) : defaultFooterBehavior,
      footerComponent: footerConfig ? parseFooterComponent(footerConfig) : defaultFooterComponent,
      menus,
    }),
    [footerConfig, headerConfig, menus],
  )

  const value = useMemo<WebNavigationContextValue>(
    () => ({
      isLoading,
      menus,
      headerConfig,
      footerConfig,
      resolved,
      refresh,
    }),
    [footerConfig, headerConfig, isLoading, menus, refresh, resolved],
  )

  return <WebNavigationContext.Provider value={value}>{children}</WebNavigationContext.Provider>
}

const useWebNavigation = (): WebNavigationContextValue => {
  const context = useContext(WebNavigationContext)
  if (!context) {
    throw new Error("useWebNavigation must be used within WebNavigationProvider")
  }

  return context
}

export { WebNavigationProvider, useWebNavigation }
