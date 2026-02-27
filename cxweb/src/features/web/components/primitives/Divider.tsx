import type { ComponentPropsWithoutRef } from "react"

import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"

type DividerProps = ComponentPropsWithoutRef<typeof Separator>

function Divider({ className, orientation = "vertical", decorative = true, ...props }: DividerProps) {
  return <Separator className={cn("bg-border/70", className)} orientation={orientation} decorative={decorative} {...props} />
}

export default Divider
