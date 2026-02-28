import { expect, test } from "@playwright/test"
import { attachNetworkMonitor, assertNoClientRuntimeIssues, assertNoDuplicateApiCalls } from "./network-monitor"

test.describe("Live console and network", () => {
  test("has no runtime errors and no repeated API storms", async ({ page }) => {
    const monitor = attachNetworkMonitor(page)
    const started = Date.now()

    await page.goto("/", { waitUntil: "domcontentloaded" })
    await page.waitForLoadState("networkidle")

    const loadMs = Date.now() - started
    expect(loadMs).toBeLessThan(15_000)
    await expect(page.locator("main")).toBeVisible()

    monitor.stop()
    assertNoClientRuntimeIssues(monitor)
    assertNoDuplicateApiCalls(monitor, 3)
  })
})
