import { defineConfig, devices } from "@playwright/test"

const frontendUrl = process.env.LIVE_FRONTEND_URL ?? "http://localhost:7043"

export default defineConfig({
  testDir: "./tests/live",
  fullyParallel: false,
  workers: 1,
  forbidOnly: !!process.env.CI,
  retries: process.env.CI ? 1 : 0,
  timeout: 120_000,
  expect: {
    timeout: 20_000,
  },
  reporter: [["list"], ["html", { open: "never", outputFolder: "playwright-report-live" }]],
  use: {
    baseURL: frontendUrl,
    headless: process.env.PLAYWRIGHT_HEADED !== "true",
    trace: "on-first-retry",
    screenshot: "only-on-failure",
    video: "retain-on-failure",
    actionTimeout: 20_000,
    navigationTimeout: 45_000,
  },
  projects: [
    {
      name: "chromium",
      use: { ...devices["Desktop Chrome"] },
    },
  ],
  webServer: [
    {
      command: "\"C:\\Program Files\\dotnet\\dotnet.exe\" run --launch-profile http",
      cwd: "../cxserver",
      url: "http://localhost:7041/",
      reuseExistingServer: true,
      timeout: 120_000,
    },
    {
      command: "npm run dev -- --host 0.0.0.0 --port 7043 --strictPort",
      cwd: ".",
      env: {
        SERVER_HTTP: "http://localhost:7041",
      },
      url: frontendUrl,
      reuseExistingServer: true,
      timeout: 120_000,
    },
  ],
})
