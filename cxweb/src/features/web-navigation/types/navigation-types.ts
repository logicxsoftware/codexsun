import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

export type ZoneName = "left" | "center" | "right"
export type HeaderMenuAlign = "left" | "center" | "right"
export type HeaderMenuSize = "small" | "medium" | "large"
export type HeaderVariant = "full" | "container"
export type FooterVariant = "full" | "container" | "minimal" | "centered"

export type HeaderLayoutConfig = {
  variant: HeaderVariant
  zoneOrder: ZoneName[]
  menuAlign: HeaderMenuAlign
  logoPosition: ZoneName
  menuSize: HeaderMenuSize
}

export type HeaderStyleConfig = {
  backgroundToken: string
  textToken: string
  hoverToken: string
  activeToken: string
  dropdownToken: string
  borderToken: string
  scrollBackgroundToken: string
  scrollTextToken: string
}

export type HeaderBehaviorConfig = {
  sticky: boolean
  scrollShadow: boolean
  transparentOnTop: boolean
  blur: boolean
  borderBottom: boolean
  mobileOverlay: boolean
}

export type HeaderLogoConfig = {
  type: "text" | "image" | "inline-svg"
  src?: string
  svg?: string
  text: string
  showText: boolean
  textPosition: "right" | "below"
  size: "small" | "medium" | "large"
}

export type HeaderAuthConfig = {
  enabled: boolean
  loginPath: string
  dashboardPath: string
}

export type HeaderCtaConfig = {
  enabled: boolean
  label: string
  url: string
  target: "_self" | "_blank"
}

export type HeaderComponentConfig = {
  left: string[]
  center: string[]
  right: string[]
  logo: HeaderLogoConfig
  auth: HeaderAuthConfig
  cta: HeaderCtaConfig
}

export type FooterLayoutConfig = {
  variant: FooterVariant
  columns: number
  sectionOrder: string[]
}

export type FooterStyleConfig = {
  backgroundToken: string
  textToken: string
  linkToken: string
  linkHoverToken: string
  borderTop: boolean
  spacing: "compact" | "normal" | "relaxed"
  columnGap: "compact" | "normal" | "relaxed"
}

export type FooterBehaviorConfig = {
  showDynamicYear: boolean
  showNewsletter: boolean
  showPayments: boolean
}

export type FooterLinkItem = {
  label: string
  url: string
  target: "_self" | "_blank"
}

export type FooterSocialItem = {
  icon: string
  label: string
  url: string
  target: "_self" | "_blank"
}

export type FooterHoursItem = {
  day: string
  hours: string
}

export type FooterComponentConfig = {
  about: {
    enabled: boolean
    title: string
    content: string
  }
  links: {
    enabled: boolean
    menuGroupSlug: string
  }
  legal: {
    enabled: boolean
    items: FooterLinkItem[]
  }
  social: {
    enabled: boolean
    items: FooterSocialItem[]
  }
  newsletter: {
    enabled: boolean
    title: string
    description: string
  }
  businessHours: {
    enabled: boolean
    items: FooterHoursItem[]
  }
  payments: {
    enabled: boolean
    providers: string[]
  }
  bottom: {
    enabled: boolean
    copyright: string
    developedBy: {
      enabled: boolean
      label: string
      url: string
    }
  }
}

export type NavigationConfigDto = {
  id: string
  tenantId: string | null
  layoutConfig: unknown
  styleConfig: unknown
  behaviorConfig: unknown
  componentConfig: unknown
  isActive: boolean
  createdAtUtc: string
  updatedAtUtc: string
}

export type UpsertNavigationConfigPayload = {
  layoutConfig: unknown
  styleConfig: unknown
  behaviorConfig: unknown
  componentConfig: unknown
  isActive: boolean
}

export type ResolvedWebNavigationState = {
  headerLayout: HeaderLayoutConfig
  headerStyle: HeaderStyleConfig
  headerBehavior: HeaderBehaviorConfig
  headerComponent: HeaderComponentConfig
  footerLayout: FooterLayoutConfig
  footerStyle: FooterStyleConfig
  footerBehavior: FooterBehaviorConfig
  footerComponent: FooterComponentConfig
  menus: MenuRenderGroupDto[]
}
