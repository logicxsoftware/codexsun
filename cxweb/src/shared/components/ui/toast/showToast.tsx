import { X } from "lucide-react"
import { toast } from "sonner"

import { cn } from "@/lib/utils"
import { toastVariantConfig } from "@/shared/components/ui/toast/toast.config"
import type { ShowToastOptions } from "@/shared/components/ui/toast/toast.types"

export const showToast = ({
  title,
  description,
  variant = "default",
  icon,
  actionLabel,
  onAction,
}: ShowToastOptions): string | number =>
  toast.custom(
    (id) => {
      const config = toastVariantConfig[variant]
      const Icon = config.icon

      const handleAction = async () => {
        if (onAction) {
          await Promise.resolve(onAction())
        }
        toast.dismiss(id)
      }

      return (
        <div
          aria-live="polite"
          className={cn(
            "group pointer-events-auto relative w-[calc(100vw-1.5rem)] max-w-sm overflow-hidden rounded-lg border shadow-lg shadow-black/10 transition-all",
            config.containerClassName,
          )}
          role="status"
        >
          <div className="flex items-start gap-3 p-4 pr-10">
            <div className={cn("mt-0.5 shrink-0", config.iconClassName)}>{icon ?? <Icon className="h-5 w-5" />}</div>
            <div className="min-w-0 flex-1 space-y-1">
              <p className="text-sm font-semibold leading-5">{title}</p>
              {description ? <p className="text-xs leading-5 opacity-90">{description}</p> : null}
              {actionLabel && onAction ? (
                <button
                  className="mt-2 inline-flex h-8 items-center rounded-md border border-border bg-background px-3 text-xs font-medium text-foreground transition-colors hover:bg-menu-hover"
                  onClick={() => {
                    void handleAction()
                  }}
                  type="button"
                >
                  {actionLabel}
                </button>
              ) : null}
            </div>
          </div>
          <button
            aria-label="Dismiss notification"
            className="absolute right-2 top-2 inline-flex h-7 w-7 items-center justify-center rounded-md text-muted-foreground transition-colors hover:bg-menu-hover hover:text-foreground"
            onClick={() => {
              toast.dismiss(id)
            }}
            type="button"
          >
            <X className="h-4 w-4" />
          </button>
        </div>
      )
    },
    {
      duration: 4500,
    },
  )
