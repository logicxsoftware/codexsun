import type { ComponentPropsWithoutRef } from "react"

import { cn } from "@/lib/utils"

type NavListProps = ComponentPropsWithoutRef<"nav">

function NavList({ className, ...props }: NavListProps) {
  return <nav className={cn("hidden items-center gap-1 md:flex", className)} {...props} />
}

export default NavList
