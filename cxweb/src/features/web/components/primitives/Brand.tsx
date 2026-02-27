import type { ComponentPropsWithoutRef, ReactNode } from "react"
import { Link } from "react-router"

import { cn } from "@/lib/utils"

type BrandProps = Omit<ComponentPropsWithoutRef<typeof Link>, "children"> & {
  logoPath: string
  logoFillClassName?: string
  logoTextClassName?: string
  logoClassName?: string
  textClassName?: string
  children: ReactNode
}

function Brand({
  className,
  logoPath,
  logoFillClassName,
  logoTextClassName,
  logoClassName,
  textClassName,
  children,
  ...props
}: BrandProps) {
  return (
    <Link className={cn("flex items-center gap-3", className)} {...props}>
      <span className={cn("inline-flex h-8 w-8 items-center justify-center rounded-md", logoClassName)} aria-hidden="true">
        <svg viewBox="0 0 24 24" className={cn("h-5 w-5", logoFillClassName)}>
          <path d={logoPath} className={cn("transition-colors", logoFillClassName)} />
        </svg>
      </span>
      <span className={cn("inline-flex items-center text-base font-semibold tracking-tight", logoTextClassName, textClassName)}>
        {children}
      </span>
    </Link>
  )
}

export default Brand
