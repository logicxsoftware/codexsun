import FadeUpStagger, { FadeUpItem } from "@/components/animations/FadeUpStagger"
import type { RoadmapMilestoneData } from "@/features/web/services/about-page-api"

type RoadmapSectionProps = {
  milestones: RoadmapMilestoneData[]
}

export default function RoadmapSection({ milestones }: RoadmapSectionProps) {
  const orderedMilestones = milestones.slice().sort((a, b) => a.order - b.order)

  if (orderedMilestones.length === 0) {
    return null
  }

  return (
    <section className="bg-background py-20">
      <div className="mx-auto max-w-5xl px-5">
        <FadeUpStagger staggerChildren={0.1}>
          <FadeUpItem>
            <div className="mb-10 text-center">
              <h2 className="text-3xl font-semibold text-foreground md:text-4xl">Company Progress</h2>
            </div>
          </FadeUpItem>
          <FadeUpItem>
            <div className="relative ml-3 border-l border-border pl-7 md:ml-6 md:pl-10">
              {orderedMilestones.map((milestone) => (
                <FadeUpItem key={`${milestone.order}-${milestone.year}`}>
                  <article className="relative mb-6 rounded-xl border border-border bg-card p-6">
                    <span className="absolute -left-[2.18rem] top-8 block h-4 w-4 rounded-full bg-primary md:-left-[2.95rem]" />
                    <p className="text-sm font-semibold text-primary">{milestone.year}</p>
                    <h3 className="mt-2 text-xl font-semibold text-foreground">{milestone.title}</h3>
                    <p className="mt-2 text-sm leading-relaxed text-muted-foreground">{milestone.description}</p>
                  </article>
                </FadeUpItem>
              ))}
            </div>
          </FadeUpItem>
        </FadeUpStagger>
      </div>
    </section>
  )
}
