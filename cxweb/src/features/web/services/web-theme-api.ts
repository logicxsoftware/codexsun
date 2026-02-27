import { httpClient } from "@/shared/services/http-client"

export type WebThemeResponse = {
  light?: Record<string, unknown>
  dark?: Record<string, unknown>
  defaultMode?: string
  mode?: string
}

export const webThemeApi = {
  getTheme: (): Promise<WebThemeResponse> =>
    httpClient.get<WebThemeResponse>("/web/theme", {
      skipGlobalLoading: true,
    }),
}
