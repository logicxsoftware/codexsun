import { expect, type Page, type Request, type Response } from "@playwright/test"

export type NetworkMonitor = {
  getConsoleErrors: () => string[]
  getPageErrors: () => string[]
  getFailedRequests: () => string[]
  getApiFailures: () => string[]
  getApiCallCounts: () => Map<string, number>
  stop: () => void
}

const isAbortNoise = (value: string): boolean => value.includes("NS_BINDING_ABORTED") || value.includes("ERR_ABORTED")

export const attachNetworkMonitor = (page: Page, allowedApiFailurePatterns: RegExp[] = []): NetworkMonitor => {
  const consoleErrors: string[] = []
  const pageErrors: string[] = []
  const failedRequests: string[] = []
  const apiFailures: string[] = []
  const apiCallCounts = new Map<string, number>()

  const onConsole = (message: { type: () => string; text: () => string }) => {
    if (message.type() === "error") {
      consoleErrors.push(message.text())
    }
  }

  const onPageError = (error: Error) => {
    pageErrors.push(error.message)
  }

  const onRequestFailed = (request: Request) => {
    const failureText = request.failure()?.errorText ?? "failed"
    if (isAbortNoise(failureText)) {
      return
    }
    failedRequests.push(`${request.method()} ${request.url()} ${failureText}`)
  }

  const onResponse = (response: Response) => {
    const url = response.url()
    if (!url.includes("/api/")) {
      return
    }

    const key = `${response.request().method()} ${new URL(url).pathname}`
    apiCallCounts.set(key, (apiCallCounts.get(key) ?? 0) + 1)

    if (response.status() >= 400) {
      const allowed = allowedApiFailurePatterns.some((pattern) => pattern.test(url))
      if (!allowed) {
        apiFailures.push(`${response.status()} ${response.request().method()} ${url}`)
      }
    }
  }

  page.on("console", onConsole)
  page.on("pageerror", onPageError)
  page.on("requestfailed", onRequestFailed)
  page.on("response", onResponse)

  return {
    getConsoleErrors: () => [...consoleErrors],
    getPageErrors: () => [...pageErrors],
    getFailedRequests: () => [...failedRequests],
    getApiFailures: () => [...apiFailures],
    getApiCallCounts: () => new Map(apiCallCounts),
    stop: () => {
      page.off("console", onConsole)
      page.off("pageerror", onPageError)
      page.off("requestfailed", onRequestFailed)
      page.off("response", onResponse)
    },
  }
}

export const assertNoClientRuntimeIssues = (monitor: NetworkMonitor): void => {
  expect(monitor.getConsoleErrors(), `Console errors:\n${monitor.getConsoleErrors().join("\n")}`).toEqual([])
  expect(monitor.getPageErrors(), `Page errors:\n${monitor.getPageErrors().join("\n")}`).toEqual([])
  expect(monitor.getFailedRequests(), `Failed requests:\n${monitor.getFailedRequests().join("\n")}`).toEqual([])
  expect(monitor.getApiFailures(), `API failures:\n${monitor.getApiFailures().join("\n")}`).toEqual([])
}

export const assertNoDuplicateApiCalls = (monitor: NetworkMonitor, maxPerEndpoint = 3): void => {
  const duplicates = [...monitor.getApiCallCounts().entries()].filter(([, count]) => count > maxPerEndpoint)
  expect(duplicates, `Duplicate API calls: ${JSON.stringify(duplicates)}`).toEqual([])
}
