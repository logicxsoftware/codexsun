import type { ComponentPropsWithoutRef } from "react"
import { Link } from "react-router"

import { cn } from "@/lib/utils"

type NavItemProps = ComponentPropsWithoutRef<typeof Link> & {
  toneClassName?: string
  hoverUnderlineEnabled?: boolean
  hoverUnderlineClassName?: string
  hoverBackgroundClassName?: string
}

function NavItem({
  className,
  toneClassName,
  hoverUnderlineEnabled = false,
  hoverUnderlineClassName,
  hoverBackgroundClassName,
  ...props
}: NavItemProps) {
  return (
    <Link
      className={cn(
        "relative rounded-md px-3 py-2 text-sm font-medium transition-colors hover:text-foreground",
        toneClassName,
        hoverBackgroundClassName,
        hoverUnderlineEnabled && "after:absolute after:bottom-1 after:left-3 after:right-3 after:h-0.5 after:rounded-full after:content-['']",
        hoverUnderlineEnabled && hoverUnderlineClassName,
        className,
      )}
      {...props}
    />
  )
}

export default NavItem
