export type WebHeaderMenuPosition = "left" | "center" | "right"

export type WebHeaderContainerWidth = "fixed" | "full"

export type WebHeaderSize = "sm" | "md" | "lg"

export type WebHeaderSpacingScale = "compact" | "normal" | "relaxed"

export type WebHeaderMenuItem = {
  label: string
  to: string
}

export type WebHeaderConfig = {
  logo: {
    path: string
    fillClassName: string
    textClassName: string
  }
  brand: {
    name: string
    to: string
  }
  menu: {
    items: WebHeaderMenuItem[]
    position: WebHeaderMenuPosition
    textClassName: string
    hoverUnderlineEnabled: boolean
    hoverUnderlineClassName: string
    hoverBackgroundClassName: string
  }
  layout: {
    containerWidth: WebHeaderContainerWidth
    size: WebHeaderSize
    spacingScale: WebHeaderSpacingScale
  }
  colors: {
    wrapperClassName: string
    dividerClassName: string
  }
  auth: {
    loginLabel: string
    dashboardLabel: string
    logoutLabel: string
    loginPath: string
    dashboardPath: string
  }
}
