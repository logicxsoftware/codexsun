import { useMemo } from "react"
import { Link } from "react-router"

import { Button } from "@/components/ui/button"
import NavAuth from "@/features/web-navigation/components/NavAuth"
import NavContainer from "@/features/web-navigation/components/NavContainer"
import NavLogo from "@/features/web-navigation/components/NavLogo"
import NavMenu from "@/features/web-navigation/components/NavMenu"
import NavStickyWrapper from "@/features/web-navigation/components/NavStickyWrapper"
import NavThemeSwitch from "@/features/web-navigation/components/NavThemeSwitch"
import NavZone from "@/features/web-navigation/components/NavZone"
import { useWebNavigation } from "@/features/web-navigation/context/WebNavigationProvider"
import { cn } from "@/lib/utils"
import { resolveBackgroundClass, resolveBorderClass, resolveTextClass } from "@/features/web-navigation/utils/token-class"

const zoneJustifyMap = {
  left: "start",
  center: "center",
  right: "end",
} as const

function WebNavigationHeader() {
  const { resolved } = useWebNavigation()

  const backgroundClass = resolveBackgroundClass(resolved.headerStyle.backgroundToken, "bg-header-bg")
  const foregroundClass = resolveTextClass(resolved.headerStyle.textToken, "text-header-foreground")
  const borderClass = resolveBorderClass(resolved.headerStyle.borderToken, "border-border")

  const normalizeZoneTokens = (zone: "left" | "center" | "right", tokens: string[]): string[] => {
    if (zone !== "right") {
      return tokens
    }

    const filtered = tokens.filter((token) => token !== "themeSwitch")
    return tokens.includes("themeSwitch") ? [...filtered, "themeSwitch"] : filtered
  }

  const zoneItems = useMemo(
    () => ({
      left: normalizeZoneTokens("left", resolved.headerComponent.left),
      center: normalizeZoneTokens("center", resolved.headerComponent.center),
      right: normalizeZoneTokens("right", resolved.headerComponent.right),
    }),
    [resolved.headerComponent.center, resolved.headerComponent.left, resolved.headerComponent.right],
  )

  const renderToken = (token: string) => {
    if (token === "logo") {
      return <NavLogo key="logo" logo={resolved.headerComponent.logo} homePath="/" />
    }

    if (token === "menu") {
      return <NavMenu key="menu" groups={resolved.menus} layout={resolved.headerLayout} />
    }

    if (token === "auth") {
      return <NavAuth key="auth" config={resolved.headerComponent.auth} />
    }

    if (token === "cta" && resolved.headerComponent.cta.enabled && resolved.headerComponent.cta.label && resolved.headerComponent.cta.url) {
      return (
        <Button key="cta" asChild size="sm" className="h-9 bg-cta-bg px-3 text-cta-foreground hover:bg-cta-bg/90">
          <Link to={resolved.headerComponent.cta.url}>{resolved.headerComponent.cta.label}</Link>
        </Button>
      )
    }

    if (token === "themeSwitch") {
      return <NavThemeSwitch key="themeSwitch" />
    }

    return null
  }

  return (
    <NavStickyWrapper
      sticky={resolved.headerBehavior.sticky}
      blur={resolved.headerBehavior.blur}
      scrollShadow={resolved.headerBehavior.scrollShadow}
      transparentOnTop={resolved.headerBehavior.transparentOnTop}
      borderBottom={resolved.headerBehavior.borderBottom}
      backgroundClassName={backgroundClass}
      foregroundClassName={foregroundClass}
      borderClassName={borderClass}
    >
      <NavContainer variant={resolved.headerLayout.variant === "full" ? "full" : "container"}>
        <div className="grid min-h-16 grid-cols-3 items-center gap-2 py-2">
          {(["left", "center", "right"] as const).map((zone) => (
            <NavZone
              key={zone}
              justify={zoneJustifyMap[zone]}
              className={cn(
                zone === "center" ? "justify-self-center" : "",
                zone === "right" ? "justify-self-end" : "justify-self-start",
              )}
            >
              {zoneItems[zone].map((token) => renderToken(token))}
            </NavZone>
          ))}
        </div>
      </NavContainer>
    </NavStickyWrapper>
  )
}

export default WebNavigationHeader
