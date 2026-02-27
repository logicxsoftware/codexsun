import type { ReactNode } from "react"

export const toastVariants = [
  "success",
  "error",
  "warning",
  "info",
  "bug",
  "happy",
  "light",
  "dark",
  "default",
] as const

export type ToastVariant = (typeof toastVariants)[number]

export type ShowToastOptions = {
  title: string
  description?: string
  variant?: ToastVariant
  icon?: ReactNode
  actionLabel?: string
  onAction?: () => void | Promise<void>
}
