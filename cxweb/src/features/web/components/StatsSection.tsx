import { cn } from "@/lib/utils"
import Counter from "@/features/web/components/Counter"
import { resolveBackgroundClass, resolveBorderClass } from "@/features/web-navigation/utils/token-class"
import type { StatsSectionData } from "@/features/web/services/web-page-api"

type StatsSectionProps = {
  data?: StatsSectionData | null
}

const toNumericValue = (value: string | number): number => {
  if (typeof value === "number") {
    return Number.isFinite(value) ? Math.floor(value) : 0
  }

  const parsed = Number.parseInt(value.replace(/[^\d]/g, ""), 10)
  return Number.isFinite(parsed) ? parsed : 0
}

export default function StatsSection({ data }: StatsSectionProps) {
  if (!data) {
    return null
  }

  const items = (data.stats ?? data.items ?? []).slice().sort((a, b) => (a.order ?? 0) - (b.order ?? 0))
  if (items.length === 0) {
    return null
  }

  const backgroundClass = resolveBackgroundClass(data.backgroundToken ?? "background", "bg-background")
  const borderClass = resolveBorderClass(data.borderToken ?? "border", "border-border")

  return (
    <section className={cn("w-full border-y py-16 md:py-20", backgroundClass, borderClass)}>
      <div className="mx-auto w-full max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-2 gap-8 md:grid-cols-4 lg:gap-12">
          {items.map((stat, index) => (
            <div key={`${stat.label}-${index}`} className="text-center">
              <Counter
                value={toNumericValue(stat.value)}
                suffix={stat.suffix ?? ""}
                className="text-4xl font-bold text-foreground/80 md:text-5xl lg:text-6xl"
              />
              <p className="mt-3 text-base font-medium text-muted-foreground/70 md:text-lg">{stat.label}</p>
            </div>
          ))}
        </div>
      </div>
    </section>
  )
}
