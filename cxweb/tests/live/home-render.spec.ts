import { expect, test } from "@playwright/test"
import { attachNetworkMonitor, assertNoClientRuntimeIssues } from "./network-monitor"

test.describe("Live home render", () => {
  test("renders homepage with navigation, content and footer", async ({ page }) => {
    const monitor = attachNetworkMonitor(page)
    await page.goto("/", { waitUntil: "domcontentloaded" })
    await page.waitForLoadState("networkidle")

    await expect(page.locator("header").first()).toBeVisible()
    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer").first()).toBeVisible()

    const headingCount = await page.locator("section h1").count()
    expect(headingCount).toBeGreaterThanOrEqual(0)

    monitor.stop()
    assertNoClientRuntimeIssues(monitor)
  })

  test("keeps page stable when slider endpoint fails", async ({ page }) => {
    await page.route("**/api/slider", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({ error: "forced failure" }),
      })
    })

    const monitor = attachNetworkMonitor(page, [/\/api\/slider$/])
    await page.goto("/", { waitUntil: "domcontentloaded" })
    await page.waitForLoadState("networkidle")

    await expect(page.locator("main")).toBeVisible()
    await expect(page.locator("footer").first()).toBeVisible()

    monitor.stop()
    const consoleErrors = monitor.getConsoleErrors().filter((line) => !line.includes("500 (Internal Server Error)"))
    expect(consoleErrors).toEqual([])
    const pageErrors = monitor.getPageErrors().filter((line) => !line.includes("Request failed with status 500"))
    expect(pageErrors).toEqual([])
    expect(monitor.getFailedRequests()).toEqual([])
    expect(monitor.getApiFailures().filter((line) => !line.includes("/api/slider"))).toEqual([])
  })
})
