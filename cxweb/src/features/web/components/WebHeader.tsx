import { Link, useNavigate } from "react-router"

import webHeaderConfig from "@/features/web/config/web-header-config"
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

function WebHeader() {
  const { auth, brand, colors, layout, logo, menu } = webHeaderConfig
  const { isAuthenticated, signOut } = useAuth()
  const navigate = useNavigate()
  const spacing = headerSpacingClassMap[layout.spacingScale]

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
                {menu.items.map((item) => (
                  <NavItem
                    key={item.to}
                    to={item.to}
                    toneClassName={menu.textClassName}
                    hoverBackgroundClassName={menu.hoverBackgroundClassName}
                    hoverUnderlineEnabled={menu.hoverUnderlineEnabled}
                    hoverUnderlineClassName={menu.hoverUnderlineClassName}
                  >
                    {item.label}
                  </NavItem>
                ))}
              </NavList>
            </div>
          </div>

          <div className={cn("flex items-center justify-end", spacing.actionGap)}>
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

        <div className={cn("border-t border-border/60 md:hidden", spacing.mobileNavPadding)}>
          <NavList className={cn("flex w-full items-center overflow-x-auto pb-2", spacing.navGap)}>
            {menu.items.map((item) => (
              <NavItem
                key={`mobile-${item.to}`}
                to={item.to}
                toneClassName={menu.textClassName}
                hoverBackgroundClassName={menu.hoverBackgroundClassName}
                hoverUnderlineEnabled={menu.hoverUnderlineEnabled}
                hoverUnderlineClassName={menu.hoverUnderlineClassName}
                className="shrink-0"
              >
                {item.label}
              </NavItem>
            ))}
          </NavList>
        </div>
      </Container>
    </HeaderWrapper>
  )
}

export default WebHeader
