import { expect, test } from "@playwright/test"
import { liveBackendUrl, livePrimaryDomain, tenantHeaders } from "./live-config"

const isIntegerInRange = (value: unknown, min: number, max: number): boolean =>
  typeof value === "number" && Number.isInteger(value) && value >= min && value <= max

test.describe("Live API contract", () => {
  test("validates slider enum ranges and required fields", async ({ request }) => {
    const response = await request.get(`${liveBackendUrl}/api/slider`, { headers: tenantHeaders(livePrimaryDomain) })
    expect(response.status()).toBe(200)
    const payload = (await response.json()) as {
      id: string
      isActive: boolean
      slides: Array<Record<string, unknown>>
    }

    expect(typeof payload.id).toBe("string")
    expect(typeof payload.isActive).toBe("boolean")
    expect(Array.isArray(payload.slides)).toBeTruthy()

    for (const slide of payload.slides) {
      expect(typeof slide.id).toBe("string")
      expect(typeof slide.title).toBe("string")
      expect(typeof slide.tagline).toBe("string")
      expect(isIntegerInRange(slide.ctaColor, 1, 6)).toBeTruthy()
      expect(isIntegerInRange(slide.direction, 1, 3)).toBeTruthy()
      expect(isIntegerInRange(slide.variant, 1, 6)).toBeTruthy()
      expect(isIntegerInRange(slide.intensity, 1, 3)).toBeTruthy()
      expect(isIntegerInRange(slide.backgroundMode, 1, 4)).toBeTruthy()
      expect(isIntegerInRange(slide.mediaType, 1, 3)).toBeTruthy()
      expect(slide).not.toHaveProperty("btn_cta")
    }
  })
})
