import { expect, test } from "@playwright/test"
import { liveBackendUrl, livePrimaryDomain, tenantHeaders } from "./live-config"

const assertObject = (value: unknown): asserts value is Record<string, unknown> => {
  expect(typeof value).toBe("object")
  expect(value).toBeTruthy()
  expect(Array.isArray(value)).toBeFalsy()
}

const assertSliderShape = (payload: unknown): void => {
  assertObject(payload)
  expect(typeof payload.id).toBe("string")
  expect(typeof payload.isActive).toBe("boolean")
  expect(Array.isArray(payload.slides)).toBeTruthy()
}

const assertNavigationShape = (payload: unknown): void => {
  expect(Array.isArray(payload)).toBeTruthy()
}

const assertFooterShape = (payload: unknown): void => {
  assertObject(payload)
  expect(typeof payload.id).toBe("string")
}

test.describe("Live transport", () => {
  test("validates backend transport endpoints on real server", async ({ request }) => {
    const started = Date.now()
    const homeDataResponse = await request.get(`${liveBackendUrl}/api/home-data`, { headers: tenantHeaders(livePrimaryDomain) })
    const homeDataDuration = Date.now() - started
    expect(homeDataDuration).toBeLessThan(10_000)

    const sliderResponse = await request.get(`${liveBackendUrl}/api/slider`, { headers: tenantHeaders(livePrimaryDomain) })
    expect(sliderResponse.status()).toBe(200)
    assertSliderShape(await sliderResponse.json())

    const navigationResponse = await request.get(`${liveBackendUrl}/api/web/menus`, { headers: tenantHeaders(livePrimaryDomain) })
    expect(navigationResponse.status()).toBe(200)
    assertNavigationShape(await navigationResponse.json())

    const footerResponse = await request.get(`${liveBackendUrl}/api/web/footer-config`, { headers: tenantHeaders(livePrimaryDomain) })
    expect(footerResponse.status()).toBe(200)
    assertFooterShape(await footerResponse.json())

    if (homeDataResponse.ok()) {
      const homeData = await homeDataResponse.json()
      assertObject(homeData)
      expect(homeData).toHaveProperty("slider")
      expect(homeData).toHaveProperty("navigation")
      expect(homeData).toHaveProperty("footer")
    } else {
      expect(homeDataResponse.status()).toBe(404)
    }
  })
})
