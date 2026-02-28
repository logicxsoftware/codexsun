import type { FooterHoursItem } from "@/features/web-navigation/types/navigation-types"

type FooterBusinessHoursProps = {
  enabled: boolean
  items: FooterHoursItem[]
}

function FooterBusinessHours({ enabled, items }: FooterBusinessHoursProps) {
  if (!enabled || items.length === 0) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-foreground">Business Hours</h3>
      <div className="grid gap-1">
        {items.map((item) => (
          <div key={`${item.day}-${item.hours}`} className="flex items-center justify-between gap-2 text-sm text-muted-foreground">
            <span>{item.day}</span>
            <span>{item.hours}</span>
          </div>
        ))}
      </div>
    </section>
  )
}

export default FooterBusinessHours
