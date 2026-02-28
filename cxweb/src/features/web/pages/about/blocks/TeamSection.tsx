import FadeUp from "@/components/animations/FadeUp"
import type { TeamMemberData } from "@/features/web/services/about-page-api"

type TeamSectionProps = {
  members: TeamMemberData[]
}

const fallbackImage = "https://images.unsplash.com/photo-1521737604893-d14cc237f11d?w=400&q=80&auto=format&fit=crop"

export default function TeamSection({ members }: TeamSectionProps) {
  const orderedMembers = members.slice().sort((a, b) => a.order - b.order)

  if (orderedMembers.length === 0) {
    return null
  }

  return (
    <section className="bg-background py-20">
      <div className="mx-auto max-w-7xl px-5">
        <FadeUp>
          <div className="mx-auto mb-12 max-w-3xl text-center">
            <h2 className="text-3xl font-semibold text-foreground md:text-4xl">Meet Our Team</h2>
            <p className="mt-3 text-muted-foreground">Experienced professionals focused on practical outcomes and dependable support.</p>
          </div>
        </FadeUp>

        <div className="grid grid-cols-1 gap-6 sm:grid-cols-2 lg:grid-cols-3">
          {orderedMembers.map((member, index) => (
            <FadeUp key={`${member.order}-${member.name}`} delay={index * 0.05}>
              <article className="rounded-xl border border-border bg-card p-6 text-center shadow-sm transition-shadow duration-300 hover:shadow-md">
                <img
                  src={member.image.trim().length > 0 ? member.image : fallbackImage}
                  alt={member.name}
                  loading="lazy"
                  className="mx-auto h-32 w-32 rounded-full object-cover"
                />
                <h3 className="mt-5 text-xl font-semibold text-foreground">{member.name}</h3>
                <p className="mt-1 text-sm text-primary">{member.role}</p>
                <p className="mt-4 text-sm leading-relaxed text-muted-foreground">{member.bio}</p>
              </article>
            </FadeUp>
          ))}
        </div>
      </div>
    </section>
  )
}
