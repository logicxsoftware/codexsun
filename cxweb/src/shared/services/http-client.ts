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

const parseResponse = async <TResponse>(response: Response): Promise<TResponse> => {
  if (response.status === 204) {
    return undefined as TResponse
  }

  const contentType = response.headers.get("content-type")
  if (contentType && contentType.includes("application/json")) {
    return (await response.json()) as TResponse
  }

  return undefined as TResponse
}

const request = async <TResponse>(method: string, path: string, body?: unknown, init?: HttpRequestInit): Promise<TResponse> => {
  const shouldTrackLoading = !init?.skipGlobalLoading
  if (shouldTrackLoading) {
    startGlobalLoading()
  }

  try {
    const response = await fetch(resolveUrl(path), {
      ...init,
      method,
      headers: {
        Accept: "application/json",
        ...(body ? { "Content-Type": "application/json" } : {}),
        ...(init?.headers ?? {}),
      },
      body: body ? JSON.stringify(body) : undefined,
    })

    if (!response.ok) {
      throw new HttpError(response.status, `Request failed with status ${response.status}`)
    }

    return await parseResponse<TResponse>(response)
  } finally {
    if (shouldTrackLoading) {
      stopGlobalLoading()
    }
  }
}

export const httpClient = {
  get<TResponse>(path: string, init?: HttpRequestInit): Promise<TResponse> {
    return request<TResponse>("GET", path, undefined, init)
  },

  post<TResponse>(path: string, body: unknown, init?: HttpRequestInit): Promise<TResponse> {
    return request<TResponse>("POST", path, body, init)
  },

  patch<TResponse>(path: string, body: unknown, init?: HttpRequestInit): Promise<TResponse> {
    return request<TResponse>("PATCH", path, body, init)
  },

  put<TResponse>(path: string, body: unknown, init?: HttpRequestInit): Promise<TResponse> {
    return request<TResponse>("PUT", path, body, init)
  },

  delete<TResponse>(path: string, init?: HttpRequestInit): Promise<TResponse> {
    return request<TResponse>("DELETE", path, undefined, init)
  },
}
