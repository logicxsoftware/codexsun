import FadeUp from "@/components/animations/FadeUp"
import type { AboutTestimonialData } from "@/features/web/services/about-page-api"
import { Star } from "lucide-react"

type TestimonialsSectionProps = {
  items: AboutTestimonialData[]
}

export default function TestimonialsSection({ items }: TestimonialsSectionProps) {
  const orderedItems = items.slice().sort((a, b) => a.order - b.order)

  if (orderedItems.length === 0) {
    return null
  }

  return (
    <section className="bg-muted py-20">
      <div className="mx-auto max-w-7xl px-5">
        <FadeUp>
          <div className="mx-auto mb-10 max-w-3xl text-center">
            <h2 className="text-3xl font-semibold text-foreground md:text-4xl">What Clients Say</h2>
          </div>
        </FadeUp>

        <div className="grid grid-cols-1 gap-6 md:grid-cols-2 lg:grid-cols-3">
          {orderedItems.map((item, index) => (
            <FadeUp key={`${item.order}-${item.name}`} delay={index * 0.05}>
              <article className="rounded-xl border border-border bg-card p-6 shadow-sm">
                <p className="text-base italic leading-relaxed text-muted-foreground">"{item.quote}"</p>
                <div className="mt-5">
                  <p className="text-base font-semibold text-foreground">{item.name}</p>
                  {item.company ? <p className="text-sm text-muted-foreground">{item.company}</p> : null}
                </div>
                {item.rating ? (
                  <div className="mt-4 flex items-center gap-1 text-primary">
                    {Array.from({ length: item.rating }).map((_, starIndex) => (
                      <Star key={`${item.order}-star-${starIndex}`} className="size-4 fill-current" />
                    ))}
                  </div>
                ) : null}
              </article>
            </FadeUp>
          ))}
        </div>
      </div>
    </section>
  )
}
