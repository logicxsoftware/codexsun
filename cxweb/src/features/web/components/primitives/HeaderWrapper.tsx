import type { ComponentPropsWithoutRef } from "react"

import { cn } from "@/lib/utils"

type HeaderWrapperProps = ComponentPropsWithoutRef<"header">

function HeaderWrapper({ className, ...props }: HeaderWrapperProps) {
  return <header className={cn("backdrop-blur supports-backdrop-filter:bg-header-bg/80", className)} {...props} />
}

export default HeaderWrapper
