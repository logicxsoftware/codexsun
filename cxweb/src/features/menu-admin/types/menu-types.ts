export const menuGroupType = {
  Header: 1,
  Footer: 2,
  Mobile: 3,
  SideMenu: 4,
} as const

export const menuVariant = {
  Common: 1,
  Custom: 2,
} as const

export const menuItemTarget = {
  Self: 1,
  Blank: 2,
} as const

export type MenuGroupType = (typeof menuGroupType)[keyof typeof menuGroupType]
export type MenuVariant = (typeof menuVariant)[keyof typeof menuVariant]
export type MenuItemTarget = (typeof menuItemTarget)[keyof typeof menuItemTarget]

export type MenuGroupDto = {
  id: string
  tenantId: string | null
  type: MenuGroupType
  name: string
  slug: string
  description: string | null
  isActive: boolean
  createdAtUtc: string
  updatedAtUtc: string
}

export type MenuDto = {
  id: string
  tenantId: string | null
  menuGroupId: string
  name: string
  slug: string
  variant: MenuVariant
  isMegaMenu: boolean
  order: number
  isActive: boolean
  createdAtUtc: string
  updatedAtUtc: string
}

export type MenuItemNodeDto = {
  id: string
  tenantId: string | null
  menuId: string
  parentId: string | null
  title: string
  slug: string
  url: string
  target: MenuItemTarget
  icon: string | null
  description: string | null
  order: number
  isActive: boolean
  createdAtUtc: string
  updatedAtUtc: string
  children: MenuItemNodeDto[]
}

export type MenuRenderNodeDto = {
  title: string
  slug: string
  url: string
  target: MenuItemTarget
  icon: string | null
  description: string | null
  order: number
  children: MenuRenderNodeDto[]
}

export type MenuRenderDto = {
  name: string
  slug: string
  variant: MenuVariant
  isMegaMenu: boolean
  order: number
  items: MenuRenderNodeDto[]
}

export type MenuRenderGroupDto = {
  groupType: MenuGroupType
  groupSlug: string
  groupName: string
  menus: MenuRenderDto[]
}

export type CreateMenuGroupPayload = {
  tenantId: string | null
  type: MenuGroupType
  name: string
  slug: string
  description: string | null
  isActive: boolean
}

export type UpdateMenuGroupPayload = {
  name: string
  slug: string
  description: string | null
  isActive: boolean
}

export type CreateMenuPayload = {
  menuGroupId: string
  tenantId: string | null
  name: string
  slug: string
  variant: MenuVariant
  isMegaMenu: boolean
  order: number
  isActive: boolean
}

export type UpdateMenuPayload = {
  name: string
  slug: string
  variant: MenuVariant
  isMegaMenu: boolean
  order: number
  isActive: boolean
}

export type CreateMenuItemPayload = {
  menuId: string
  tenantId: string | null
  parentId: string | null
  title: string
  slug: string
  url: string
  target: MenuItemTarget
  icon: string | null
  description: string | null
  order: number
  isActive: boolean
}

export type UpdateMenuItemPayload = {
  parentId: string | null
  title: string
  slug: string
  url: string
  target: MenuItemTarget
  icon: string | null
  description: string | null
  order: number
  isActive: boolean
}

export type ReorderMenuItemsPayload = {
  items: Array<{
    itemId: string
    parentId: string | null
    order: number
  }>
}
