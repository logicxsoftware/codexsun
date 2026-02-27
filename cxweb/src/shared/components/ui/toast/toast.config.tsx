import type { LucideIcon } from "lucide-react"
import { BadgeCheck, Bell, Bug, CircleAlert, CircleCheckBig, MoonStar, PartyPopper, Sun } from "lucide-react"

import type { ToastVariant } from "@/shared/components/ui/toast/toast.types"

type ToastVariantConfig = {
  containerClassName: string
  iconClassName: string
  icon: LucideIcon
}

export const toastVariantConfig: Record<ToastVariant, ToastVariantConfig> = {
  success: {
    containerClassName: "border-primary/40 bg-card text-card-foreground",
    iconClassName: "text-primary",
    icon: CircleCheckBig,
  },
  error: {
    containerClassName: "border-destructive/45 bg-card text-card-foreground",
    iconClassName: "text-destructive",
    icon: CircleAlert,
  },
  warning: {
    containerClassName: "border-chart-5/55 bg-card text-card-foreground",
    iconClassName: "text-chart-5",
    icon: CircleAlert,
  },
  info: {
    containerClassName: "border-chart-2/45 bg-card text-card-foreground",
    iconClassName: "text-chart-2",
    icon: Bell,
  },
  bug: {
    containerClassName: "border-chart-3/45 bg-card text-card-foreground",
    iconClassName: "text-chart-3",
    icon: Bug,
  },
  happy: {
    containerClassName: "border-chart-4/45 bg-card text-card-foreground",
    iconClassName: "text-chart-4",
    icon: PartyPopper,
  },
  light: {
    containerClassName: "border-border bg-background text-foreground",
    iconClassName: "text-brand",
    icon: Sun,
  },
  dark: {
    containerClassName: "border-border bg-foreground text-background",
    iconClassName: "text-background",
    icon: MoonStar,
  },
  default: {
    containerClassName: "border-border bg-card text-card-foreground",
    iconClassName: "text-brand",
    icon: BadgeCheck,
  },
}
