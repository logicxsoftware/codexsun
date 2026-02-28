import type { ReactNode } from "react"

import { cn } from "@/lib/utils"

type NavContainerProps = {
  children: ReactNode
  variant: "full" | "container"
}

function NavContainer({ children, variant }: NavContainerProps) {
  return (
    <div className={cn("w-full px-3 sm:px-4 lg:px-6", variant === "container" ? "mx-auto max-w-7xl" : "mx-auto")}>
      {children}
    </div>
  )
}

export default NavContainer
