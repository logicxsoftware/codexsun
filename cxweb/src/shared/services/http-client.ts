import { appEnvironment } from "@/shared/config/env"
import { startGlobalLoading, stopGlobalLoading } from "@/shared/components/ui/GlobalLoader/loading-store"

export class HttpError extends Error {
  readonly status: number

  constructor(status: number, message: string) {
    super(message)
    this.status = status
  }
}

const normalizePath = (path: string): string => {
  if (path.startsWith("/")) {
    return path
  }

  return `/${path}`
}

const resolveUrl = (path: string): string => {
  const normalizedPath = normalizePath(path)

  if (appEnvironment.apiBaseUrl.startsWith("http://") || appEnvironment.apiBaseUrl.startsWith("https://")) {
    const base = appEnvironment.apiBaseUrl.endsWith("/") ? appEnvironment.apiBaseUrl.slice(0, -1) : appEnvironment.apiBaseUrl
    return `${base}${normalizedPath}`
  }

  const base = appEnvironment.apiBaseUrl.endsWith("/") ? appEnvironment.apiBaseUrl.slice(0, -1) : appEnvironment.apiBaseUrl
  return `${base}${normalizedPath}`
}

type HttpRequestInit = RequestInit & {
  skipGlobalLoading?: boolean
}

export const httpClient = {
  async get<TResponse>(path: string, init?: HttpRequestInit): Promise<TResponse> {
    const shouldTrackLoading = !init?.skipGlobalLoading
    if (shouldTrackLoading) {
      startGlobalLoading()
    }

    try {
      const response = await fetch(resolveUrl(path), {
        ...init,
        method: "GET",
        headers: {
          Accept: "application/json",
          ...(init?.headers ?? {}),
        },
      })

      if (!response.ok) {
        throw new HttpError(response.status, `Request failed with status ${response.status}`)
      }

      return (await response.json()) as TResponse
    } finally {
      if (shouldTrackLoading) {
        stopGlobalLoading()
      }
    }
  },
}
