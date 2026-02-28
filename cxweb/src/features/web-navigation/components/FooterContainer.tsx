import type { ReactNode } from "react"

import type { FooterLayoutConfig, FooterStyleConfig } from "@/features/web-navigation/types/navigation-types"
import { cn } from "@/lib/utils"
import { resolveBackgroundClass, resolveBorderClass, resolveTextClass } from "@/features/web-navigation/utils/token-class"

type FooterContainerProps = {
  children: ReactNode
  layout: FooterLayoutConfig
  style: FooterStyleConfig
}

function FooterContainer({ children, layout: _layout, style }: FooterContainerProps) {
  const widthClass = "w-full px-5"
  const spacingClass = style.spacing === "compact" ? "py-5" : style.spacing === "relaxed" ? "py-10" : "py-7"
  const borderClass = style.borderTop ? cn("border-t", resolveBorderClass("border", "border-border")) : ""

  return (
    <footer className={cn(resolveBackgroundClass(style.backgroundToken, "bg-footer-bg"), resolveTextClass(style.textToken, "text-footer-foreground"), borderClass)}>
      <div className={cn(widthClass, spacingClass)}>{children}</div>
    </footer>
  )
}

export default FooterContainer
