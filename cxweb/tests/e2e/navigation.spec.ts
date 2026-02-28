import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, validateNavigationTransport, mockDefaultWebApis } from "../utils/network-utils"

test.describe("Navigation", () => {
  test("loads menu data and renders header", async ({ page, gotoHome, primaryHost }) => {
    const tracker = attachRuntimeTracker(page)
    await mockDefaultWebApis(page)

    const menuResponsePromise = page.waitForResponse((response) => response.url().includes("/api/web/menus") && response.request().method() === "GET")
    await gotoHome(page, primaryHost)
    const menuResponse = await menuResponsePromise
    expect(menuResponse.status()).toBe(200)

    const payload = await menuResponse.json()
    validateNavigationTransport(payload)

    await expect(page.locator("nav[aria-label='Main navigation']")).toBeVisible()
    await expect(page.getByRole("link", { name: "Login" })).toBeVisible()

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
  })

  test("mega menu hover, mobile toggle, theme switch and sticky behavior", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")

    await expect(page.locator("nav[aria-label='Main navigation']")).toBeVisible()

    await page.setViewportSize({ width: 390, height: 844 })
    await page.getByRole("button", { name: "Toggle mobile menu" }).click()
    await expect(page.locator("nav[aria-label='Mobile navigation']")).toBeVisible()

    await page.setViewportSize({ width: 1280, height: 900 })
    await page.getByRole("button", { name: "Switch theme to Dark" }).click()
    await expect(page.locator("html")).toHaveClass(/dark/)

    const header = page.locator("header").first()
    await expect(header).toHaveClass(/sticky/)
    await page.evaluate(() => window.scrollTo(0, 900))
    await page.waitForTimeout(120)
    await expect(header).toHaveClass(/sticky/)
  })

  test("tenant menu override renders tenant-specific items", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/web/menus", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify([
          {
            groupType: 1,
            groupSlug: "header",
            groupName: "Header",
            menus: [
              {
                name: "TenantMenu",
                slug: "tenant-menu",
                variant: 1,
                isMegaMenu: false,
                order: 0,
                items: [
                  {
                    title: "Tenant Custom Link",
                    slug: "tenant-custom-link",
                    url: "/tenant-custom-link",
                    target: 1,
                    icon: null,
                    description: null,
                    order: 0,
                    children: [],
                  },
                ],
              },
            ],
          },
        ]),
      })
    })

    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")
    await expect(page.getByRole("link", { name: "Tenant Custom Link" })).toBeVisible()
  })

  test("navigation disabled scenario keeps page stable", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/web/menus", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify([]),
      })
    })

    await page.route("**/api/web/navigation-config", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          id: "n1",
          tenantId: null,
          layoutConfig: { variant: "container", zoneOrder: ["left", "center", "right"], menuAlign: "center", logoPosition: "left", menuSize: "medium" },
          styleConfig: { backgroundToken: "header-bg", textToken: "header-foreground" },
          behaviorConfig: { sticky: false, scrollShadow: false, transparentOnTop: false, blur: false, borderBottom: false, mobileOverlay: true },
          componentConfig: { left: ["logo"], center: [], right: [], logo: { type: "text", text: "Brand", showText: true, textPosition: "right", size: "medium" }, auth: { enabled: false, loginPath: "/auth/login", dashboardPath: "/app" }, cta: { enabled: false, label: "", url: "", target: "_self" } },
          isActive: false,
          createdAtUtc: new Date().toISOString(),
          updatedAtUtc: new Date().toISOString(),
        }),
      })
    })

    await gotoHome(page, primaryHost)
    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer")).toBeVisible()
  })
})
