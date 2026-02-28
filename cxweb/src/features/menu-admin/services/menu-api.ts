import { httpClient } from "@/shared/services/http-client"
import type {
  CreateMenuGroupPayload,
  CreateMenuItemPayload,
  CreateMenuPayload,
  MenuDto,
  MenuGroupDto,
  MenuItemNodeDto,
  MenuRenderGroupDto,
  ReorderMenuItemsPayload,
  UpdateMenuGroupPayload,
  UpdateMenuItemPayload,
  UpdateMenuPayload,
} from "@/features/menu-admin/types/menu-types"

export const menuApi = {
  getMenuGroups: (includeGlobal = true, activeOnly = false): Promise<MenuGroupDto[]> =>
    httpClient.get<MenuGroupDto[]>(`/admin/menu-groups?includeGlobal=${String(includeGlobal)}&activeOnly=${String(activeOnly)}`),

  createMenuGroup: (payload: CreateMenuGroupPayload): Promise<MenuGroupDto> =>
    httpClient.post<MenuGroupDto>("/admin/menu-groups", payload),

  updateMenuGroup: (id: string, payload: UpdateMenuGroupPayload): Promise<MenuGroupDto> =>
    httpClient.patch<MenuGroupDto>(`/admin/menu-groups/${encodeURIComponent(id)}`, payload),

  deleteMenuGroup: (id: string): Promise<void> =>
    httpClient.delete<void>(`/admin/menu-groups/${encodeURIComponent(id)}`),

  getMenusByGroup: (menuGroupId: string, activeOnly = false): Promise<MenuDto[]> =>
    httpClient.get<MenuDto[]>(`/admin/menu-groups/${encodeURIComponent(menuGroupId)}/menus?activeOnly=${String(activeOnly)}`),

  createMenu: (payload: CreateMenuPayload): Promise<MenuDto> => httpClient.post<MenuDto>("/admin/menus", payload),

  updateMenu: (id: string, payload: UpdateMenuPayload): Promise<MenuDto> =>
    httpClient.patch<MenuDto>(`/admin/menus/${encodeURIComponent(id)}`, payload),

  deleteMenu: (id: string): Promise<void> => httpClient.delete<void>(`/admin/menus/${encodeURIComponent(id)}`),

  getMenuItemTree: (menuId: string, activeOnly = false): Promise<MenuItemNodeDto[]> =>
    httpClient.get<MenuItemNodeDto[]>(`/admin/menus/${encodeURIComponent(menuId)}/items?activeOnly=${String(activeOnly)}`),

  createMenuItem: (payload: CreateMenuItemPayload): Promise<MenuItemNodeDto> =>
    httpClient.post<MenuItemNodeDto>("/admin/menu-items", payload),

  updateMenuItem: (id: string, payload: UpdateMenuItemPayload): Promise<MenuItemNodeDto> =>
    httpClient.patch<MenuItemNodeDto>(`/admin/menu-items/${encodeURIComponent(id)}`, payload),

  deleteMenuItem: (id: string): Promise<void> =>
    httpClient.delete<void>(`/admin/menu-items/${encodeURIComponent(id)}`),

  reorderMenuItems: (menuId: string, payload: ReorderMenuItemsPayload): Promise<void> =>
    httpClient.patch<void>(`/admin/menus/${encodeURIComponent(menuId)}/items/reorder`, payload),

  getRenderMenus: (): Promise<MenuRenderGroupDto[]> => httpClient.get<MenuRenderGroupDto[]>("/web/menus"),
}
