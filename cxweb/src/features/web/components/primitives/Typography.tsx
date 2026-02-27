import type { ComponentPropsWithoutRef } from "react"

import { cn } from "@/lib/utils"

type HeaderTitleProps = ComponentPropsWithoutRef<"span">

type HeaderCaptionProps = ComponentPropsWithoutRef<"span">

function HeaderTitle({ className, ...props }: HeaderTitleProps) {
  return <span className={cn("text-base font-semibold tracking-tight text-foreground", className)} {...props} />
}

function HeaderCaption({ className, ...props }: HeaderCaptionProps) {
  return <span className={cn("text-sm text-muted-foreground", className)} {...props} />
}

export { HeaderCaption, HeaderTitle }
