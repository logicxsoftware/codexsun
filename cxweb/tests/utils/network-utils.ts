import { expect, type APIRequestContext, type Page, type Request, type Response } from "@playwright/test"

export type ConsoleAndNetworkTracker = {
  getConsoleErrors: () => string[]
  getPageErrors: () => string[]
  getFailedRequests: () => string[]
  getApiFailures: () => string[]
  getApiCalls: () => string[]
  stop: () => void
}

export type SliderResponse = {
  id: string
  isActive: boolean
  slides: Array<{
    id: string
    title: string
    tagline: string
    actionText: string | null
    actionHref: string | null
    ctaColor: number
    duration: number
    direction: number
    variant: number
    intensity: number
    backgroundMode: number
    showOverlay: boolean
    overlayToken: string
    backgroundUrl: string
    mediaType: number
    youtubeVideoId: string | null
    isActive: boolean
    layers: Array<{
      id: string
      order: number
      type: number
      content: string
      mediaUrl: string | null
      positionX: number
      positionY: number
      width: string
      animationFrom: number
      animationDelay: number
      animationDuration: number
      animationEasing: string
      responsiveVisibility: string
    }>
    highlights: Array<{
      id: string
      text: string
      variant: string
      order: number
    }>
  }>
  autoplay: boolean
  loop: boolean
  showProgress: boolean
  showNavArrows: boolean
  showDots: boolean
}

export const endpointCandidates = {
  homeData: ["/api/home-data"],
  slider: ["/api/slider"],
  navigation: ["/api/navigation", "/api/web/navigation-config", "/api/web/menus"],
  footer: ["/api/footer", "/api/web/footer-config"],
} as const

const isObject = (value: unknown): value is Record<string, unknown> => typeof value === "object" && value !== null && !Array.isArray(value)

const hasNumber = (value: Record<string, unknown>, key: string): boolean => typeof value[key] === "number"
const hasString = (value: Record<string, unknown>, key: string): boolean => typeof value[key] === "string"
const hasBoolean = (value: Record<string, unknown>, key: string): boolean => typeof value[key] === "boolean"
const hasArray = (value: Record<string, unknown>, key: string): boolean => Array.isArray(value[key])

export const attachRuntimeTracker = (page: Page, allowedFailurePatterns: RegExp[] = []): ConsoleAndNetworkTracker => {
  const consoleErrors: string[] = []
  const pageErrors: string[] = []
  const failedRequests: string[] = []
  const apiFailures: string[] = []
  const apiCalls: string[] = []

  const onConsole = (message: { type: () => string; text: () => string }) => {
    if (message.type() === "error") {
      consoleErrors.push(message.text())
    }
  }

  const onPageError = (error: Error) => {
    pageErrors.push(error.message)
  }

  const onRequestFailed = (request: Request) => {
    const errorText = request.failure()?.errorText ?? "failed"
    if (errorText.includes("NS_BINDING_ABORTED") || errorText.includes("net::ERR_ABORTED")) {
      return
    }
    failedRequests.push(`${request.method()} ${request.url()} ${errorText}`)
  }

  const onResponse = (response: Response) => {
    const url = response.url()
    if (!url.includes("/api/")) {
      return
    }

    apiCalls.push(`${response.request().method()} ${url}`)
    if (response.status() >= 400) {
      const allow = allowedFailurePatterns.some((pattern) => pattern.test(url))
      if (!allow) {
        apiFailures.push(`${response.status()} ${response.request().method()} ${url}`)
      }
    }
  }

  page.on("console", onConsole)
  page.on("pageerror", onPageError)
  page.on("requestfailed", onRequestFailed)
  page.on("response", onResponse)

  return {
    getConsoleErrors: () => [...consoleErrors],
    getPageErrors: () => [...pageErrors],
    getFailedRequests: () => [...failedRequests],
    getApiFailures: () => [...apiFailures],
    getApiCalls: () => [...apiCalls],
    stop: () => {
      page.off("console", onConsole)
      page.off("pageerror", onPageError)
      page.off("requestfailed", onRequestFailed)
      page.off("response", onResponse)
    },
  }
}

export const assertNoRuntimeErrors = async (page: Page, allowedFailurePatterns: RegExp[] = []): Promise<void> => {
  const tracker = attachRuntimeTracker(page, allowedFailurePatterns)
  await page.waitForTimeout(50)
  tracker.stop()
  expect(tracker.getConsoleErrors(), `Console errors:\n${tracker.getConsoleErrors().join("\n")}`).toEqual([])
  expect(tracker.getPageErrors(), `Page errors:\n${tracker.getPageErrors().join("\n")}`).toEqual([])
  expect(tracker.getFailedRequests(), `Failed requests:\n${tracker.getFailedRequests().join("\n")}`).toEqual([])
  expect(tracker.getApiFailures(), `API failures:\n${tracker.getApiFailures().join("\n")}`).toEqual([])
}

export const getFirstOkJson = async (request: APIRequestContext, candidates: readonly string[], timeout = 10_000): Promise<{ url: string; status: number; json: unknown }> => {
  let lastStatus = 0
  for (const endpoint of candidates) {
    const response = await request.get(endpoint, { timeout })
    lastStatus = response.status()
    if (!response.ok()) {
      continue
    }

    const contentType = response.headers()["content-type"] ?? ""
    if (!contentType.includes("application/json")) {
      continue
    }

    const json = await response.json()
    return { url: endpoint, status: response.status(), json }
  }

  throw new Error(`No successful JSON response from candidates: ${candidates.join(", ")}. Last status: ${lastStatus}`)
}

export const validateSliderTransport = (payload: unknown): SliderResponse => {
  if (!isObject(payload)) {
    throw new Error("Slider response is not an object")
  }

  const requiredBooleanKeys = ["isActive", "autoplay", "loop", "showProgress", "showNavArrows", "showDots"] as const
  for (const key of requiredBooleanKeys) {
    if (!hasBoolean(payload, key)) {
      throw new Error(`Slider field ${key} must be boolean`)
    }
  }

  if (!hasString(payload, "id")) {
    throw new Error("Slider field id must be string")
  }

  if (!hasArray(payload, "slides")) {
    throw new Error("Slider field slides must be array")
  }

  const slides = payload.slides as unknown[]
  for (const slide of slides) {
    if (!isObject(slide)) {
      throw new Error("Slide must be object")
    }

    const numericFields = ["ctaColor", "duration", "direction", "variant", "intensity", "backgroundMode", "mediaType"] as const
    for (const field of numericFields) {
      if (!hasNumber(slide, field)) {
        throw new Error(`Slide field ${field} must be number`)
      }
    }

    if (!hasString(slide, "id") || !hasString(slide, "title") || !hasString(slide, "tagline") || !hasString(slide, "overlayToken") || !hasString(slide, "backgroundUrl")) {
      throw new Error("Slide string fields are invalid")
    }

    if (!hasArray(slide, "layers") || !hasArray(slide, "highlights")) {
      throw new Error("Slide array fields are invalid")
    }
  }

  return payload as SliderResponse
}

export const validateNavigationTransport = (payload: unknown): void => {
  if (Array.isArray(payload)) {
    for (const group of payload) {
      if (!isObject(group)) {
        throw new Error("Navigation group must be object")
      }

      if (!hasArray(group, "menus")) {
        throw new Error("Navigation group menus must be array")
      }
    }

    return
  }

  if (isObject(payload)) {
    if (!hasString(payload, "id") && !hasString(payload, "groupSlug")) {
      throw new Error("Navigation payload object missing required keys")
    }

    return
  }

  throw new Error("Navigation payload invalid")
}

export const validateFooterTransport = (payload: unknown): void => {
  if (!isObject(payload)) {
    throw new Error("Footer payload must be object")
  }

  if (!hasString(payload, "id")) {
    throw new Error("Footer payload missing id")
  }
}

export const assertNoDuplicateApiCalls = (apiCalls: string[]): void => {
  const apiOnly = apiCalls.filter((value) => value.includes("/api/"))
  const counters = new Map<string, number>()
  for (const call of apiOnly) {
    counters.set(call, (counters.get(call) ?? 0) + 1)
  }

  const duplicates = [...counters.entries()].filter(([, count]) => count > 2)
  expect(duplicates, `Duplicate API calls: ${JSON.stringify(duplicates)}`).toEqual([])
}

type MockOptions = {
  slider?: unknown
  menus?: unknown
  navigationConfig?: unknown
  footerConfig?: unknown
  webPage?: unknown
  theme?: unknown
  tenantCurrent?: unknown
}

const defaultSlider = {
  id: "slider-default",
  tenantId: null,
  isActive: true,
  heightMode: 0,
  heightValue: 100,
  containerMode: 1,
  contentAlignment: 0,
  autoplay: true,
  loop: true,
  showProgress: true,
  showNavArrows: true,
  showDots: true,
  parallax: false,
  particles: false,
  defaultVariant: 0,
  defaultIntensity: 1,
  defaultDirection: 0,
  defaultBackgroundMode: 0,
  scrollBehavior: 0,
  slides: [
    {
      id: "s1",
      order: 0,
      title: "Build Smarter SaaS Products",
      tagline: "Scalable systems for growth",
      actionText: "Get Started",
      actionHref: "/signup",
      ctaColor: 0,
      duration: 1200,
      direction: 0,
      variant: 0,
      intensity: 1,
      backgroundMode: 0,
      showOverlay: true,
      overlayToken: "muted/70",
      backgroundUrl: "https://images.unsplash.com/photo-1551434678-e076c223a692?w=1920",
      mediaType: 0,
      youtubeVideoId: null,
      isActive: true,
      layers: [],
      highlights: [{ id: "h1", text: "Cloud Scalable", variant: "primary", order: 0 }],
    },
    {
      id: "s2",
      order: 1,
      title: "Codexsun CRM",
      tagline: "Close deals faster",
      actionText: "Explore CRM",
      actionHref: "/products/crm",
      ctaColor: 1,
      duration: 1200,
      direction: 1,
      variant: 0,
      intensity: 1,
      backgroundMode: 0,
      showOverlay: true,
      overlayToken: "muted/70",
      backgroundUrl: "https://images.unsplash.com/photo-1556740749-887f6717d7e4?w=1920",
      mediaType: 0,
      youtubeVideoId: null,
      isActive: true,
      layers: [],
      highlights: [{ id: "h2", text: "Automation", variant: "success", order: 0 }],
    },
  ],
}

const defaultMenus = [
  {
    groupType: 1,
    groupSlug: "header",
    groupName: "Header",
    menus: [
      {
        name: "Primary",
        slug: "primary",
        variant: 1,
        isMegaMenu: false,
        order: 0,
        items: [
          { title: "Home", slug: "home", url: "/", target: 1, icon: null, description: null, order: 0, children: [] },
          { title: "About", slug: "about", url: "/about", target: 1, icon: null, description: null, order: 1, children: [] },
          { title: "Blog", slug: "blog", url: "/blog", target: 1, icon: null, description: null, order: 2, children: [] },
        ],
      },
    ],
  },
  {
    groupType: 2,
    groupSlug: "footer",
    groupName: "Footer",
    menus: [
      {
        name: "FooterLinks",
        slug: "footer-links",
        variant: 1,
        isMegaMenu: false,
        order: 0,
        items: [
          { title: "Privacy", slug: "privacy", url: "/privacy-policy", target: 1, icon: null, description: null, order: 0, children: [] },
          { title: "Terms", slug: "terms", url: "/terms", target: 1, icon: null, description: null, order: 1, children: [] },
        ],
      },
    ],
  },
  {
    groupType: 3,
    groupSlug: "mobile",
    groupName: "Mobile",
    menus: [
      {
        name: "Mobile",
        slug: "mobile",
        variant: 2,
        isMegaMenu: false,
        order: 0,
        items: [
          { title: "Home", slug: "home-mobile", url: "/", target: 1, icon: null, description: null, order: 0, children: [] },
          { title: "About", slug: "about-mobile", url: "/about", target: 1, icon: null, description: null, order: 1, children: [] },
        ],
      },
    ],
  },
]

const defaultNavigationConfig = {
  id: "nav1",
  tenantId: null,
  layoutConfig: { variant: "container", zoneOrder: ["left", "center", "right"], menuAlign: "center", logoPosition: "left", menuSize: "medium" },
  styleConfig: { backgroundToken: "header-bg", textToken: "header-foreground", hoverToken: "menu-hover", activeToken: "primary", dropdownToken: "card", borderToken: "border", scrollBackgroundToken: "header-bg", scrollTextToken: "header-foreground" },
  behaviorConfig: { sticky: true, scrollShadow: true, transparentOnTop: true, blur: true, borderBottom: true, mobileOverlay: true },
  componentConfig: {
    left: ["logo"],
    center: ["menu"],
    right: ["themeSwitch", "auth"],
    logo: { type: "text", text: "Codexsun", showText: true, textPosition: "right", size: "medium" },
    auth: { enabled: true, loginPath: "/auth/login", dashboardPath: "/app" },
    cta: { enabled: true, label: "Get Started", url: "/signup", target: "_self" },
  },
  isActive: true,
  createdAtUtc: new Date().toISOString(),
  updatedAtUtc: new Date().toISOString(),
}

const defaultFooterConfig = {
  id: "foot1",
  tenantId: null,
  layoutConfig: { variant: "container", columns: 4, sectionOrder: ["about", "links", "legal", "social", "businessHours", "newsletter", "payments", "bottom"] },
  styleConfig: { backgroundToken: "footer-bg", textToken: "foreground", linkToken: "link", linkHoverToken: "primary", borderTop: true, spacing: "normal", columnGap: "normal" },
  behaviorConfig: { showDynamicYear: true, showNewsletter: true, showPayments: true },
  componentConfig: {
    about: { enabled: true, title: "Codexsun", content: "Modern SaaS software company" },
    links: { enabled: true, menuGroupSlug: "footer" },
    legal: { enabled: true, items: [{ label: "Privacy Policy", url: "/privacy-policy", target: "_self" }] },
    social: { enabled: true, items: [{ icon: "github", label: "GitHub", url: "https://github.com/codexsun", target: "_blank" }] },
    newsletter: { enabled: true, title: "Stay Updated", description: "Subscribe for updates" },
    businessHours: { enabled: true, items: [{ day: "Monday - Friday", hours: "9:00 AM - 6:00 PM" }] },
    payments: { enabled: true, providers: ["Stripe", "PayPal"] },
    bottom: { enabled: true, copyright: "© {YEAR} Codexsun. All rights reserved.", developedBy: { enabled: true, label: "Codexsun Engineering Team", url: "https://codexsun.com" } },
  },
  isActive: true,
  createdAtUtc: new Date().toISOString(),
  updatedAtUtc: new Date().toISOString(),
}

const defaultWebPage = {
  slug: "home",
  title: "Home",
  seoTitle: "Home",
  seoDescription: "Home page",
  updatedAtUtc: new Date().toISOString(),
  sections: [],
}

const defaultTenantCurrent = {
  tenantId: "00000000-0000-0000-0000-000000000001",
  tenantName: "Codexsun",
  domain: "localhost",
}

export const mockDefaultWebApis = async (page: Page, options: MockOptions = {}): Promise<void> => {
  await page.route("**/api/slider", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.slider ?? defaultSlider),
    })
  })

  await page.route("**/api/web/menus", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.menus ?? defaultMenus),
    })
  })

  await page.route("**/api/web/navigation-config", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.navigationConfig ?? defaultNavigationConfig),
    })
  })

  await page.route("**/api/web/footer-config", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.footerConfig ?? defaultFooterConfig),
    })
  })

  await page.route("**/api/web/theme", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.theme ?? {}),
    })
  })

  await page.route("**/api/web/home", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.webPage ?? defaultWebPage),
    })
  })

  await page.route("**/api/tenant/current", async (route) => {
    await route.fulfill({
      status: 200,
      contentType: "application/json",
      body: JSON.stringify(options.tenantCurrent ?? defaultTenantCurrent),
    })
  })
}
