const backgroundClassMap: Record<string, string> = {
  background: "bg-background",
  card: "bg-card",
  "header-bg": "bg-header-bg",
  "footer-bg": "bg-footer-bg",
  muted: "bg-muted",
  secondary: "bg-secondary",
  primary: "bg-primary",
}

const textClassMap: Record<string, string> = {
  foreground: "text-foreground",
  "card-foreground": "text-card-foreground",
  "header-foreground": "text-header-foreground",
  "footer-foreground": "text-footer-foreground",
  "muted-foreground": "text-muted-foreground",
  "primary-foreground": "text-primary-foreground",
  link: "text-link",
  "link-hover": "text-link-hover",
}

const borderClassMap: Record<string, string> = {
  border: "border-border",
  "sidebar-border": "border-sidebar-border",
}

export const resolveBackgroundClass = (token: string, fallback: string): string => backgroundClassMap[token] ?? fallback
export const resolveTextClass = (token: string, fallback: string): string => textClassMap[token] ?? fallback
export const resolveBorderClass = (token: string, fallback: string): string => borderClassMap[token] ?? fallback
