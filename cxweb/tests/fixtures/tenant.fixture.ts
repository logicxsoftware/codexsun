import { test as base, type APIRequestContext, type BrowserContext, type Page } from "@playwright/test"

type TenantFixture = {
  primaryHost: string
  secondaryHost: string
  gotoHome: (page: Page, host?: string) => Promise<void>
  createApiForHost: (host: string) => Promise<APIRequestContext>
}

const primaryHost = process.env.PW_TENANT_PRIMARY_HOST ?? "localhost"
const secondaryHost = process.env.PW_TENANT_SECONDARY_HOST ?? "tenant2.localhost"

export const test = base.extend<TenantFixture>({
  primaryHost: async ({}, use) => {
    await use(primaryHost)
  },
  secondaryHost: async ({}, use) => {
    await use(secondaryHost)
  },
  gotoHome: async ({ baseURL }, use) => {
    if (!baseURL) {
      throw new Error("baseURL is required")
    }

    await use(async (page: Page, host?: string) => {
      const url = new URL(baseURL)
      if (host && host.trim().length > 0) {
        url.hostname = host
      }

      await page.goto(url.toString(), { waitUntil: "domcontentloaded" })
    })
  },
  createApiForHost: async ({ playwright, baseURL }, use) => {
    if (!baseURL) {
      throw new Error("baseURL is required")
    }

    const contexts: APIRequestContext[] = []
    await use(async (host: string) => {
      const context = await playwright.request.newContext({
        baseURL,
        extraHTTPHeaders: {
          Host: host,
        },
      })
      contexts.push(context)
      return context
    })

    for (const context of contexts) {
      await context.dispose()
    }
  },
})

export { expect } from "@playwright/test"
