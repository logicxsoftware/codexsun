import { useMemo } from "react"
import type { JSX } from "react"

import FooterAbout from "@/features/web-navigation/components/FooterAbout"
import FooterBottomBar from "@/features/web-navigation/components/FooterBottomBar"
import FooterContainer from "@/features/web-navigation/components/FooterContainer"
import FooterLegal from "@/features/web-navigation/components/FooterLegal"
import FooterLinks from "@/features/web-navigation/components/FooterLinks"
import FooterSocial from "@/features/web-navigation/components/FooterSocial"
import { useWebNavigation } from "@/features/web-navigation/context/WebNavigationProvider"
import { cn } from "@/lib/utils"

const gapClassMap = {
  compact: "gap-3",
  normal: "gap-5",
  relaxed: "gap-7",
} as const

function WebFooter() {
  const { resolved } = useWebNavigation()

  const orderedSections = useMemo(() => resolved.footerLayout.sectionOrder, [resolved.footerLayout.sectionOrder])
  const columns = Math.max(1, Math.min(6, resolved.footerLayout.columns))

  const gridClass = columns === 1 ? "grid-cols-1" : columns === 2 ? "md:grid-cols-2" : columns === 3 ? "md:grid-cols-3" : "md:grid-cols-2 lg:grid-cols-4"

  const sectionMap: Record<string, JSX.Element | null> = {
    about: <FooterAbout enabled={resolved.footerComponent.about.enabled} title={resolved.footerComponent.about.title} content={resolved.footerComponent.about.content} />,
    links: <FooterLinks enabled={resolved.footerComponent.links.enabled} menuGroupSlug={resolved.footerComponent.links.menuGroupSlug} groups={resolved.menus} />,
    legal: <FooterLegal enabled={resolved.footerComponent.legal.enabled} items={resolved.footerComponent.legal.items} />,
    social: <FooterSocial enabled={resolved.footerComponent.social.enabled} items={resolved.footerComponent.social.items} />,
    newsletter: null,
    businessHours: null,
    payments: null,
  }

  return (
    <FooterContainer layout={resolved.footerLayout} style={resolved.footerStyle}>
      <div className={cn("grid", gridClass, gapClassMap[resolved.footerStyle.columnGap])}>
        {orderedSections
          .filter((section) => section !== "bottom" && section !== "newsletter" && section !== "businessHours" && section !== "payments")
          .map((section) => (
            <div key={section}>{sectionMap[section] ?? null}</div>
          ))}
      </div>
      <FooterBottomBar
        enabled={resolved.footerComponent.bottom.enabled}
        showDynamicYear={resolved.footerBehavior.showDynamicYear}
        copyright={resolved.footerComponent.bottom.copyright}
        developedBy={resolved.footerComponent.bottom.developedBy}
      />
    </FooterContainer>
  )
}

export default WebFooter
