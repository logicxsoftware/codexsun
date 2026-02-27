import type { WebHeaderConfig } from "@/features/web/types/web-header-config"

const webHeaderConfig: WebHeaderConfig = {
  logo: {
    path: "M12 2.5C6.753 2.5 2.5 6.753 2.5 12C2.5 17.247 6.753 21.5 12 21.5C15.146 21.5 18.109 19.958 19.895 17.335L16.917 15.779C15.745 17.312 13.941 18.25 12 18.25C8.548 18.25 5.75 15.452 5.75 12C5.75 8.548 8.548 5.75 12 5.75C13.941 5.75 15.745 6.688 16.917 8.221L19.895 6.665C18.109 4.042 15.146 2.5 12 2.5Z",
    fillClassName: "fill-brand",
    textClassName: "text-header-foreground",
  },
  brand: {
    name: "CodexSun",
    to: "/",
  },
  menu: {
    items: [
      { label: "Home", to: "/" },
      { label: "About", to: "/about" },
      { label: "Services", to: "/services" },
      { label: "Blog", to: "/blog" },
      { label: "Contact", to: "/contact" },
    ],
    position: "left",
    textClassName: "text-header-foreground/85",
    hoverUnderlineEnabled: true,
    hoverUnderlineClassName: "after:bg-menu-hover-underline",
    hoverBackgroundClassName: "hover:bg-menu-hover",
  },
  layout: {
    containerWidth: "fixed",
    size: "md",
    spacingScale: "normal",
  },
  colors: {
    wrapperClassName: "border-b border-border/70 bg-header-bg text-header-foreground",
    dividerClassName: "bg-border/60",
  },
  auth: {
    loginLabel: "Login",
    dashboardLabel: "Dashboard",
    logoutLabel: "Logout",
    loginPath: "/auth/login",
    dashboardPath: "/app",
  },
}

export default webHeaderConfig
