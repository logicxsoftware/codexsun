import { expect, test } from "@playwright/test"
import { getActiveTenants } from "./db-helper"
import { liveBackendUrl, tenantHeaders } from "./live-config"

test.describe("Live tenant isolation", () => {
  test("validates different tenants return isolated data", async ({ request }) => {
    const tenants = await getActiveTenants()
    test.skip(tenants.length < 2, "At least two active tenants are required")

    const primary = tenants[0]
    const secondary = tenants[1]

    const primaryTenantResponse = await request.get(`${liveBackendUrl}/api/tenant/current`, { headers: tenantHeaders(primary.domain) })
    const secondaryTenantResponse = await request.get(`${liveBackendUrl}/api/tenant/current`, { headers: tenantHeaders(secondary.domain) })
    expect(primaryTenantResponse.status()).toBe(200)
    expect(secondaryTenantResponse.status()).toBe(200)

    const primaryTenant = await primaryTenantResponse.json()
    const secondaryTenant = await secondaryTenantResponse.json()
    expect(primaryTenant.tenantId).not.toBe(secondaryTenant.tenantId)

    const primarySliderResponse = await request.get(`${liveBackendUrl}/api/slider`, { headers: tenantHeaders(primary.domain) })
    const secondarySliderResponse = await request.get(`${liveBackendUrl}/api/slider`, { headers: tenantHeaders(secondary.domain) })
    expect(primarySliderResponse.status()).toBe(200)
    expect(secondarySliderResponse.status()).toBe(200)

    const primarySlider = await primarySliderResponse.json()
    const secondarySlider = await secondarySliderResponse.json()
    expect(primarySlider.id).not.toBe(secondarySlider.id)

    const primaryMenus = await (await request.get(`${liveBackendUrl}/api/web/menus`, { headers: tenantHeaders(primary.domain) })).json()
    const secondaryMenus = await (await request.get(`${liveBackendUrl}/api/web/menus`, { headers: tenantHeaders(secondary.domain) })).json()
    const menusDifferent = JSON.stringify(primaryMenus) !== JSON.stringify(secondaryMenus)
    const slidersDifferent = primarySlider.id !== secondarySlider.id
    expect(menusDifferent || slidersDifferent).toBeTruthy()
  })

  test("rejects invalid host routing", async ({ request }) => {
    const invalidHostResponse = await request.get(`${liveBackendUrl}/api/tenant/current`, { headers: tenantHeaders("unknown-tenant.invalid") })
    expect([400, 404]).toContain(invalidHostResponse.status())
  })
})
