import { test, expect } from "../fixtures/tenant.fixture"
import { endpointCandidates, getFirstOkJson, validateSliderTransport } from "../utils/network-utils"

test.describe("Tenant isolation", () => {
  test("ensures tenant-specific data separation", async ({ createApiForHost, primaryHost, secondaryHost }) => {
    const primaryApi = await createApiForHost(primaryHost)
    const secondaryApi = await createApiForHost(secondaryHost)
    try {
      const probe = await primaryApi.get("/api/tenant/current", { timeout: 5_000 })
      test.skip(!probe.ok(), "Primary backend API is not available")
    } catch {
      test.skip(true, "Primary backend API is not available")
    }

    const primaryTenantResponse = await primaryApi.get("/api/tenant/current")
    const secondaryTenantResponse = await secondaryApi.get("/api/tenant/current")

    test.skip(!secondaryTenantResponse.ok(), `Secondary tenant is not available for host ${secondaryHost}`)
    expect(primaryTenantResponse.ok()).toBeTruthy()
    expect(secondaryTenantResponse.ok()).toBeTruthy()

    const primaryTenant = await primaryTenantResponse.json()
    const secondaryTenant = await secondaryTenantResponse.json()
    expect(primaryTenant.tenantId).not.toBe(secondaryTenant.tenantId)

    const primarySlider = validateSliderTransport((await getFirstOkJson(primaryApi, endpointCandidates.slider)).json)
    const secondarySlider = validateSliderTransport((await getFirstOkJson(secondaryApi, endpointCandidates.slider)).json)

    const primaryNav = (await getFirstOkJson(primaryApi, endpointCandidates.navigation)).json
    const secondaryNav = (await getFirstOkJson(secondaryApi, endpointCandidates.navigation)).json

    const primarySliderTitles = primarySlider.slides.map((slide) => slide.title).join("|")
    const secondarySliderTitles = secondarySlider.slides.map((slide) => slide.title).join("|")
    const primaryNavRaw = JSON.stringify(primaryNav)
    const secondaryNavRaw = JSON.stringify(secondaryNav)

    const hasAnyDifference = primarySliderTitles !== secondarySliderTitles || primaryNavRaw !== secondaryNavRaw
    expect(hasAnyDifference).toBeTruthy()
  })
})
