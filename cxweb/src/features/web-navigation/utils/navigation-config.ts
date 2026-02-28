import type {
  FooterBehaviorConfig,
  FooterComponentConfig,
  FooterLayoutConfig,
  FooterStyleConfig,
  HeaderBehaviorConfig,
  HeaderComponentConfig,
  HeaderLayoutConfig,
  HeaderStyleConfig,
  NavigationConfigDto,
} from "@/features/web-navigation/types/navigation-types"

export const defaultHeaderLayout: HeaderLayoutConfig = {
  variant: "full",
  zoneOrder: ["left", "center", "right"],
  menuAlign: "center",
  logoPosition: "left",
  menuSize: "medium",
}

export const defaultHeaderStyle: HeaderStyleConfig = {
  backgroundToken: "header-bg",
  textToken: "header-foreground",
  hoverToken: "menu-hover",
  activeToken: "primary",
  dropdownToken: "card",
  borderToken: "border",
  scrollBackgroundToken: "header-bg",
  scrollTextToken: "foreground",
}

export const defaultHeaderBehavior: HeaderBehaviorConfig = {
  sticky: true,
  scrollShadow: true,
  transparentOnTop: false,
  blur: true,
  borderBottom: true,
  mobileOverlay: true,
}

export const defaultHeaderComponent: HeaderComponentConfig = {
  left: ["logo"],
  center: ["menu"],
  right: ["themeSwitch", "auth"],
  logo: {
    type: "text",
    text: "CodexSun",
    showText: true,
    textPosition: "right",
    size: "medium",
  },
  auth: {
    enabled: true,
    loginPath: "/auth/login",
    dashboardPath: "/app",
  },
  cta: {
    enabled: false,
    label: "",
    url: "",
    target: "_self",
  },
}

export const defaultFooterLayout: FooterLayoutConfig = {
  variant: "container",
  columns: 4,
  sectionOrder: ["about", "links", "legal", "social", "businessHours", "newsletter", "payments", "bottom"],
}

export const defaultFooterStyle: FooterStyleConfig = {
  backgroundToken: "footer-bg",
  textToken: "footer-foreground",
  linkToken: "link",
  linkHoverToken: "link-hover",
  borderTop: true,
  spacing: "normal",
  columnGap: "normal",
}

export const defaultFooterBehavior: FooterBehaviorConfig = {
  showDynamicYear: true,
  showNewsletter: false,
  showPayments: false,
}

export const defaultFooterComponent: FooterComponentConfig = {
  about: {
    enabled: true,
    title: "About",
    content: "Dynamic tenant footer.",
  },
  links: {
    enabled: true,
    menuGroupSlug: "footer",
  },
  legal: {
    enabled: true,
    items: [
      { label: "Privacy Policy", url: "/privacy-policy", target: "_self" },
      { label: "Terms", url: "/terms", target: "_self" },
      { label: "Support", url: "/support", target: "_self" },
    ],
  },
  social: {
    enabled: false,
    items: [],
  },
  newsletter: {
    enabled: false,
    title: "Newsletter",
    description: "Get updates.",
  },
  businessHours: {
    enabled: false,
    items: [],
  },
  payments: {
    enabled: false,
    providers: [],
  },
  bottom: {
    enabled: true,
    copyright: "All rights reserved",
    developedBy: {
      enabled: false,
      label: "",
      url: "",
    },
  },
}

const asRecord = (value: unknown): Record<string, unknown> => {
  if (typeof value === "object" && value !== null && !Array.isArray(value)) {
    return value as Record<string, unknown>
  }

  return {}
}

const asBoolean = (value: unknown, fallback: boolean): boolean => (typeof value === "boolean" ? value : fallback)
const asString = (value: unknown, fallback: string): string => (typeof value === "string" && value.length > 0 ? value : fallback)
const asNumber = (value: unknown, fallback: number): number => (typeof value === "number" && Number.isFinite(value) ? value : fallback)
const asStringArray = (value: unknown, fallback: string[]): string[] =>
  Array.isArray(value) ? value.filter((entry): entry is string => typeof entry === "string") : fallback

export const parseHeaderLayout = (dto: NavigationConfigDto | null): HeaderLayoutConfig => {
  const config = asRecord(dto?.layoutConfig)
  return {
    variant: config.variant === "container" ? "container" : "full",
    zoneOrder: asStringArray(config.zoneOrder, defaultHeaderLayout.zoneOrder).filter(
      (zone): zone is "left" | "center" | "right" => zone === "left" || zone === "center" || zone === "right",
    ),
    menuAlign: config.menuAlign === "left" || config.menuAlign === "right" ? config.menuAlign : "center",
    logoPosition: config.logoPosition === "center" || config.logoPosition === "right" ? config.logoPosition : "left",
    menuSize: config.menuSize === "small" || config.menuSize === "large" ? config.menuSize : "medium",
  }
}

export const parseHeaderStyle = (dto: NavigationConfigDto | null): HeaderStyleConfig => {
  const config = asRecord(dto?.styleConfig)
  return {
    backgroundToken: asString(config.backgroundToken, defaultHeaderStyle.backgroundToken),
    textToken: asString(config.textToken, defaultHeaderStyle.textToken),
    hoverToken: asString(config.hoverToken, defaultHeaderStyle.hoverToken),
    activeToken: asString(config.activeToken, defaultHeaderStyle.activeToken),
    dropdownToken: asString(config.dropdownToken, defaultHeaderStyle.dropdownToken),
    borderToken: asString(config.borderToken, defaultHeaderStyle.borderToken),
    scrollBackgroundToken: asString(config.scrollBackgroundToken, defaultHeaderStyle.scrollBackgroundToken),
    scrollTextToken: asString(config.scrollTextToken, defaultHeaderStyle.scrollTextToken),
  }
}

export const parseHeaderBehavior = (dto: NavigationConfigDto | null): HeaderBehaviorConfig => {
  const config = asRecord(dto?.behaviorConfig)
  return {
    sticky: asBoolean(config.sticky, defaultHeaderBehavior.sticky),
    scrollShadow: asBoolean(config.scrollShadow, defaultHeaderBehavior.scrollShadow),
    transparentOnTop: asBoolean(config.transparentOnTop, defaultHeaderBehavior.transparentOnTop),
    blur: asBoolean(config.blur, defaultHeaderBehavior.blur),
    borderBottom: asBoolean(config.borderBottom, defaultHeaderBehavior.borderBottom),
    mobileOverlay: asBoolean(config.mobileOverlay, defaultHeaderBehavior.mobileOverlay),
  }
}

export const parseHeaderComponent = (dto: NavigationConfigDto | null): HeaderComponentConfig => {
  const config = asRecord(dto?.componentConfig)
  const logo = asRecord(config.logo)
  const auth = asRecord(config.auth)
  const cta = asRecord(config.cta)

  return {
    left: asStringArray(config.left, defaultHeaderComponent.left),
    center: asStringArray(config.center, defaultHeaderComponent.center),
    right: asStringArray(config.right, defaultHeaderComponent.right),
    logo: {
      type: logo.type === "image" || logo.type === "inline-svg" ? logo.type : "text",
      src: typeof logo.src === "string" ? logo.src : undefined,
      svg: typeof logo.svg === "string" ? logo.svg : undefined,
      text: asString(logo.text, defaultHeaderComponent.logo.text),
      showText: asBoolean(logo.showText, defaultHeaderComponent.logo.showText),
      textPosition: logo.textPosition === "below" ? "below" : "right",
      size: logo.size === "small" || logo.size === "large" ? logo.size : "medium",
    },
    auth: {
      enabled: asBoolean(auth.enabled, defaultHeaderComponent.auth.enabled),
      loginPath: asString(auth.loginPath, defaultHeaderComponent.auth.loginPath),
      dashboardPath: asString(auth.dashboardPath, defaultHeaderComponent.auth.dashboardPath),
    },
    cta: {
      enabled: asBoolean(cta.enabled, defaultHeaderComponent.cta.enabled),
      label: asString(cta.label, defaultHeaderComponent.cta.label),
      url: asString(cta.url, defaultHeaderComponent.cta.url),
      target: cta.target === "_blank" ? "_blank" : "_self",
    },
  }
}

export const parseFooterLayout = (dto: NavigationConfigDto | null): FooterLayoutConfig => {
  const config = asRecord(dto?.layoutConfig)
  return {
    variant:
      config.variant === "full" || config.variant === "minimal" || config.variant === "centered" ? config.variant : "container",
    columns: Math.max(1, Math.min(6, asNumber(config.columns, defaultFooterLayout.columns))),
    sectionOrder: asStringArray(config.sectionOrder, defaultFooterLayout.sectionOrder),
  }
}

export const parseFooterStyle = (dto: NavigationConfigDto | null): FooterStyleConfig => {
  const config = asRecord(dto?.styleConfig)
  return {
    backgroundToken: asString(config.backgroundToken, defaultFooterStyle.backgroundToken),
    textToken: asString(config.textToken, defaultFooterStyle.textToken),
    linkToken: asString(config.linkToken, defaultFooterStyle.linkToken),
    linkHoverToken: asString(config.linkHoverToken, defaultFooterStyle.linkHoverToken),
    borderTop: asBoolean(config.borderTop, defaultFooterStyle.borderTop),
    spacing: config.spacing === "compact" || config.spacing === "relaxed" ? config.spacing : "normal",
    columnGap: config.columnGap === "compact" || config.columnGap === "relaxed" ? config.columnGap : "normal",
  }
}

export const parseFooterBehavior = (dto: NavigationConfigDto | null): FooterBehaviorConfig => {
  const config = asRecord(dto?.behaviorConfig)
  return {
    showDynamicYear: asBoolean(config.showDynamicYear, defaultFooterBehavior.showDynamicYear),
    showNewsletter: asBoolean(config.showNewsletter, defaultFooterBehavior.showNewsletter),
    showPayments: asBoolean(config.showPayments, defaultFooterBehavior.showPayments),
  }
}

export const parseFooterComponent = (dto: NavigationConfigDto | null): FooterComponentConfig => {
  const config = asRecord(dto?.componentConfig)
  const about = asRecord(config.about)
  const links = asRecord(config.links)
  const legal = asRecord(config.legal)
  const social = asRecord(config.social)
  const newsletter = asRecord(config.newsletter)
  const businessHours = asRecord(config.businessHours)
  const payments = asRecord(config.payments)
  const bottom = asRecord(config.bottom)
  const developedBy = asRecord(bottom.developedBy)

  const legalItems = Array.isArray(legal.items)
    ? legal.items
        .map((item) => asRecord(item))
        .filter((item) => typeof item.label === "string" && typeof item.url === "string")
        .map((item) => ({ label: String(item.label), url: String(item.url), target: item.target === "_blank" ? "_blank" : "_self" as "_self" | "_blank" }))
    : defaultFooterComponent.legal.items

  const socialItems = Array.isArray(social.items)
    ? social.items
        .map((item) => asRecord(item))
        .filter((item) => typeof item.label === "string" && typeof item.url === "string")
        .map((item) => ({
          icon: typeof item.icon === "string" ? item.icon : "",
          label: String(item.label),
          url: String(item.url),
          target: item.target === "_blank" ? "_blank" : "_self" as "_self" | "_blank",
        }))
    : defaultFooterComponent.social.items

  const hoursItems = Array.isArray(businessHours.items)
    ? businessHours.items
        .map((item) => asRecord(item))
        .filter((item) => typeof item.day === "string" && typeof item.hours === "string")
        .map((item) => ({ day: String(item.day), hours: String(item.hours) }))
    : defaultFooterComponent.businessHours.items

  const providerItems = Array.isArray(payments.providers)
    ? payments.providers.filter((item): item is string => typeof item === "string")
    : defaultFooterComponent.payments.providers

  return {
    about: {
      enabled: asBoolean(about.enabled, defaultFooterComponent.about.enabled),
      title: asString(about.title, defaultFooterComponent.about.title),
      content: asString(about.content, defaultFooterComponent.about.content),
    },
    links: {
      enabled: asBoolean(links.enabled, defaultFooterComponent.links.enabled),
      menuGroupSlug: asString(links.menuGroupSlug, defaultFooterComponent.links.menuGroupSlug),
    },
    legal: {
      enabled: asBoolean(legal.enabled, defaultFooterComponent.legal.enabled),
      items: legalItems,
    },
    social: {
      enabled: asBoolean(social.enabled, defaultFooterComponent.social.enabled),
      items: socialItems,
    },
    newsletter: {
      enabled: asBoolean(newsletter.enabled, defaultFooterComponent.newsletter.enabled),
      title: asString(newsletter.title, defaultFooterComponent.newsletter.title),
      description: asString(newsletter.description, defaultFooterComponent.newsletter.description),
    },
    businessHours: {
      enabled: asBoolean(businessHours.enabled, defaultFooterComponent.businessHours.enabled),
      items: hoursItems,
    },
    payments: {
      enabled: asBoolean(payments.enabled, defaultFooterComponent.payments.enabled),
      providers: providerItems,
    },
    bottom: {
      enabled: asBoolean(bottom.enabled, defaultFooterComponent.bottom.enabled),
      copyright: asString(bottom.copyright, defaultFooterComponent.bottom.copyright),
      developedBy: {
        enabled: asBoolean(developedBy.enabled, defaultFooterComponent.bottom.developedBy.enabled),
        label: asString(developedBy.label, defaultFooterComponent.bottom.developedBy.label),
        url: asString(developedBy.url, defaultFooterComponent.bottom.developedBy.url),
      },
    },
  }
}
