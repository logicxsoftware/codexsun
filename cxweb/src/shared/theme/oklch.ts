import { defaultTenantTheme, type ThemeMode, type ThemePreference, type ThemeTokenMap, themeTokenNames, type TenantTheme } from "@/shared/theme/tokens"

const oklchColorPattern = /^oklch\(\s*[\d.]+%?\s+[\d.]+\s+[\d.]+(?:\s*\/\s*[\d.]+%?)?\s*\)$/i

const tokenNameAliases: Record<string, (typeof themeTokenNames)[number]> = {
  cardForeground: "card-foreground",
  popoverForeground: "popover-foreground",
  primaryForeground: "primary-foreground",
  secondaryForeground: "secondary-foreground",
  mutedForeground: "muted-foreground",
  accentForeground: "accent-foreground",
  destructiveForeground: "destructive-foreground",
  chart1: "chart-1",
  chart2: "chart-2",
  chart3: "chart-3",
  chart4: "chart-4",
  chart5: "chart-5",
  sidebarForeground: "sidebar-foreground",
  sidebarPrimary: "sidebar-primary",
  sidebarPrimaryForeground: "sidebar-primary-foreground",
  sidebarAccent: "sidebar-accent",
  sidebarAccentForeground: "sidebar-accent-foreground",
  sidebarBorder: "sidebar-border",
  sidebarRing: "sidebar-ring",
  headerBg: "header-bg",
  headerForeground: "header-foreground",
  menuHover: "menu-hover",
  menuHoverUnderline: "menu-hover-underline",
  footerBg: "footer-bg",
  footerForeground: "footer-foreground",
  linkHover: "link-hover",
  ctaBg: "cta-bg",
  ctaForeground: "cta-foreground",
}

export type RawTenantThemeResponse = {
  light?: Record<string, unknown>
  dark?: Record<string, unknown>
  defaultMode?: unknown
  mode?: unknown
}

const normalizeTokenName = (value: string): (typeof themeTokenNames)[number] | null => {
  if (themeTokenNames.includes(value as (typeof themeTokenNames)[number])) {
    return value as (typeof themeTokenNames)[number]
  }

  if (value in tokenNameAliases) {
    return tokenNameAliases[value]
  }

  const kebab = value
    .replace(/([a-z0-9])([A-Z])/g, "$1-$2")
    .replace(/_/g, "-")
    .toLowerCase()

  if (themeTokenNames.includes(kebab as (typeof themeTokenNames)[number])) {
    return kebab as (typeof themeTokenNames)[number]
  }

  return null
}

export const isValidThemeTokenValue = (tokenName: string, tokenValue: string): boolean => {
  if (tokenName === "radius") {
    return /^(\d+(?:\.\d+)?)(px|rem)$/.test(tokenValue)
  }

  return oklchColorPattern.test(tokenValue)
}

const sanitizeTokenMap = (source: Record<string, unknown> | undefined, fallback: ThemeTokenMap): ThemeTokenMap => {
  const next: ThemeTokenMap = { ...fallback }

  if (!source) {
    return next
  }

  for (const [key, rawValue] of Object.entries(source)) {
    const tokenName = normalizeTokenName(key)
    if (!tokenName) {
      continue
    }

    if (typeof rawValue !== "string") {
      continue
    }

    const value = rawValue.trim()
    if (!isValidThemeTokenValue(tokenName, value)) {
      continue
    }

    next[tokenName] = value
  }

  return next
}

const normalizeMode = (rawMode: unknown): ThemePreference => {
  if (rawMode === "light" || rawMode === "dark" || rawMode === "system") {
    return rawMode
  }

  return defaultTenantTheme.defaultMode
}

export const sanitizeTenantTheme = (raw: unknown): TenantTheme => {
  if (!raw || typeof raw !== "object") {
    return defaultTenantTheme
  }

  const payload = raw as RawTenantThemeResponse

  return {
    light: sanitizeTokenMap(payload.light, defaultTenantTheme.light),
    dark: sanitizeTokenMap(payload.dark, defaultTenantTheme.dark),
    defaultMode: normalizeMode(payload.defaultMode ?? payload.mode),
  }
}

export const resolveThemeMode = (modePreference: ThemePreference, matchesSystemDark: boolean): ThemeMode => {
  if (modePreference === "system") {
    return matchesSystemDark ? "dark" : "light"
  }

  return modePreference
}

export const getThemeSignature = (mode: ThemeMode, tokens: ThemeTokenMap): string => `${mode}:${Object.values(tokens).join(";")}`
