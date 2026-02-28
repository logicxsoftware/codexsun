import { spawn, type ChildProcess } from "node:child_process"
import path from "node:path"

export type ManagedProcess = {
  readonly name: string
  readonly child: ChildProcess
  readonly stdout: string[]
  readonly stderr: string[]
  stop: () => Promise<void>
}

type StartProcessInput = {
  name: string
  command: string
  args: string[]
  cwd: string
  env?: NodeJS.ProcessEnv
}

const killTree = async (pid: number): Promise<void> => {
  if (process.platform === "win32") {
    await new Promise<void>((resolve) => {
      const killer = spawn("taskkill", ["/pid", String(pid), "/t", "/f"], { stdio: "ignore", shell: false, windowsHide: true })
      killer.on("exit", () => resolve())
      killer.on("error", () => resolve())
    })
    return
  }

  try {
    process.kill(-pid, "SIGKILL")
  } catch {
    try {
      process.kill(pid, "SIGKILL")
    } catch {
    }
  }
}

export const startProcess = (input: StartProcessInput): ManagedProcess => {
  const stdout: string[] = []
  const stderr: string[] = []

  const child = spawn(input.command, input.args, {
    cwd: input.cwd,
    env: { ...process.env, ...input.env },
    shell: false,
    windowsHide: true,
    detached: process.platform !== "win32",
  })

  child.stdout?.on("data", (chunk: Buffer) => {
    stdout.push(chunk.toString())
  })
  child.stderr?.on("data", (chunk: Buffer) => {
    stderr.push(chunk.toString())
  })

  return {
    name: input.name,
    child,
    stdout,
    stderr,
    stop: async () => {
      if (child.exitCode !== null || child.pid === undefined) {
        return
      }
      await killTree(child.pid)
    },
  }
}

export const waitForHttpReady = async (
  urls: readonly string[],
  timeoutMs: number,
  successStatuses: readonly number[] = [200, 204],
): Promise<string> => {
  const start = Date.now()
  let lastError = ""

  while (Date.now() - start < timeoutMs) {
    for (const url of urls) {
      try {
        const response = await fetch(url)
        if (successStatuses.includes(response.status)) {
          return url
        }
        if (response.status < 500 && response.status !== 404) {
          return url
        }
        lastError = `status:${response.status} url:${url}`
      } catch (error) {
        lastError = `${url} ${(error as Error).message}`
      }
    }
    await new Promise((resolve) => setTimeout(resolve, 750))
  }

  throw new Error(`HTTP readiness timeout. ${lastError}`)
}

export const resolveRepoPaths = (cwd: string) => {
  const repoRoot = path.resolve(cwd, "..", "..")
  const backendCwd = path.resolve(repoRoot, "cxserver")
  const frontendCwd = path.resolve(repoRoot, "cxweb")
  return { repoRoot, backendCwd, frontendCwd }
}
