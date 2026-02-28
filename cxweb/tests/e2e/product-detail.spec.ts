import { test, expect } from "../fixtures/tenant.fixture"
import { attachRuntimeTracker, mockDefaultWebApis } from "../utils/network-utils"

test.describe("Product detail page", () => {
  test("loads product detail by slug and renders attributes", async ({ page, primaryHost }) => {
    await mockDefaultWebApis(page)

    await page.route("**/api/products/dell-latitude-5440", async (route) => {
      await route.fulfill({
        status: 200,
        contentType: "application/json",
        body: JSON.stringify({
          product: {
            id: "p1",
            name: "Dell Latitude 5440",
            slug: "dell-latitude-5440",
            description: "Business laptop with long battery life.",
            shortDescription: "Business-ready laptop",
            price: 74999,
            comparePrice: 81999,
            inStock: true,
            categoryName: "Laptops",
            categorySlug: "laptops",
            images: [
              "https://images.unsplash.com/photo-1496181133206-80ce9b88a853",
              "https://images.unsplash.com/photo-1517336714731-489689fd1ca8",
            ],
            specifications: {
              brand: "Dell",
              ram: "16GB",
            },
            sku: "LAP-001",
          },
          relatedProducts: [
            { id: "p2", name: "HP ProBook 440", slug: "hp-probook-440", price: 68999, comparePrice: 73999, imageUrl: "https://images.unsplash.com/photo-1517336714731-489689fd1ca8" },
          ],
        }),
      })
    })

    const tracker = attachRuntimeTracker(page)
    const url = new URL("http://localhost:7043/products/dell-latitude-5440")
    url.hostname = primaryHost
    await page.goto(url.toString(), { waitUntil: "domcontentloaded" })

    await expect(page.getByRole("heading", { name: "Dell Latitude 5440" })).toBeVisible()
    await expect(page.getByText("Brand:")).toBeVisible()
    await expect(page.getByText("Dell", { exact: true })).toBeVisible()
    await expect(page.getByRole("heading", { name: "Related Products" })).toBeVisible()

    tracker.stop()
    expect(tracker.getConsoleErrors()).toEqual([])
    expect(tracker.getPageErrors()).toEqual([])
    expect(tracker.getApiFailures()).toEqual([])
  })
})
