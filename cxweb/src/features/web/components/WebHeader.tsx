import { useEffect, useMemo, useState } from "react"
import { Link, useNavigate } from "react-router"
import { Menu as MenuIcon, X } from "lucide-react"

import {
  Brand,
  Container,
  Divider,
  HeaderTitle,
  HeaderWrapper,
  NavItem,
  NavList,
  ThemedButton,
} from "@/features/web/components/primitives"
import webHeaderConfig from "@/features/web/config/web-header-config"
import { menuApi } from "@/features/menu-admin/services/menu-api"
import { menuGroupType, menuItemTarget, type MenuRenderDto, type MenuRenderGroupDto, type MenuRenderNodeDto } from "@/features/menu-admin/types/menu-types"
import { cn } from "@/lib/utils"
import { useAuth } from "@/shared/auth"
import { ThemeSwitch } from "@/shared/theme"

const headerSizeClassMap = {
  sm: "py-2 md:py-3",
  md: "py-3 md:py-4",
  lg: "py-4 md:py-5",
} as const

const headerSpacingClassMap = {
  compact: {
    contentGap: "gap-2 md:gap-3",
    actionGap: "gap-1 md:gap-2",
    navGap: "gap-0.5 md:gap-1",
    mobileNavPadding: "pt-2",
  },
  normal: {
    contentGap: "gap-3 md:gap-4",
    actionGap: "gap-2",
    navGap: "gap-1 md:gap-1.5",
    mobileNavPadding: "pt-2.5",
  },
  relaxed: {
    contentGap: "gap-4 md:gap-6",
    actionGap: "gap-2 md:gap-3",
    navGap: "gap-1.5 md:gap-2",
    mobileNavPadding: "pt-3",
  },
} as const

const menuPositionClassMap = {
  left: "justify-start",
  center: "justify-center",
  right: "justify-end",
} as const

const flattenItems = (items: MenuRenderNodeDto[]): MenuRenderNodeDto[] => {
  return items.flatMap((item) => [item, ...flattenItems(item.children)])
}

const getLinkTarget = (target: number): "_self" | "_blank" => {
  return target === menuItemTarget.Blank ? "_blank" : "_self"
}

function MegaMenu({ menu }: { menu: MenuRenderDto }) {
  const items = useMemo(() => menu.items, [menu.items])

  return (
    <div className="relative group">
      <button type="button" className="rounded-md px-3 py-2 text-sm font-medium text-header-foreground/85 hover:bg-menu-hover hover:text-foreground">
        {menu.name}
      </button>
      <div className="invisible absolute left-0 top-full z-40 mt-2 w-[520px] rounded-lg border border-border/70 bg-card p-4 opacity-0 shadow-lg transition-all duration-150 group-hover:visible group-hover:opacity-100">
        <div className="grid grid-cols-2 gap-3">
          {items.map((item) => (
            <div key={`${menu.slug}-${item.slug}-${item.url}`} className="grid gap-1 rounded-md border border-border/60 bg-background p-2">
              <a href={item.url} target={getLinkTarget(item.target)} rel={item.target === menuItemTarget.Blank ? "noreferrer" : undefined} className="text-sm font-medium text-foreground hover:text-link-hover">
                {item.title}
              </a>
              {item.children.length > 0 ? (
                <div className="grid gap-1 text-xs text-muted-foreground">
                  {item.children.map((child) => (
                    <a key={`${item.slug}-${child.slug}-${child.url}`} href={child.url} target={getLinkTarget(child.target)} rel={child.target === menuItemTarget.Blank ? "noreferrer" : undefined} className="hover:text-link-hover">
                      {child.title}
                    </a>
                  ))}
                </div>
              ) : null}
            </div>
          ))}
        </div>
      </div>
    </div>
  )
}

function WebHeader() {
  const { auth, brand, colors, layout, logo, menu } = webHeaderConfig
  const { isAuthenticated, signOut } = useAuth()
  const navigate = useNavigate()
  const spacing = headerSpacingClassMap[layout.spacingScale]

  const [groups, setGroups] = useState<MenuRenderGroupDto[]>([])
  const [mobileOpen, setMobileOpen] = useState(false)

  useEffect(() => {
    let active = true

    const load = async () => {
      try {
        const response = await menuApi.getRenderMenus()
        if (active) {
          setGroups(response)
        }
      } catch {
        if (active) {
          setGroups([])
        }
      }
    }

    void load()

    return () => {
      active = false
    }
  }, [])

  const headerGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Header), [groups])
  const mobileGroup = useMemo(() => groups.find((group) => group.groupType === menuGroupType.Mobile), [groups])

  const headerMenus = headerGroup?.menus ?? []
  const primaryMenu = headerMenus.find((item) => !item.isMegaMenu)
  const primaryItems = primaryMenu ? flattenItems(primaryMenu.items) : []
  const megaMenus = headerMenus.filter((item) => item.isMegaMenu)
  const mobileMenus = mobileGroup?.menus ?? []
  const mobileItems = mobileMenus.length > 0 ? flattenItems(mobileMenus[0]?.items ?? []) : primaryItems

  const onLogout = () => {
    signOut()
    void navigate(brand.to)
  }

  return (
    <HeaderWrapper className={cn("backdrop-blur supports-[backdrop-filter]:bg-header-bg/80", colors.wrapperClassName)}>
      <Container width={layout.containerWidth} paddingScale={layout.spacingScale} className="flex flex-col">
        <div className={cn("grid grid-cols-[minmax(0,1fr)_auto] items-center", headerSizeClassMap[layout.size])}>
          <div className={cn("flex min-w-0 items-center", spacing.contentGap)}>
            <Brand
              to={brand.to}
              logoPath={logo.path}
              logoFillClassName={logo.fillClassName}
              logoTextClassName={logo.textClassName}
              logoClassName="bg-brand/15"
              className="min-w-0"
            >
              <HeaderTitle className={cn("truncate", logo.textClassName)}>{brand.name}</HeaderTitle>
            </Brand>

            <div className={cn("hidden min-w-0 flex-1 items-center md:flex", menuPositionClassMap[menu.position])}>
              <Divider className={cn("mr-4 h-6", colors.dividerClassName)} />
              <NavList className={cn("flex", spacing.navGap)}>
                {primaryItems.map((item) => (
                  <NavItem
                    key={`${item.slug}-${item.url}`}
                    to={item.url}
                    target={getLinkTarget(item.target)}
                    rel={item.target === menuItemTarget.Blank ? "noreferrer" : undefined}
                    toneClassName={menu.textClassName}
                    hoverBackgroundClassName={menu.hoverBackgroundClassName}
                    hoverUnderlineEnabled={menu.hoverUnderlineEnabled}
                    hoverUnderlineClassName={menu.hoverUnderlineClassName}
                  >
                    {item.title}
                  </NavItem>
                ))}
                {megaMenus.map((megaMenu) => (
                  <MegaMenu key={megaMenu.slug} menu={megaMenu} />
                ))}
              </NavList>
            </div>
          </div>

          <div className={cn("flex items-center justify-end", spacing.actionGap)}>
            <button
              type="button"
              className="inline-flex h-8 w-8 items-center justify-center rounded-md border border-border/70 bg-background text-foreground md:hidden"
              onClick={() => setMobileOpen((prev) => !prev)}
            >
              {mobileOpen ? <X className="h-4 w-4" /> : <MenuIcon className="h-4 w-4" />}
            </button>
            <ThemeSwitch />
            {isAuthenticated ? (
              <>
                <ThemedButton asChild type="button" className="h-8 bg-secondary px-3 text-secondary-foreground hover:bg-secondary/80 md:h-9 md:px-4">
                  <Link to={auth.dashboardPath}>{auth.dashboardLabel}</Link>
                </ThemedButton>
                <ThemedButton
                  type="button"
                  onClick={onLogout}
                  className="h-8 border border-border bg-background px-3 text-foreground hover:bg-menu-hover md:h-9 md:px-4"
                >
                  {auth.logoutLabel}
                </ThemedButton>
              </>
            ) : (
              <ThemedButton asChild type="button" className="h-8 bg-cta-bg px-3 text-cta-foreground hover:bg-cta-bg/90 md:h-9 md:px-4">
                <Link to={auth.loginPath}>{auth.loginLabel}</Link>
              </ThemedButton>
            )}
          </div>
        </div>

        <div className={cn("border-t border-border/60 md:hidden", spacing.mobileNavPadding, !mobileOpen && "hidden")}>
          <NavList className={cn("flex w-full items-center overflow-x-auto pb-2", spacing.navGap)}>
            {mobileItems.map((item) => (
              <NavItem
                key={`mobile-${item.slug}-${item.url}`}
                to={item.url}
                target={getLinkTarget(item.target)}
                rel={item.target === menuItemTarget.Blank ? "noreferrer" : undefined}
                toneClassName={menu.textClassName}
                hoverBackgroundClassName={menu.hoverBackgroundClassName}
                hoverUnderlineEnabled={menu.hoverUnderlineEnabled}
                hoverUnderlineClassName={menu.hoverUnderlineClassName}
                className="shrink-0"
              >
                {item.title}
              </NavItem>
            ))}
          </NavList>
        </div>
      </Container>
    </HeaderWrapper>
  )
}

export default WebHeader
