import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, mockDefaultWebApis, validateSliderTransport } from "../utils/network-utils"

type MockSlider = {
  id: string
  isActive: boolean
  slides: Array<{
    id: string
    order: number
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
    highlights: Array<{ id: string; text: string; variant: string; order: number }>
  }>
  heightMode: number
  heightValue: number
  containerMode: number
  contentAlignment: number
  autoplay: boolean
  loop: boolean
  showProgress: boolean
  showNavArrows: boolean
  showDots: boolean
  parallax: boolean
  particles: boolean
  defaultVariant: number
  defaultIntensity: number
  defaultDirection: number
  defaultBackgroundMode: number
  scrollBehavior: number
}

const buildMockSlider = (overrides?: Partial<MockSlider>): MockSlider => ({
  id: "slider-mock",
  isActive: true,
  slides: [
    {
      id: "slide-1",
      order: 0,
      title: "Slide One",
      tagline: "Tagline One",
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
      highlights: [{ id: "h1", text: "A", variant: "primary", order: 0 }],
    },
    {
      id: "slide-2",
      order: 1,
      title: "Slide Two",
      tagline: "Tagline Two",
      actionText: "Explore",
      actionHref: "/products",
      ctaColor: 1,
      duration: 1200,
      direction: 1,
      variant: 0,
      intensity: 1,
      backgroundMode: 0,
      showOverlay: true,
      overlayToken: "muted/70",
      backgroundUrl: "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=1920",
      mediaType: 0,
      youtubeVideoId: null,
      isActive: true,
      layers: [],
      highlights: [{ id: "h2", text: "B", variant: "success", order: 0 }],
    },
  ],
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
  ...overrides,
})

test.describe("Slider", () => {
  test("renders from API and matches transport schema", async ({ page, gotoHome, primaryHost }) => {
    const tracker = attachRuntimeTracker(page)
    await mockDefaultWebApis(page)

    const sliderResponsePromise = page.waitForResponse((response) => response.url().includes("/api/slider") && response.request().method() === "GET")
    await gotoHome(page, primaryHost)
    const sliderResponse = await sliderResponsePromise

    expect(sliderResponse.status()).toBe(200)
    const sliderPayload = validateSliderTransport(await sliderResponse.json())

    await expect(page.locator("section h1").first()).toBeVisible()

    const activeSlides = sliderPayload.slides.filter((slide) => slide.isActive)
    const dots = page.locator("button[aria-label^='Go to slide']")
    if (sliderPayload.showDots && activeSlides.length > 1) {
      await expect(dots).toHaveCount(activeSlides.length)
    }

    const first = activeSlides[0]
    if (first?.actionText) {
      await expect(page.getByRole("button", { name: first.actionText })).toBeVisible()
    }

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getFailedRequests()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
  })

  test("auto advances and controls work", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/slider", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(buildMockSlider()),
      })
    })

    await gotoHome(page, primaryHost)
    await expect(page.getByRole("heading", { name: "Slide One" })).toBeVisible()

    await page.getByRole("button", { name: "Next slide" }).click()
    await expect(page.getByRole("heading", { name: "Slide Two" })).toBeVisible()

    await page.getByRole("button", { name: "Previous slide" }).click()
    await expect(page.getByRole("heading", { name: "Slide One" })).toBeVisible()

    await page.getByRole("button", { name: "Go to slide 2" }).click()
    await expect(page.getByRole("heading", { name: "Slide Two" })).toBeVisible()

    await page.waitForTimeout(1700)
    await expect(page.getByRole("heading", { name: "Slide One" })).toBeVisible()

    await expect(page.locator("div.h-1.bg-muted\\/70")).toBeVisible()
  })

  test("shows skeleton before media and handles disabled slider", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/slider", async (route) => {
      await new Promise((resolve) => setTimeout(resolve, 1400))
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(buildMockSlider({ isActive: false })),
      })
    })

    await gotoHome(page, primaryHost)
    await expect(page.locator("div.animate-pulse").first()).toBeVisible()
    await page.waitForLoadState("networkidle")
    await expect(page.locator("section h1").first()).toHaveCount(0)
  })

  test("handles empty, null and invalid slider payloads without crashing", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/slider", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(buildMockSlider({ slides: [] })),
      })
    })

    await gotoHome(page, primaryHost)
    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()

    await page.unroute("**/api/slider")
    await page.route("**/api/slider", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: "null",
      })
    })

    await page.reload({ waitUntil: "domcontentloaded" })
    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()

    await page.unroute("**/api/slider")
    await page.route("**/api/slider", async (route) => {
      const invalid = buildMockSlider()
      invalid.slides[0] = { ...invalid.slides[0], direction: 9999 }
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(invalid),
      })
    })

    await page.reload({ waitUntil: "domcontentloaded" })
    await expect(page.locator("main")).toBeVisible()
  })
})
