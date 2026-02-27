import { useEffect, useMemo, useRef, useState } from "react"
import { Link } from "react-router"
import { Bell, Loader2 } from "lucide-react"

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { showToast } from "@/shared/components/ui/toast"
import { cn } from "@/lib/utils"
import { isValidThemeTokenValue } from "@/shared/theme/oklch"
import { useTenantTheme } from "@/shared/theme"
import { defaultDarkTokens, defaultLightTokens, type ThemeMode, type ThemeTokenMap, themeTokenNames } from "@/shared/theme/tokens"

type DraftByMode = {
  light: ThemeTokenMap
  dark: ThemeTokenMap
}

type Preset = {
  name: string
  light: Partial<ThemeTokenMap>
  dark: Partial<ThemeTokenMap>
}

const corePreviewOrder = [
  "background",
  "foreground",
  "primary",
  "primary-foreground",
  "secondary",
  "secondary-foreground",
  "accent",
  "accent-foreground",
  "muted",
  "muted-foreground",
  "destructive",
  "destructive-foreground",
  "border",
  "input",
  "ring",
  "sidebar",
  "sidebar-foreground",
  "header-bg",
  "header-foreground",
  "footer-bg",
  "footer-foreground",
  "brand",
  "link",
  "link-hover",
  "cta-bg",
  "cta-foreground",
] as const

const allTokenOrder = [...corePreviewOrder, ...themeTokenNames.filter((name) => !corePreviewOrder.includes(name as (typeof corePreviewOrder)[number]))]

const presets: Preset[] = [
  {
    name: "Ocean",
    light: {
      primary: "oklch(0.58 0.15 232)",
      "primary-foreground": "oklch(0.985 0 0)",
      accent: "oklch(0.9 0.08 196)",
      brand: "oklch(0.61 0.14 226)",
      "header-bg": "oklch(0.97 0.03 220 / 96%)",
      "footer-bg": "oklch(0.93 0.03 220)",
      link: "oklch(0.48 0.15 230)",
      "link-hover": "oklch(0.4 0.15 230)",
      "cta-bg": "oklch(0.56 0.16 234)",
      "cta-foreground": "oklch(0.98 0 0)",
    },
    dark: {
      primary: "oklch(0.76 0.11 230)",
      "primary-foreground": "oklch(0.18 0.02 250)",
      accent: "oklch(0.34 0.06 210)",
      brand: "oklch(0.8 0.1 226)",
      "header-bg": "oklch(0.22 0.03 235 / 94%)",
      "footer-bg": "oklch(0.2 0.03 235)",
      link: "oklch(0.82 0.1 228)",
      "link-hover": "oklch(0.9 0.07 226)",
      "cta-bg": "oklch(0.74 0.11 232)",
      "cta-foreground": "oklch(0.2 0.02 252)",
    },
  },
  {
    name: "Amber",
    light: {
      primary: "oklch(0.66 0.15 70)",
      "primary-foreground": "oklch(0.18 0.02 72)",
      accent: "oklch(0.93 0.06 88)",
      brand: "oklch(0.68 0.15 72)",
      "header-bg": "oklch(0.98 0.03 86 / 96%)",
      "footer-bg": "oklch(0.95 0.04 86)",
      link: "oklch(0.56 0.14 66)",
      "link-hover": "oklch(0.48 0.13 66)",
      "cta-bg": "oklch(0.66 0.15 70)",
      "cta-foreground": "oklch(0.18 0.02 72)",
    },
    dark: {
      primary: "oklch(0.8 0.12 82)",
      "primary-foreground": "oklch(0.2 0.03 78)",
      accent: "oklch(0.35 0.07 72)",
      brand: "oklch(0.82 0.11 84)",
      "header-bg": "oklch(0.22 0.03 78 / 94%)",
      "footer-bg": "oklch(0.2 0.03 78)",
      link: "oklch(0.84 0.1 84)",
      "link-hover": "oklch(0.92 0.08 84)",
      "cta-bg": "oklch(0.8 0.12 82)",
      "cta-foreground": "oklch(0.2 0.03 78)",
    },
  },
]

const debounceMs = 120

function useDebouncedValue<T>(value: T, delayMs: number): T {
  const [debouncedValue, setDebouncedValue] = useState(value)

  useEffect(() => {
    const timeout = window.setTimeout(() => {
      setDebouncedValue(value)
    }, delayMs)

    return () => {
      window.clearTimeout(timeout)
    }
  }, [delayMs, value])

  return debouncedValue
}

function sanitizeModeTokens(current: ThemeTokenMap, fallback: ThemeTokenMap): ThemeTokenMap {
  const next = { ...fallback }

  for (const tokenName of themeTokenNames) {
    const tokenValue = current[tokenName]
    next[tokenName] = isValidThemeTokenValue(tokenName, tokenValue) ? tokenValue : fallback[tokenName]
  }

  return next
}

function ThemePreviewPage() {
  const { theme } = useTenantTheme()
  const [previewMode, setPreviewMode] = useState<ThemeMode>("light")
  const [draft, setDraft] = useState<DraftByMode>({
    light: theme.light,
    dark: theme.dark,
  })

  const debouncedDraft = useDebouncedValue(draft, debounceMs)
  const scopeRef = useRef<HTMLDivElement | null>(null)

  useEffect(() => {
    setDraft({
      light: theme.light,
      dark: theme.dark,
    })
  }, [theme.dark, theme.light])

  const sanitizedDraft = useMemo<DraftByMode>(
    () => ({
      light: sanitizeModeTokens(debouncedDraft.light, theme.light),
      dark: sanitizeModeTokens(debouncedDraft.dark, theme.dark),
    }),
    [debouncedDraft.dark, debouncedDraft.light, theme.dark, theme.light],
  )

  useEffect(() => {
    const scope = scopeRef.current
    if (!scope) {
      return
    }

    const tokens = previewMode === "dark" ? sanitizedDraft.dark : sanitizedDraft.light
    for (const tokenName of themeTokenNames) {
      scope.style.setProperty(`--${tokenName}`, tokens[tokenName])
    }

    scope.classList.toggle("dark", previewMode === "dark")
  }, [previewMode, sanitizedDraft.dark, sanitizedDraft.light])

  const activeDraft = previewMode === "dark" ? draft.dark : draft.light

  const invalidTokens = useMemo(
    () => themeTokenNames.filter((tokenName) => !isValidThemeTokenValue(tokenName, activeDraft[tokenName])),
    [activeDraft],
  )

  const onTokenChange = (tokenName: (typeof themeTokenNames)[number], value: string) => {
    setDraft((prev) => ({
      ...prev,
      [previewMode]: {
        ...prev[previewMode],
        [tokenName]: value,
      },
    }))
  }

  const onReset = () => {
    setDraft({
      light: theme.light,
      dark: theme.dark,
    })
  }

  const applyPreset = (preset: Preset) => {
    setDraft((prev) => ({
      light: {
        ...prev.light,
        ...preset.light,
      },
      dark: {
        ...prev.dark,
        ...preset.dark,
      },
    }))
  }

  const applyDefaults = () => {
    setDraft({
      light: defaultLightTokens,
      dark: defaultDarkTokens,
    })
  }

  return (
    <div className="min-h-screen bg-background px-4 py-6 text-foreground md:px-6 md:py-8">
      <div className="mx-auto grid w-full max-w-[1600px] gap-6 xl:grid-cols-[420px_minmax(0,1fr)]">
        <Card className="h-fit border-border/80 bg-card/95">
          <CardHeader>
            <CardTitle>Theme Preview Sandbox</CardTitle>
            <CardDescription>Edit OKLCH tokens and preview components in isolation.</CardDescription>
          </CardHeader>
          <CardContent className="grid gap-4">
            <div className="flex flex-wrap items-center gap-2">
              <Button
                type="button"
                variant={previewMode === "light" ? "default" : "outline"}
                onClick={() => {
                  setPreviewMode("light")
                }}
              >
                Light
              </Button>
              <Button
                type="button"
                variant={previewMode === "dark" ? "default" : "outline"}
                onClick={() => {
                  setPreviewMode("dark")
                }}
              >
                Dark
              </Button>
              <Button type="button" variant="outline" onClick={onReset}>
                Reset Tenant
              </Button>
              <Button type="button" variant="outline" onClick={applyDefaults}>
                Reset Base
              </Button>
            </div>

            <div className="flex flex-wrap gap-2">
              {presets.map((preset) => (
                <Button
                  key={preset.name}
                  type="button"
                  size="sm"
                  variant="secondary"
                  onClick={() => {
                    applyPreset(preset)
                  }}
                >
                  {preset.name}
                </Button>
              ))}
            </div>

            {invalidTokens.length > 0 ? (
              <Alert variant="destructive">
                <Bell className="h-4 w-4" />
                <AlertTitle>Invalid OKLCH Tokens</AlertTitle>
                <AlertDescription>{invalidTokens.join(", ")}</AlertDescription>
              </Alert>
            ) : null}

            <div className="max-h-[70vh] space-y-3 overflow-y-auto pr-1">
              {allTokenOrder.map((tokenName) => {
                const value = activeDraft[tokenName]
                const valid = isValidThemeTokenValue(tokenName, value)

                return (
                  <div key={`${previewMode}-${tokenName}`} className="grid gap-1">
                    <label className="text-xs font-medium uppercase tracking-wide text-muted-foreground" htmlFor={`${previewMode}-${tokenName}`}>
                      {tokenName}
                    </label>
                    <Input
                      id={`${previewMode}-${tokenName}`}
                      value={value}
                      onChange={(event) => {
                        onTokenChange(tokenName, event.target.value)
                      }}
                      className={cn(!valid && "border-destructive focus-visible:ring-destructive/30")}
                    />
                  </div>
                )
              })}
            </div>
          </CardContent>
        </Card>

        <div className="rounded-xl border border-border/70 bg-card/20 p-3 md:p-4">
          <div ref={scopeRef} className="rounded-lg border border-border/70 bg-background text-foreground">
            <div className="border-b border-border/70 bg-header-bg text-header-foreground">
              <div className="flex items-center justify-between gap-2 px-4 py-3">
                <div className="flex items-center gap-2">
                  <div className="h-8 w-8 rounded-md bg-brand/20" />
                  <span className="text-sm font-semibold">Theme Preview</span>
                </div>
                <div className="flex items-center gap-2">
                  <Button size="sm" variant="ghost" className="hover:bg-menu-hover">
                    Docs
                  </Button>
                  <Button size="sm" className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
                    Action
                  </Button>
                </div>
              </div>
            </div>

            <div className="grid gap-6 p-4 lg:grid-cols-[260px_minmax(0,1fr)]">
              <aside className="rounded-lg border border-sidebar-border bg-sidebar p-3 text-sidebar-foreground">
                <div className="mb-2 text-xs uppercase tracking-wide text-sidebar-foreground/70">Sidebar</div>
                <div className="grid gap-1 text-sm">
                  <button type="button" className="rounded-md bg-sidebar-primary px-3 py-2 text-left text-sidebar-primary-foreground">
                    Overview
                  </button>
                  <button type="button" className="rounded-md px-3 py-2 text-left hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
                    Analytics
                  </button>
                  <button type="button" className="rounded-md px-3 py-2 text-left hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
                    Settings
                  </button>
                </div>
              </aside>

              <div className="grid gap-6">
                <Card className="border-border/70 bg-card">
                  <CardHeader>
                    <CardTitle>Components</CardTitle>
                    <CardDescription>Token-driven controls and surfaces.</CardDescription>
                  </CardHeader>
                  <CardContent className="grid gap-4">
                    <div className="flex flex-wrap gap-2">
                      <Button>Default</Button>
                      <Button variant="secondary">Secondary</Button>
                      <Button variant="outline">Outline</Button>
                      <Button variant="destructive">Destructive</Button>
                      <Button variant="ghost">Ghost</Button>
                      <Button variant="link" asChild>
                        <Link to="/">Link</Link>
                      </Button>
                    </div>

                    <div className="grid gap-3 md:grid-cols-2">
                      <Input placeholder="Input token surface" />
                      <Select defaultValue="option-1">
                        <SelectTrigger className="w-full">
                          <SelectValue placeholder="Select option" />
                        </SelectTrigger>
                        <SelectContent>
                          <SelectItem value="option-1">Option 1</SelectItem>
                          <SelectItem value="option-2">Option 2</SelectItem>
                        </SelectContent>
                      </Select>
                    </div>

                    <Textarea rows={3} placeholder="Textarea token surface" />

                    <Alert>
                      <Bell className="h-4 w-4" />
                      <AlertTitle>Alert Preview</AlertTitle>
                      <AlertDescription>Alert styles are rendered via token-based palette.</AlertDescription>
                    </Alert>
                  </CardContent>
                </Card>

                <div className="grid gap-4 md:grid-cols-2">
                  <Card className="border-border/70 bg-card">
                    <CardHeader>
                      <CardTitle>Toast Preview</CardTitle>
                      <CardDescription>Preview live toast from this sandbox.</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <Button
                        onClick={() => {
                          showToast({
                            title: "Preview Toast",
                            description: "Tokenized toast preview fired from sandbox.",
                            variant: "default",
                          })
                        }}
                      >
                        Show Toast
                      </Button>
                    </CardContent>
                  </Card>

                  <Card className="border-border/70 bg-card">
                    <CardHeader>
                      <CardTitle>Loader Preview</CardTitle>
                      <CardDescription>Preview tokenized loading indicator.</CardDescription>
                    </CardHeader>
                    <CardContent>
                      <div className="inline-flex items-center gap-3 rounded-md border border-border bg-muted/40 px-3 py-2">
                        <Loader2 className="h-4 w-4 animate-spin text-brand" />
                        <span className="text-sm text-muted-foreground">Loading content...</span>
                      </div>
                    </CardContent>
                  </Card>
                </div>

                <div className="grid gap-4 md:grid-cols-2">
                  <Card className="border-border/70 bg-card">
                    <CardHeader>
                      <CardTitle>Section Block</CardTitle>
                      <CardDescription>Section cards inherit theme tokens.</CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-2 text-sm text-muted-foreground">
                      <p>Reusable section shells adapt instantly when tokens change.</p>
                      <p>
                        Try adjusting <code>card</code>, <code>card-foreground</code>, and <code>border</code>.
                      </p>
                    </CardContent>
                  </Card>

                  <Card className="border-border/70 bg-card">
                    <CardHeader>
                      <CardTitle>Blog Card</CardTitle>
                      <CardDescription>Tokenized link, metadata, and CTA palette.</CardDescription>
                    </CardHeader>
                    <CardContent className="space-y-2 text-sm">
                      <p className="text-muted-foreground">February 27, 2026</p>
                      <p className="text-foreground">Building tenant-aware design systems with OKLCH tokens.</p>
                      <Button size="sm" className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
                        Read Article
                      </Button>
                    </CardContent>
                  </Card>
                </div>
              </div>
            </div>

            <footer className="border-t border-border/70 bg-footer-bg px-4 py-4 text-sm text-footer-foreground">
              <div className="flex flex-wrap items-center justify-between gap-3">
                <span>Preview footer surface</span>
                <Link to="/" className="text-link hover:text-link-hover">
                  Return to site
                </Link>
              </div>
            </footer>
          </div>
        </div>
      </div>
    </div>
  )
}

export default ThemePreviewPage
