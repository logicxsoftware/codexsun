import { test, expect } from "../fixtures/tenant.fixture"
import { assertNoDuplicateApiCalls, attachRuntimeTracker, mockDefaultWebApis } from "../utils/network-utils"

test.describe("Console and network errors", () => {
  test("has no console errors, page errors, request failures or API 4xx/5xx", async ({ page, gotoHome, primaryHost }) => {
    const tracker = attachRuntimeTracker(page)
    await mockDefaultWebApis(page)

    const startedAt = Date.now()
    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")
    const elapsed = Date.now() - startedAt

    await expect(page.locator("main")).toBeVisible()

    tracker.stop()
    expect(elapsed).toBeLessThan(12_000)
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getFailedRequests()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
    assertNoDuplicateApiCalls(tracker.getApiCalls())
  })

  test("allows mocked failing endpoint while remaining stable", async ({ page, gotoHome, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/web/footer-config", async (route) => {
      await route.fulfill({
        status: 500,
        contentType: "application/json",
        body: JSON.stringify({ error: "mocked footer failure" }),
      })
    })

    const tracker = attachRuntimeTracker(page, [/\/api\/web\/footer-config$/])
    await gotoHome(page, primaryHost)
    await page.waitForLoadState("networkidle")

    await expect(page.locator("main")).toBeVisible()
    tracker.stop()
    const consoleErrors = tracker.getConsoleErrors().filter((message) => !message.includes("500 (Internal Server Error)"))
    const failedRequests = tracker.getFailedRequests().filter((request) => !request.includes("NS_BINDING_ABORTED"))
    expect(consoleErrors).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(failedRequests).toEqual([])
  })
})
