import { useMemo, useState } from "react"
import { Menu as MenuIcon, X } from "lucide-react"

import { menuGroupType } from "@/features/menu-admin/types/menu-types"
import type { MenuRenderGroupDto, MenuRenderNodeDto } from "@/features/menu-admin/types/menu-types"
import type { HeaderLayoutConfig } from "@/features/web-navigation/types/navigation-types"
import { cn } from "@/lib/utils"
import NavMegaMenu, { buildMergedMenuItems } from "@/features/web-navigation/components/NavMegaMenu"

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

function NavMenu({ groups, layout }: NavMenuProps) {
  const [mobileOpen, setMobileOpen] = useState(false)
  const headerGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Header), [groups])
  const mobileGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Mobile), [groups])

  const headerMenus = useMemo(() => headerGroup?.menus ?? [], [headerGroup])
  const mergedHeaderItems = useMemo(() => buildMergedMenuItems(headerMenus), [headerMenus])
  const mobileMenus = useMemo(() => mobileGroup?.menus ?? [], [mobileGroup])
  const mobileItems = useMemo(() => (mobileMenus.length > 0 ? buildMergedMenuItems(mobileMenus) : mergedHeaderItems), [mergedHeaderItems, mobileMenus])

  const renderMobileItems = (items: MenuRenderNodeDto[], level = 0, prefix = "mobile") => {
    if (items.length === 0) {
      return null
    }

    return (
      <ul className={cn("grid gap-1", level > 0 ? "ml-3 border-l border-border/60 pl-3" : "")}>
        {items.map((item) => (
          <li key={`${prefix}-${item.slug}-${item.url}`}>
            <a
              href={item.url}
              target={item.target === 2 ? "_blank" : "_self"}
              rel={item.target === 2 ? "noreferrer" : undefined}
              className={cn(
                "block rounded-md px-3 py-2 text-foreground transition-colors hover:bg-menu-hover",
                level === 0 ? "text-sm font-medium" : "text-sm text-muted-foreground hover:text-link-hover",
              )}
              onClick={() => setMobileOpen(false)}
            >
              {item.title}
            </a>
            {renderMobileItems(item.children, level + 1, `${prefix}-${item.slug}-${item.url}`)}
          </li>
        ))}
      </ul>
    )
  }

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
        <NavMegaMenu menus={headerMenus} menuSizeClassName={sizeClassMap[layout.menuSize]} />
      </nav>

      <div className={cn("w-full border-t border-border pt-2 md:hidden", !mobileOpen ? "hidden" : "")}>
        <nav aria-label="Mobile navigation">{renderMobileItems(mobileItems)}</nav>
      </div>
    </>
  )
}

export default NavMenu
