import { test as base, type APIRequestContext, type Page } from "@playwright/test"

type TenantFixture = {
  primaryHost: string
  secondaryHost: string
  gotoHome: (page: Page, host?: string) => Promise<void>
  createApiForHost: (host: string) => Promise<APIRequestContext>
}

const primaryHost = process.env.PW_TENANT_PRIMARY_HOST ?? "localhost"
const secondaryHost = process.env.PW_TENANT_SECONDARY_HOST ?? "tenant2.localhost"

export const test = base.extend<TenantFixture>({
  primaryHost: async (_args, setFixture) => {
    await setFixture(primaryHost)
  },
  secondaryHost: async (_args, setFixture) => {
    await setFixture(secondaryHost)
  },
  gotoHome: async ({ baseURL }, setFixture) => {
    if (!baseURL) {
      throw new Error("baseURL is required")
    }

    await setFixture(async (page: Page, host?: string) => {
      const url = new URL(baseURL)
      if (host && host.trim().length > 0) {
        url.hostname = host
      }

      await page.goto(url.toString(), { waitUntil: "domcontentloaded" })
    })
  },
  createApiForHost: async ({ playwright, baseURL }, setFixture) => {
    if (!baseURL) {
      throw new Error("baseURL is required")
    }

    const contexts: APIRequestContext[] = []
    await setFixture(async (host: string) => {
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
