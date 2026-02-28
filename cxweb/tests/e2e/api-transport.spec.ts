import { test, expect } from "../fixtures/tenant.fixture"
import { endpointCandidates, getFirstOkJson, validateFooterTransport, validateNavigationTransport, validateSliderTransport } from "../utils/network-utils"

const containsPotentialHtmlInjection = (value: unknown): boolean => {
  if (typeof value === "string") {
    const lower = value.toLowerCase()
    return lower.includes("<script") || lower.includes("javascript:")
  }

  if (Array.isArray(value)) {
    return value.some(containsPotentialHtmlInjection)
  }

  if (typeof value === "object" && value !== null) {
    return Object.values(value).some(containsPotentialHtmlInjection)
  }

  return false
}

test.describe("API transport", () => {
  test("validates home-data, slider, navigation, footer transports", async ({ createApiForHost, primaryHost }) => {
    const api = await createApiForHost(primaryHost)
    let tenantProbeOk = false
    try {
      const probe = await api.get("/api/tenant/current", { timeout: 5_000 })
      tenantProbeOk = probe.ok()
    } catch {
      tenantProbeOk = false
    }
    test.skip(!tenantProbeOk, "Backend API is not available for transport validation")

    const startSlider = Date.now()
    const slider = await getFirstOkJson(api, endpointCandidates.slider)
    const sliderDuration = Date.now() - startSlider
    expect(slider.status).toBe(200)
    expect(sliderDuration).toBeLessThan(5_000)
    const sliderPayload = validateSliderTransport(slider.json)
    expect(Array.isArray(sliderPayload.slides)).toBeTruthy()
    expect(containsPotentialHtmlInjection(sliderPayload)).toBeFalsy()

    const startNav = Date.now()
    const navigation = await getFirstOkJson(api, endpointCandidates.navigation)
    const navDuration = Date.now() - startNav
    expect(navigation.status).toBe(200)
    expect(navDuration).toBeLessThan(5_000)
    validateNavigationTransport(navigation.json)
    expect(containsPotentialHtmlInjection(navigation.json)).toBeFalsy()

    const startFooter = Date.now()
    const footer = await getFirstOkJson(api, endpointCandidates.footer)
    const footerDuration = Date.now() - startFooter
    expect(footer.status).toBe(200)
    expect(footerDuration).toBeLessThan(5_000)
    validateFooterTransport(footer.json)
    expect(containsPotentialHtmlInjection(footer.json)).toBeFalsy()

    const homeDataAttempt = await api.get("/api/home-data")
    if (homeDataAttempt.ok()) {
      const homeData = await homeDataAttempt.json()
      expect(typeof homeData).toBe("object")
      expect(homeData).toBeTruthy()
      expect(containsPotentialHtmlInjection(homeData)).toBeFalsy()
    } else {
      const fallback = {
        tenant: await (await api.get("/api/tenant/current")).json(),
        navigation: navigation.json,
        footer: footer.json,
        slider: slider.json,
      }
      expect(typeof fallback.tenant).toBe("object")
      expect(containsPotentialHtmlInjection(fallback)).toBeFalsy()
    }
  })
})
