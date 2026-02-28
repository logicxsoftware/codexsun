import { httpClient } from "@/shared/services/http-client"
import type { NavigationConfigDto, UpsertNavigationConfigPayload } from "@/features/web-navigation/types/navigation-types"

export const navigationApi = {
  getPublicNavigationConfig: (): Promise<NavigationConfigDto> => httpClient.get<NavigationConfigDto>("/web/navigation-config"),
  getPublicFooterConfig: (): Promise<NavigationConfigDto> => httpClient.get<NavigationConfigDto>("/web/footer-config"),

  getAdminNavigationConfig: (): Promise<NavigationConfigDto> => httpClient.get<NavigationConfigDto>("/admin/web-navigation-config"),
  upsertAdminNavigationConfig: (payload: UpsertNavigationConfigPayload): Promise<NavigationConfigDto> =>
    httpClient.put<NavigationConfigDto>("/admin/web-navigation-config", payload),
  resetAdminNavigationConfig: (): Promise<NavigationConfigDto> =>
    httpClient.post<NavigationConfigDto>("/admin/web-navigation-config/reset", {}),

  getAdminFooterConfig: (): Promise<NavigationConfigDto> => httpClient.get<NavigationConfigDto>("/admin/footer-config"),
  upsertAdminFooterConfig: (payload: UpsertNavigationConfigPayload): Promise<NavigationConfigDto> =>
    httpClient.put<NavigationConfigDto>("/admin/footer-config", payload),
  resetAdminFooterConfig: (): Promise<NavigationConfigDto> => httpClient.post<NavigationConfigDto>("/admin/footer-config/reset", {}),
}
