import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, mockDefaultWebApis } from "../utils/network-utils"

const buildProductsPayload = (url: URL) => {
  const page = Number(url.searchParams.get("page") ?? "1")
  const pageSize = Number(url.searchParams.get("pageSize") ?? "12")
  const search = (url.searchParams.get("search") ?? "").toLowerCase()

  const source = [
    { id: "p1", name: "Dell Latitude 5440", slug: "dell-latitude-5440", shortDescription: "Business laptop", price: 74999, comparePrice: 81999, stockQuantity: 10, categoryName: "Laptops", categorySlug: "laptops", imageUrl: "https://images.unsplash.com/photo-1496181133206-80ce9b88a853" },
    { id: "p2", name: "HP ProBook 440", slug: "hp-probook-440", shortDescription: "Corporate laptop", price: 68999, comparePrice: 73999, stockQuantity: 12, categoryName: "Laptops", categorySlug: "laptops", imageUrl: "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
    { id: "p3", name: "LG UltraGear 27", slug: "lg-ultragear-27", shortDescription: "Gaming monitor", price: 25999, comparePrice: 29999, stockQuantity: 8, categoryName: "Monitors", categorySlug: "monitors", imageUrl: "https://images.unsplash.com/photo-1527443224154-c4a3942d3acf" },
  ]

  const filtered = source.filter((item) => item.name.toLowerCase().includes(search))
  const totalItems = filtered.length
  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize))
  const safePage = Math.min(Math.max(page, 1), totalPages)
  const start = (safePage - 1) * pageSize
  const data = filtered.slice(start, start + pageSize)

  return {
    data,
    pagination: {
      page: safePage,
      pageSize,
      totalItems,
      totalPages,
      hasPrevious: safePage > 1,
      hasNext: safePage < totalPages,
    },
    filters: {
      categories: [
        { name: "Laptops", slug: "laptops", count: 2 },
        { name: "Monitors", slug: "monitors", count: 1 },
      ],
      attributes: [
        {
          key: "brand",
          options: [
            { value: "Dell", count: 1 },
            { value: "HP", count: 1 },
            { value: "LG", count: 1 },
          ],
        },
      ],
      priceRange: { min: 25999, max: 74999 },
    },
  }
}

test.describe("Products page", () => {
  test("syncs search filters into URL and requests backend pagination", async ({ page, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/products**", async (route) => {
      const payload = buildProductsPayload(new URL(route.request().url()))
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(payload),
      })
    })

    const tracker = attachRuntimeTracker(page)
    const url = new URL("http://localhost:7043/products")
    url.hostname = primaryHost
    await page.goto(url.toString(), { waitUntil: "domcontentloaded" })

    await expect(page.getByRole("heading", { name: "Products" })).toBeVisible()
    await page.getByLabel("Search").fill("Dell")
    await page.waitForTimeout(350)

    await expect(page).toHaveURL(/search=Dell/)
    await expect(page.locator("article").first()).toContainText("Dell Latitude 5440")

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
  })

  test("toggles grid and list views", async ({ page, primaryHost }) => {
    await mockDefaultWebApis(page)
    await page.route("**/api/products**", async (route) => {
      const payload = buildProductsPayload(new URL(route.request().url()))
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify(payload),
      })
    })

    const url = new URL("http://localhost:7043/products")
    url.hostname = primaryHost
    await page.goto(url.toString(), { waitUntil: "domcontentloaded" })

    await page.setViewportSize({ width: 1280, height: 900 })
    await page.getByRole("button", { name: "List" }).click()
    await expect(page.locator("article").first()).toHaveClass(/flex/)

    await page.getByRole("button", { name: "Grid" }).click()
    await expect(page.locator("article").first()).not.toHaveClass(/flex/)
  })
})
