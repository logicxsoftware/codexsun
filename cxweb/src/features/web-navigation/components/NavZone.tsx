import type { ReactNode } from "react"

import { cn } from "@/lib/utils"

type NavZoneProps = {
  children: ReactNode
  justify?: "start" | "center" | "end"
  className?: string
}

function NavZone({ children, justify = "start", className }: NavZoneProps) {
  return (
    <div
      className={cn(
        "flex min-w-0 items-center gap-1.5 md:gap-2",
        justify === "center" ? "justify-center" : justify === "end" ? "justify-end" : "justify-start",
        className,
      )}
    >
      {children}
    </div>
  )
}

export default NavZone
