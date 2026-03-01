import { liveBackendUrl, liveFrontendUrl } from "./live-config"
import { resolveRepoPaths, startProcess, waitForHttpReady, type ManagedProcess } from "./process-manager"

const backendHealthUrls = (baseUrl: string): string[] => [
  `${baseUrl}/health`,
  `${baseUrl}/healthz`,
  `${baseUrl}/`,
]

const frontendHealthUrls = (baseUrl: string): string[] => [
  `${baseUrl}/`,
]

const formatLogs = (process: ManagedProcess): string => {
  const output = `${process.stdout.join("")}\n${process.stderr.join("")}`.trim()
  return output.length > 6000 ? output.slice(output.length - 6000) : output
}

export default async function globalSetup(): Promise<() => Promise<void>> {
  const { backendCwd, frontendCwd } = resolveRepoPaths(process.cwd())

  const backend = process.platform === "win32"
    ? startProcess({
      name: "backend",
      command: "powershell",
      args: ["-NoProfile", "-Command", "dotnet run --launch-profile http"],
      cwd: backendCwd,
    })
    : startProcess({
      name: "backend",
      command: "dotnet",
      args: ["run", "--launch-profile", "http"],
      cwd: backendCwd,
    })

  try {
    await waitForHttpReady(backendHealthUrls(liveBackendUrl), 120_000)
  } catch (error) {
    await backend.stop()
    throw new Error(`Backend startup failed: ${(error as Error).message}\n${formatLogs(backend)}`)
  }

  const frontend = process.platform === "win32"
    ? startProcess({
      name: "frontend",
      command: "powershell",
      args: ["-NoProfile", "-Command", "npm run dev -- --host 0.0.0.0 --port 7043 --strictPort"],
      cwd: frontendCwd,
      env: {
        SERVER_HTTP: liveBackendUrl,
      },
    })
    : startProcess({
      name: "frontend",
      command: "npm",
      args: ["run", "dev", "--", "--host", "0.0.0.0", "--port", "7043", "--strictPort"],
      cwd: frontendCwd,
      env: {
        SERVER_HTTP: liveBackendUrl,
      },
    })

  try {
    await waitForHttpReady(frontendHealthUrls(liveFrontendUrl), 120_000)
  } catch (error) {
    await frontend.stop()
    await backend.stop()
    throw new Error(`Frontend startup failed: ${(error as Error).message}\n${formatLogs(frontend)}`)
  }

  return async () => {
    await frontend.stop()
    await backend.stop()
  }
}
