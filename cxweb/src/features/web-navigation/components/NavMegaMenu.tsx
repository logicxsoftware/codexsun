import { useMemo } from "react"

import { menuVariant } from "@/features/menu-admin/types/menu-types"
import type { MenuRenderDto, MenuRenderNodeDto } from "@/features/menu-admin/types/menu-types"
import { cn } from "@/lib/utils"

type NavMegaMenuProps = {
  menus: MenuRenderDto[]
  menuSizeClassName: string
}

const MEGA_LAYOUT_THRESHOLD = 6
const PRIMARY_COMMON_SLUGS = new Set(["home", "about"])
const TRAILING_COMMON_SLUGS = new Set(["blog", "contact"])

const sortByOrderAndSlug = <T extends { order: number; slug: string }>(left: T, right: T): number => {
  if (left.order !== right.order) {
    return left.order - right.order
  }

  return left.slug.localeCompare(right.slug)
}

const normalizeSlug = (value: string): string => value.trim().toLowerCase()

const toKey = (slug: string, url: string): string => {
  const normalized = normalizeSlug(slug)
  return normalized.length > 0 ? normalized : `url:${url}`
}

const cloneChildren = (nodes: MenuRenderNodeDto[]): MenuRenderNodeDto[] =>
  nodes
    .map((node) => ({
      ...node,
      children: cloneChildren(node.children ?? []),
    }))
    .sort(sortByOrderAndSlug)

export const mergeChildren = (commonChildren: MenuRenderNodeDto[], tenantChildren: MenuRenderNodeDto[]): MenuRenderNodeDto[] => {
  const merged = new Map<string, MenuRenderNodeDto>()

  for (const node of commonChildren) {
    const commonNode = {
      ...node,
      children: cloneChildren(node.children ?? []),
    }
    merged.set(toKey(commonNode.slug, commonNode.url), commonNode)
  }

  for (const node of tenantChildren) {
    const tenantNode = {
      ...node,
      children: cloneChildren(node.children ?? []),
    }
    const key = toKey(tenantNode.slug, tenantNode.url)
    const commonNode = merged.get(key)

    if (!commonNode) {
      merged.set(key, tenantNode)
      continue
    }

    const mergedNode: MenuRenderNodeDto = {
      ...commonNode,
      ...tenantNode,
      children:
        tenantNode.children.length === 0
          ? []
          : mergeChildren(commonNode.children, tenantNode.children),
    }

    merged.set(key, mergedNode)
  }

  return [...merged.values()].sort(sortByOrderAndSlug)
}

export const mergeMenus = (commonMenus: MenuRenderDto[], tenantMenus: MenuRenderDto[]): MenuRenderDto[] => {
  const primaryCommon = new Map<string, MenuRenderDto>()
  const trailingCommon = new Map<string, MenuRenderDto>()
  const otherCommon = new Map<string, MenuRenderDto>()
  const tenantAll = new Map<string, MenuRenderDto>()

  for (const menu of commonMenus) {
    const commonMenu: MenuRenderDto = {
      ...menu,
      items: cloneChildren(menu.items ?? []),
    }
    const key = toKey(commonMenu.slug, commonMenu.name)
    const normalizedSlug = normalizeSlug(commonMenu.slug)

    if (PRIMARY_COMMON_SLUGS.has(normalizedSlug)) {
      primaryCommon.set(key, commonMenu)
      continue
    }

    if (TRAILING_COMMON_SLUGS.has(normalizedSlug)) {
      trailingCommon.set(key, commonMenu)
      continue
    }

    otherCommon.set(key, commonMenu)
  }

  for (const menu of tenantMenus) {
    const tenantMenu: MenuRenderDto = {
      ...menu,
      items: cloneChildren(menu.items ?? []),
    }
    const key = toKey(tenantMenu.slug, tenantMenu.name)
    const commonMenu = primaryCommon.get(key) ?? trailingCommon.get(key) ?? otherCommon.get(key)

    tenantAll.set(key, tenantMenu)

    if (!commonMenu) {
      continue
    }

    const mergedMenu: MenuRenderDto = {
      ...commonMenu,
      ...tenantMenu,
      items:
        tenantMenu.items.length === 0
          ? []
          : mergeChildren(commonMenu.items, tenantMenu.items),
    }

    if (primaryCommon.has(key)) {
      primaryCommon.set(key, mergedMenu)
      continue
    }

    if (trailingCommon.has(key)) {
      trailingCommon.set(key, mergedMenu)
      continue
    }

    otherCommon.set(key, mergedMenu)
  }

  const tenantSpecific = [...tenantAll.entries()]
    .filter(([key]) => !primaryCommon.has(key) && !trailingCommon.has(key) && !otherCommon.has(key))
    .map(([, menu]) => menu)
    .sort(sortByOrderAndSlug)

  return [
    ...[...primaryCommon.values()].sort(sortByOrderAndSlug),
    ...tenantSpecific,
    ...[...otherCommon.values()].sort(sortByOrderAndSlug),
    ...[...trailingCommon.values()].sort(sortByOrderAndSlug),
  ]
}

export const buildMergedMenuItems = (menus: MenuRenderDto[]): MenuRenderNodeDto[] => {
  const commonMenus = menus.filter((menu) => menu.variant === menuVariant.Common)
  const tenantMenus = menus.filter((menu) => menu.variant !== menuVariant.Common)
  const mergedMenus = mergeMenus(commonMenus, tenantMenus)

  return mergedMenus.reduce<MenuRenderNodeDto[]>((accumulator, menu) => mergeChildren(accumulator, menu.items ?? []), [])
}

const getNodeKey = (node: MenuRenderNodeDto, prefix: string): string => `${prefix}-${toKey(node.slug, node.url)}`

function NavMegaMenu({ menus, menuSizeClassName }: NavMegaMenuProps) {
  const mergedItems = useMemo(() => buildMergedMenuItems(menus), [menus])

  const renderNestedItems = (items: MenuRenderNodeDto[], level: number, prefix: string) => {
    if (items.length === 0) {
      return null
    }

    return (
      <ul className={cn("grid gap-1", level > 0 ? "ml-3 border-l border-border/60 pl-3" : "")}>
        {items.map((item) => (
          <li key={getNodeKey(item, prefix)}>
            <a
              href={item.url}
              target={item.target === 2 ? "_blank" : "_self"}
              rel={item.target === 2 ? "noreferrer" : undefined}
              className={cn(
                "inline-flex rounded-md px-2 py-1.5 text-sm text-foreground/90 transition-colors hover:bg-menu-hover hover:text-foreground",
                level === 0 ? "font-medium" : "text-muted-foreground hover:text-link-hover",
              )}
            >
              {item.title}
            </a>
            {renderNestedItems(item.children, level + 1, `${prefix}-${toKey(item.slug, item.url)}`)}
          </li>
        ))}
      </ul>
    )
  }

  const renderDropdown = (item: MenuRenderNodeDto, keyPrefix: string) => {
    const hasChildren = item.children.length > 0
    const isMegaLayout = item.children.length > MEGA_LAYOUT_THRESHOLD || item.children.some((child) => child.children.length > 0)

    if (!hasChildren) {
      return (
        <a
          href={item.url}
          target={item.target === 2 ? "_blank" : "_self"}
          rel={item.target === 2 ? "noreferrer" : undefined}
          className={cn(
            "rounded-md px-3 py-2 font-medium text-foreground/90 transition-colors hover:bg-menu-hover hover:text-foreground",
            menuSizeClassName,
          )}
        >
          {item.title}
        </a>
      )
    }

    return (
      <div className="group relative">
        <button
          type="button"
          className={cn(
            "rounded-md px-3 py-2 font-medium text-foreground/90 transition-colors hover:bg-menu-hover hover:text-foreground",
            menuSizeClassName,
          )}
        >
          {item.title}
        </button>
        <div className="invisible absolute left-0 top-full z-50 mt-2 min-w-64 rounded-lg border border-border bg-card p-3 opacity-0 shadow-md transition-all duration-200 ease-out group-hover:visible group-hover:translate-y-0 group-hover:opacity-100 motion-reduce:transition-none">
          {isMegaLayout ? (
            <div className="grid min-w-160 grid-cols-2 gap-2">
              {item.children.map((child) => (
                <div key={getNodeKey(child, keyPrefix)} className="rounded-md border border-border/60 bg-background p-2">
                  <a
                    href={child.url}
                    target={child.target === 2 ? "_blank" : "_self"}
                    rel={child.target === 2 ? "noreferrer" : undefined}
                    className="text-sm font-medium text-foreground transition-colors hover:text-link-hover"
                  >
                    {child.title}
                  </a>
                  {renderNestedItems(child.children, 1, keyPrefix)}
                </div>
              ))}
            </div>
          ) : (
            renderNestedItems(item.children, 0, keyPrefix)
          )}
        </div>
      </div>
    )
  }

  return (
    <div className="flex items-center gap-1 lg:gap-1.5">
      {mergedItems.map((item) => (
        <div key={getNodeKey(item, "desktop")}>{renderDropdown(item, "desktop")}</div>
      ))}
    </div>
  )
}

export default NavMegaMenu
