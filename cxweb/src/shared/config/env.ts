export interface AppEnvironment {
  apiBaseUrl: string
  appName: string
}

const readValue = (value: string | undefined, fallback: string): string => {
  if (!value || value.trim().length === 0) {
    return fallback
  }

  return value.trim()
}

export const appEnvironment: AppEnvironment = {
  apiBaseUrl: readValue(import.meta.env.VITE_API_BASE_URL, "/api"),
  appName: readValue(import.meta.env.VITE_APP_NAME, "Codexsun"),
}
