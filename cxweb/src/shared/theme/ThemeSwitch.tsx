import { LaptopMinimal, Moon, Sun } from "lucide-react"

import { Button } from "@/components/ui/button"
import { cn } from "@/lib/utils"
import { useTenantTheme } from "@/shared/theme/TenantThemeProvider"
import type { ThemePreference } from "@/shared/theme/tokens"

const modeOptions: Array<{ mode: ThemePreference; label: string; icon: typeof Sun }> = [
  { mode: "light", label: "Light", icon: Sun },
  { mode: "dark", label: "Dark", icon: Moon },
  { mode: "system", label: "System", icon: LaptopMinimal },
]

export function ThemeSwitch() {
  const { modePreference, setModePreference } = useTenantTheme()

  return (
    <div className="inline-flex items-center gap-1 rounded-md border border-border/70 bg-card/60 p-1">
      {modeOptions.map((option) => {
        const Icon = option.icon
        const active = modePreference === option.mode

        return (
          <Button
            key={option.mode}
            type="button"
            variant="ghost"
            size="sm"
            className={cn(
              "h-8 gap-1.5 px-2 text-xs",
              active ? "bg-menu-hover text-foreground" : "text-muted-foreground hover:bg-menu-hover hover:text-foreground",
            )}
            onClick={() => {
              setModePreference(option.mode)
            }}
            aria-pressed={active}
            aria-label={`Switch theme to ${option.label}`}
          >
            <Icon className="h-3.5 w-3.5" />
            <span className="hidden lg:inline">{option.label}</span>
          </Button>
        )
      })}
    </div>
  )
}
