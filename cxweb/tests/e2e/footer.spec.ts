import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, validateFooterTransport, mockDefaultWebApis } from "../utils/network-utils"

test.describe("Footer", () => {
  test("renders footer sections and validates backend transport", async ({ page, gotoHome, primaryHost }) => {
    const tracker = attachRuntimeTracker(page)
    await mockDefaultWebApis(page)
    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")

    await expect(page.locator("footer")).toBeVisible()
    await expect(page.getByRole("heading", { name: "Social" })).toBeVisible()
    await expect(page.getByRole("heading", { name: "Business Hours" })).toBeVisible()
    await expect(page.getByRole("heading", { name: "Payments" })).toBeVisible()
    await expect(page.getByPlaceholder("Email address")).toBeVisible()
    await expect(page.getByRole("button", { name: "Join" })).toBeVisible()

    const year = new Date().getFullYear().toString()
    await expect(page.locator("footer")).toContainText(year)

    validateFooterTransport({
      id: "foot1",
    })

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
  })

  test("footer disabled/empty scenario does not crash page", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/web/footer-config", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          id: "footer-disabled",
          tenantId: null,
          layoutConfig: { variant: "container", columns: 1, sectionOrder: ["bottom"] },
          styleConfig: { backgroundToken: "footer-bg", textToken: "foreground", linkToken: "link", linkHoverToken: "primary", borderTop: false, spacing: "compact", columnGap: "compact" },
          behaviorConfig: { showDynamicYear: false, showNewsletter: false, showPayments: false },
          componentConfig: { about: { enabled: false, title: "", content: "" }, links: { enabled: false, menuGroupSlug: "footer" }, legal: { enabled: false, items: [] }, social: { enabled: false, items: [] }, newsletter: { enabled: false, title: "", description: "" }, businessHours: { enabled: false, items: [] }, payments: { enabled: false, providers: [] }, bottom: { enabled: false, copyright: "", developedBy: { enabled: false, label: "", url: "" } } },
          isActive: false,
          createdAtUtc: new Date().toISOString(),
          updatedAtUtc: new Date().toISOString(),
        }),
      })
    })

    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")
    await expect(page.locator("main")).toBeVisible()
  })
})
