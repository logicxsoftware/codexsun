import { useMemo } from "react"
import type { ReactNode } from "react"

import { cn } from "@/lib/utils"

export type NavWidthVariant = "container" | "full" | "boxed"

type NavContainerProps = {
  children: ReactNode
  variant?: NavWidthVariant
}

function NavContainer({ children, variant = "container" }: NavContainerProps) {
  const containerClassName = useMemo(() => {
    if (variant === "full") {
      return "w-full px-5"
    }

    if (variant === "boxed") {
      return "mx-auto w-full max-w-7xl px-5"
    }

    return "mx-auto w-full max-w-7xl px-3 sm:px-4 lg:px-6"
  }, [variant])

  const boxedClassName = variant === "boxed" ? "rounded-lg border border-border/60" : ""

  return <div className={cn(containerClassName, boxedClassName)}>{children}</div>
}

export default NavContainer
