import { useMemo, useState } from "react"
import { Menu as MenuIcon, X } from "lucide-react"

import { menuGroupType } from "@/features/menu-admin/types/menu-types"
import type { MenuRenderGroupDto, MenuRenderNodeDto } from "@/features/menu-admin/types/menu-types"
import type { HeaderLayoutConfig } from "@/features/web-navigation/types/navigation-types"
import { cn } from "@/lib/utils"
import NavMegaMenu from "@/features/web-navigation/components/NavMegaMenu"

type NavMenuProps = {
  groups: MenuRenderGroupDto[]
  layout: HeaderLayoutConfig
}

const sizeClassMap: Record<HeaderLayoutConfig["menuSize"], string> = {
  small: "text-xs",
  medium: "text-sm",
  large: "text-base",
}

const alignClassMap: Record<HeaderLayoutConfig["menuAlign"], string> = {
  left: "justify-start",
  center: "justify-center",
  right: "justify-end",
}

const flattenItems = (items: MenuRenderNodeDto[]): MenuRenderNodeDto[] => {
  return items.flatMap((item) => [item, ...flattenItems(item.children)])
}

function NavMenu({ groups, layout }: NavMenuProps) {
  const [mobileOpen, setMobileOpen] = useState(false)
  const headerGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Header), [groups])
  const mobileGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Mobile), [groups])

  const headerMenus = headerGroup?.menus ?? []
  const primaryMenu = headerMenus.find((entry) => !entry.isMegaMenu)
  const primaryItems = primaryMenu ? flattenItems(primaryMenu.items) : []
  const megaMenus = headerMenus.filter((entry) => entry.isMegaMenu)
  const mobileMenus = mobileGroup?.menus ?? []
  const mobileItems = mobileMenus.length > 0 ? flattenItems(mobileMenus[0]?.items ?? []) : primaryItems

  return (
    <>
      <button
        type="button"
        aria-label="Toggle mobile menu"
        className="inline-flex h-9 w-9 items-center justify-center rounded-md border border-border bg-background text-foreground md:hidden"
        onClick={() => setMobileOpen((prev) => !prev)}
      >
        {mobileOpen ? <X className="h-4 w-4" /> : <MenuIcon className="h-4 w-4" />}
      </button>

      <nav aria-label="Main navigation" className={cn("hidden min-w-0 flex-1 items-center md:flex", alignClassMap[layout.menuAlign])}>
        <div className="flex items-center gap-0.5 lg:gap-1">
          {primaryItems.map((item) => (
            <a
              key={`${item.slug}-${item.url}`}
              href={item.url}
              target={item.target === 2 ? "_blank" : "_self"}
              rel={item.target === 2 ? "noreferrer" : undefined}
              className={cn(
                "rounded-md px-3 py-2 font-medium text-foreground/90 transition-colors hover:bg-menu-hover hover:text-foreground",
                sizeClassMap[layout.menuSize],
              )}
            >
              {item.title}
            </a>
          ))}
          {megaMenus.map((megaMenu) => (
            <NavMegaMenu key={megaMenu.slug} menu={megaMenu} />
          ))}
        </div>
      </nav>

      <div className={cn("w-full border-t border-border pt-2 md:hidden", !mobileOpen ? "hidden" : "")}>
        <nav aria-label="Mobile navigation" className="grid gap-1">
          {mobileItems.map((item) => (
            <a
              key={`mobile-${item.slug}-${item.url}`}
              href={item.url}
              target={item.target === 2 ? "_blank" : "_self"}
              rel={item.target === 2 ? "noreferrer" : undefined}
              className="rounded-md px-3 py-2 text-sm text-foreground hover:bg-menu-hover"
              onClick={() => setMobileOpen(false)}
            >
              {item.title}
            </a>
          ))}
        </nav>
      </div>
    </>
  )
}

export default NavMenu
