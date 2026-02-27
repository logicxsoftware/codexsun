import type { ComponentPropsWithoutRef } from "react"

import { cn } from "@/lib/utils"

type ContainerWidth = "fixed" | "full"

type ContainerPaddingScale = "compact" | "normal" | "relaxed"

type ContainerProps = ComponentPropsWithoutRef<"div"> & {
  width?: ContainerWidth
  paddingScale?: ContainerPaddingScale
}

const containerWidthClassMap: Record<ContainerWidth, string> = {
  fixed: "max-w-6xl",
  full: "max-w-none",
}

const containerPaddingClassMap: Record<ContainerPaddingScale, string> = {
  compact: "px-3 md:px-4",
  normal: "px-4 md:px-6",
  relaxed: "px-5 md:px-8",
}

function Container({ className, width = "fixed", paddingScale = "normal", ...props }: ContainerProps) {
  return <div className={cn("mx-auto w-full", containerWidthClassMap[width], containerPaddingClassMap[paddingScale], className)} {...props} />
}

export default Container
