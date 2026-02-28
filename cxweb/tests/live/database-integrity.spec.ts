import { expect, test } from "@playwright/test"
import { createTenantPool, getActiveTenants, getSingleNumber } from "./db-helper"
import { liveBackendUrl, tenantHeaders } from "./live-config"

test.describe("Live database integrity", () => {
  test("validates slider CRUD persistence and relational integrity", async ({ request }) => {
    const tenants = await getActiveTenants()
    test.skip(tenants.length === 0, "No active tenant found")
    const tenant = tenants[0]

    const tenantDb = createTenantPool(tenant.connectionString)
    const uniqueSuffix = Date.now()
    const title = `Live Slide ${uniqueSuffix}`
    const updatedTitle = `Live Slide Updated ${uniqueSuffix}`

    const createResponse = await request.post(`${liveBackendUrl}/api/slider/slides`, {
      headers: tenantHeaders(tenant.domain),
      data: {
        order: 999,
        title,
        tagline: "Live validation",
        actionText: "Open",
        actionHref: "/live",
        ctaColor: 1,
        duration: 5000,
        direction: 1,
        variant: 1,
        intensity: 2,
        backgroundMode: 1,
        showOverlay: true,
        overlayToken: "muted",
        backgroundUrl: "https://images.unsplash.com/photo-1551434678-e076c223a692?w=1920",
        mediaType: 1,
        youtubeVideoId: null,
        isActive: true,
        highlights: [
          { text: "Live", variant: "primary", order: 0 },
        ],
      },
    })

    expect(createResponse.status()).toBe(201)
    const created = await createResponse.json()
    expect(typeof created.id).toBe("string")
    const createdId = created.id as string

    const createdCount = await getSingleNumber(tenantDb, "select count(*) from slides where id = ? and is_deleted = 0", [createdId])
    expect(createdCount).toBe(1)

    const updateResponse = await request.patch(`${liveBackendUrl}/api/slider/slides/${createdId}`, {
      headers: tenantHeaders(tenant.domain),
      data: {
        order: 998,
        title: updatedTitle,
        tagline: "Live validation updated",
        actionText: "Open Updated",
        actionHref: "/live-updated",
        ctaColor: 2,
        duration: 4000,
        direction: 2,
        variant: 2,
        intensity: 1,
        backgroundMode: 2,
        showOverlay: true,
        overlayToken: "muted",
        backgroundUrl: "https://images.unsplash.com/photo-1522071820081-009f0129c71c?w=1920",
        mediaType: 1,
        youtubeVideoId: null,
        isActive: true,
        highlights: [
          { text: "Live Update", variant: "success", order: 0 },
        ],
      },
    })

    expect(updateResponse.status()).toBe(200)
    const updatedCount = await getSingleNumber(tenantDb, "select count(*) from slides where id = ? and title = ? and is_deleted = 0", [createdId, updatedTitle])
    expect(updatedCount).toBe(1)

    const invalidEnumResponse = await request.patch(`${liveBackendUrl}/api/slider/slides/${createdId}`, {
      headers: tenantHeaders(tenant.domain),
      data: {
        order: 998,
        title: updatedTitle,
        tagline: "Live validation updated",
        actionText: "Open Updated",
        actionHref: "/live-updated",
        ctaColor: 99,
        duration: 4000,
        direction: 99,
        variant: 99,
        intensity: 99,
        backgroundMode: 99,
        showOverlay: true,
        overlayToken: "muted",
        backgroundUrl: "not-a-url",
        mediaType: 99,
        youtubeVideoId: "bad",
        isActive: true,
        highlights: [
          { text: "Live Update", variant: "success", order: 0 },
        ],
      },
    })
    expect(invalidEnumResponse.status()).toBeGreaterThanOrEqual(400)

    const deleteResponse = await request.delete(`${liveBackendUrl}/api/slider/slides/${createdId}`, { headers: tenantHeaders(tenant.domain) })
    expect([204, 200]).toContain(deleteResponse.status())
    const deletedActiveCount = await getSingleNumber(tenantDb, "select count(*) from slides where id = ? and is_deleted = 0", [createdId])
    expect(deletedActiveCount).toBe(0)

    const orphanLayers = await getSingleNumber(
      tenantDb,
      `select count(*) from slide_layers l
       left join slides s on s.id = l.slide_id
       where l.is_deleted = 0 and s.id is null`,
    )
    expect(orphanLayers).toBe(0)

    const orphanMenuItems = await getSingleNumber(
      tenantDb,
      `select count(*) from menu_items mi
       left join menus m on m.id = mi.menu_id
       where mi.is_deleted = 0 and m.id is null`,
    )
    expect(orphanMenuItems).toBe(0)

    await tenantDb.end()
  })
})
