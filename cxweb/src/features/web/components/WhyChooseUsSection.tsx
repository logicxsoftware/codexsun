import FadeUp from "@/components/animations/FadeUp"
import type { WhyChooseUsSectionData } from "@/features/web/services/web-page-api"
import { BadgeCheck, Factory, IndianRupee, ShieldCheck, Truck, Users } from "lucide-react"

type WhyChooseUsSectionProps = {
  data: WhyChooseUsSectionData
}

const iconMap = {
  BadgeCheck,
  IndianRupee,
  Factory,
  Truck,
  ShieldCheck,
  Users,
} as const

export default function WhyChooseUsSection({ data }: WhyChooseUsSectionProps) {
  const heading = data.heading?.trim() ?? ""
  const subheading = data.subheading?.trim() ?? ""
  const items = data.items.slice().sort((a, b) => (a.order ?? 0) - (b.order ?? 0))

  if (heading.length === 0 && subheading.length === 0 && items.length === 0) {
    return null
  }

  return (
    <section className="bg-muted/70 py-20 md:py-24 lg:py-28">
      <div className="mx-auto max-w-7xl px-5">
        <FadeUp>
          <div className="mx-auto mb-12 max-w-3xl text-center">
            {heading ? <h2 className="text-3xl font-bold text-foreground md:text-4xl">{heading}</h2> : null}
            {subheading ? <p className="mt-4 text-lg text-muted-foreground">{subheading}</p> : null}
          </div>
        </FadeUp>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {items.map((item, index) => {
            const Icon = iconMap[item.icon as keyof typeof iconMap] ?? BadgeCheck
            return (
              <FadeUp key={`${item.title}-${index}`} delay={index * 0.05}>
                <article className="group rounded-2xl border border-border bg-card p-8 shadow-sm transition-all duration-300 hover:-translate-y-2 hover:shadow-lg">
                  <div className="mb-5 inline-flex rounded-xl bg-primary/10 p-3 text-primary transition-colors duration-300 group-hover:bg-primary group-hover:text-primary-foreground">
                    <Icon className="size-6" aria-hidden />
                  </div>
                  <h3 className="text-xl font-semibold text-foreground">{item.title}</h3>
                  <p className="mt-3 text-base leading-relaxed text-muted-foreground">{item.description}</p>
                </article>
              </FadeUp>
            )
          })}
        </div>
      </div>
    </section>
  )
}

