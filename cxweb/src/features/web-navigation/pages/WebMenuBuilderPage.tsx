import { useEffect, useMemo, useState } from "react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"
import { menuApi } from "@/features/menu-admin/services/menu-api"
import { navigationApi } from "@/features/web-navigation/services/navigation-api"
import type { NavigationConfigDto } from "@/features/web-navigation/types/navigation-types"
import {
  defaultFooterBehavior,
  defaultFooterComponent,
  defaultFooterLayout,
  defaultFooterStyle,
  defaultHeaderBehavior,
  defaultHeaderComponent,
  defaultHeaderLayout,
  defaultHeaderStyle,
} from "@/features/web-navigation/utils/navigation-config"
import { showToast } from "@/shared/components/ui/toast"

const formatJson = (value: unknown): string => JSON.stringify(value, null, 2)

const parseJson = <T,>(value: string, fallback: T): T => {
  try {
    return JSON.parse(value) as T
  } catch {
    return fallback
  }
}

function WebMenuBuilderPage() {
  const [isLoading, setIsLoading] = useState(true)
  const [menuCount, setMenuCount] = useState(0)
  const [headerConfig, setHeaderConfig] = useState<NavigationConfigDto | null>(null)
  const [footerConfig, setFooterConfig] = useState<NavigationConfigDto | null>(null)
  const [headerLayoutJson, setHeaderLayoutJson] = useState(formatJson(defaultHeaderLayout))
  const [headerStyleJson, setHeaderStyleJson] = useState(formatJson(defaultHeaderStyle))
  const [headerBehaviorJson, setHeaderBehaviorJson] = useState(formatJson(defaultHeaderBehavior))
  const [headerComponentJson, setHeaderComponentJson] = useState(formatJson(defaultHeaderComponent))
  const [footerLayoutJson, setFooterLayoutJson] = useState(formatJson(defaultFooterLayout))
  const [footerStyleJson, setFooterStyleJson] = useState(formatJson(defaultFooterStyle))
  const [footerBehaviorJson, setFooterBehaviorJson] = useState(formatJson(defaultFooterBehavior))
  const [footerComponentJson, setFooterComponentJson] = useState(formatJson(defaultFooterComponent))
  const [isActive, setIsActive] = useState(true)

  useEffect(() => {
    let active = true

    const load = async () => {
      setIsLoading(true)
      const [headerResponse, footerResponse, menuResponse] = await Promise.allSettled([
        navigationApi.getAdminNavigationConfig(),
        navigationApi.getAdminFooterConfig(),
        menuApi.getRenderMenus(),
      ])

      if (!active) {
        return
      }

      if (headerResponse.status === "fulfilled") {
        setHeaderConfig(headerResponse.value)
        setHeaderLayoutJson(formatJson(headerResponse.value.layoutConfig))
        setHeaderStyleJson(formatJson(headerResponse.value.styleConfig))
        setHeaderBehaviorJson(formatJson(headerResponse.value.behaviorConfig))
        setHeaderComponentJson(formatJson(headerResponse.value.componentConfig))
        setIsActive(headerResponse.value.isActive)
      }

      if (footerResponse.status === "fulfilled") {
        setFooterConfig(footerResponse.value)
        setFooterLayoutJson(formatJson(footerResponse.value.layoutConfig))
        setFooterStyleJson(formatJson(footerResponse.value.styleConfig))
        setFooterBehaviorJson(formatJson(footerResponse.value.behaviorConfig))
        setFooterComponentJson(formatJson(footerResponse.value.componentConfig))
      }

      if (menuResponse.status === "fulfilled") {
        setMenuCount(menuResponse.value.reduce((count, group) => count + group.menus.length, 0))
      }

      setIsLoading(false)
    }

    void load()

    return () => {
      active = false
    }
  }, [])

  const metadataLabel = useMemo(
    () => ({
      header: headerConfig ? `Updated ${new Date(headerConfig.updatedAtUtc).toLocaleString()}` : "No header config",
      footer: footerConfig ? `Updated ${new Date(footerConfig.updatedAtUtc).toLocaleString()}` : "No footer config",
    }),
    [footerConfig, headerConfig],
  )

  const saveNavigation = async () => {
    await navigationApi.upsertAdminNavigationConfig({
      layoutConfig: parseJson(headerLayoutJson, defaultHeaderLayout),
      styleConfig: parseJson(headerStyleJson, defaultHeaderStyle),
      behaviorConfig: parseJson(headerBehaviorJson, defaultHeaderBehavior),
      componentConfig: parseJson(headerComponentJson, defaultHeaderComponent),
      isActive,
    })
    showToast({ title: "Navigation config saved", variant: "success" })
  }

  const saveFooter = async () => {
    await navigationApi.upsertAdminFooterConfig({
      layoutConfig: parseJson(footerLayoutJson, defaultFooterLayout),
      styleConfig: parseJson(footerStyleJson, defaultFooterStyle),
      behaviorConfig: parseJson(footerBehaviorJson, defaultFooterBehavior),
      componentConfig: parseJson(footerComponentJson, defaultFooterComponent),
      isActive,
    })
    showToast({ title: "Footer config saved", variant: "success" })
  }

  const resetNavigation = async () => {
    const reset = await navigationApi.resetAdminNavigationConfig()
    setHeaderLayoutJson(formatJson(reset.layoutConfig))
    setHeaderStyleJson(formatJson(reset.styleConfig))
    setHeaderBehaviorJson(formatJson(reset.behaviorConfig))
    setHeaderComponentJson(formatJson(reset.componentConfig))
    setIsActive(reset.isActive)
    showToast({ title: "Navigation config reset", variant: "info" })
  }

  const resetFooter = async () => {
    const reset = await navigationApi.resetAdminFooterConfig()
    setFooterLayoutJson(formatJson(reset.layoutConfig))
    setFooterStyleJson(formatJson(reset.styleConfig))
    setFooterBehaviorJson(formatJson(reset.behaviorConfig))
    setFooterComponentJson(formatJson(reset.componentConfig))
    showToast({ title: "Footer config reset", variant: "info" })
  }

  const jsonAreaClassName = "min-h-40 w-full rounded-md border border-input bg-background px-3 py-2 text-sm font-mono text-foreground"

  return (
    <div className="space-y-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <div>
          <h1 className="text-xl font-semibold">Web Navigation Builder</h1>
          <p className="text-sm text-muted-foreground">Menus available: {menuCount}</p>
        </div>
        <div className="flex items-center gap-2">
          <label htmlFor="config-active" className="text-sm text-foreground">Active</label>
          <input
            id="config-active"
            type="checkbox"
            checked={isActive}
            onChange={(event) => setIsActive(event.target.checked)}
            className="h-4 w-4 rounded border border-border bg-background"
          />
        </div>
      </div>

      <Tabs defaultValue="navigation">
        <TabsList>
          <TabsTrigger value="navigation">Main Navigation</TabsTrigger>
          <TabsTrigger value="footer">Footer</TabsTrigger>
        </TabsList>

        <TabsContent value="navigation">
          <Card>
            <CardHeader>
              <CardTitle>Main Navigation Config</CardTitle>
              <p className="text-sm text-muted-foreground">{metadataLabel.header}</p>
            </CardHeader>
            <CardContent className="grid gap-4">
              <div className="grid gap-2">
                <label htmlFor="nav-layout" className="text-sm font-medium text-foreground">Layout Config</label>
                <textarea id="nav-layout" className={jsonAreaClassName} value={headerLayoutJson} onChange={(event) => setHeaderLayoutJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="nav-style" className="text-sm font-medium text-foreground">Style Config</label>
                <textarea id="nav-style" className={jsonAreaClassName} value={headerStyleJson} onChange={(event) => setHeaderStyleJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="nav-behavior" className="text-sm font-medium text-foreground">Behavior Config</label>
                <textarea id="nav-behavior" className={jsonAreaClassName} value={headerBehaviorJson} onChange={(event) => setHeaderBehaviorJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="nav-component" className="text-sm font-medium text-foreground">Component Config</label>
                <textarea id="nav-component" className={jsonAreaClassName} value={headerComponentJson} onChange={(event) => setHeaderComponentJson(event.target.value)} />
              </div>
              <div className="flex gap-2">
                <Button type="button" onClick={() => void saveNavigation()} disabled={isLoading}>
                  Save Navigation
                </Button>
                <Button type="button" variant="outline" onClick={() => void resetNavigation()} disabled={isLoading}>
                  Reset Default
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="footer">
          <Card>
            <CardHeader>
              <CardTitle>Footer Config</CardTitle>
              <p className="text-sm text-muted-foreground">{metadataLabel.footer}</p>
            </CardHeader>
            <CardContent className="grid gap-4">
              <div className="grid gap-2">
                <label htmlFor="footer-layout" className="text-sm font-medium text-foreground">Layout Config</label>
                <textarea id="footer-layout" className={jsonAreaClassName} value={footerLayoutJson} onChange={(event) => setFooterLayoutJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="footer-style" className="text-sm font-medium text-foreground">Style Config</label>
                <textarea id="footer-style" className={jsonAreaClassName} value={footerStyleJson} onChange={(event) => setFooterStyleJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="footer-behavior" className="text-sm font-medium text-foreground">Behavior Config</label>
                <textarea id="footer-behavior" className={jsonAreaClassName} value={footerBehaviorJson} onChange={(event) => setFooterBehaviorJson(event.target.value)} />
              </div>
              <div className="grid gap-2">
                <label htmlFor="footer-component" className="text-sm font-medium text-foreground">Component Config</label>
                <textarea id="footer-component" className={jsonAreaClassName} value={footerComponentJson} onChange={(event) => setFooterComponentJson(event.target.value)} />
              </div>
              <div className="flex gap-2">
                <Button type="button" onClick={() => void saveFooter()} disabled={isLoading}>
                  Save Footer
                </Button>
                <Button type="button" variant="outline" onClick={() => void resetFooter()} disabled={isLoading}>
                  Reset Default
                </Button>
              </div>
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </div>
  )
}

export default WebMenuBuilderPage
