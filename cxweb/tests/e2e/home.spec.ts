import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, assertNoDuplicateApiCalls, mockDefaultWebApis } from "../utils/network-utils"

test.describe("Home page", () => {
  test("loads successfully with navigation and footer", async ({ page, gotoHome, primaryHost }) => {
    const tracker = attachRuntimeTracker(page)
    await mockDefaultWebApis(page)

    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")

    await expect(page).toHaveURL(/\/?$/)
    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("header, nav").first()).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getFailedRequests()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
    assertNoDuplicateApiCalls(tracker.getApiCalls())
  })

  test("renders page when slider API fails", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/slider", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({ error: "forced" }),
      })
    })

    const tracker = attachRuntimeTracker(page, [/\/api\/slider$/])

    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")

    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("header, nav").first()).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()

    tracker.stop()
    const consoleErrors = tracker.getConsoleErrors().filter((message) => !message.includes("500 (Internal Server Error)"))
    const pageErrors = tracker.getPageErrors().filter((message) => !message.includes("Request failed with status 500"))
    expect(consoleErrors).toEqual([])
    expect(pageErrors).toEqual([])
    expect(tracker.getFailedRequests()).toEqual([])
  })

  test("remains stable when slider endpoint is slow", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/slider", async (route) => {
      await new Promise((resolve) => setTimeout(resolve, 2_500))
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          id: "slow-slider",
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
          slides: [],
        }),
      })
    })

    const tracker = attachRuntimeTracker(page)
    await gotoHome(page, primaryHost)

    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()

    await page.waitForLoadState("networkidle")
    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
  })
})
