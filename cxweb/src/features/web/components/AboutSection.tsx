import FadeUp from "@/components/animations/FadeUp"
import type { AboutSectionData } from "@/features/web/services/web-page-api"

type AboutSectionProps = {
  data: AboutSectionData
}

function AboutSection({ data }: AboutSectionProps) {
  const safeTitle = data.title?.trim() || "About"
  const safeSubtitle = data.subtitle?.trim() || "Reliable technology solutions tailored for homes and businesses"
  const safeContent = data.content.length > 0 ? data.content : ["Information will be available soon."]
  const safeImageSrc = data.image.src?.trim() || "https://images.unsplash.com/photo-1518770660439-4636190af475?w=1200&q=80&auto=format&fit=crop"
  const safeImageAlt = data.image.alt?.trim() || "Team delivering IT support and services"

  return (
    <section className="w-full bg-muted">
      <div className="mx-auto max-w-7xl px-5 py-12 md:py-16">
        <div className="grid grid-cols-1 items-center gap-8 md:grid-cols-2 md:gap-12">
          <FadeUp>
            <div className="text-center md:text-left">
              <h2 className="text-3xl font-bold text-foreground md:text-4xl lg:text-5xl">{safeTitle}</h2>
              <p className="mt-4 text-xl font-medium text-muted-foreground">{safeSubtitle}</p>
              <div className="mt-6 space-y-4">
                {safeContent.map((line, index) => (
                  <p key={`${line}-${index}`} className="text-base leading-relaxed text-muted-foreground md:text-lg">
                    {line}
                  </p>
                ))}
              </div>
            </div>
          </FadeUp>

          <FadeUp delay={0.2}>
            <div className="relative overflow-hidden rounded-2xl bg-background shadow-2xl ring-1 ring-border">
              <img
                src={safeImageSrc}
                alt={safeImageAlt}
                className="h-full w-full object-cover transition-transform duration-500 hover:scale-105"
                loading="lazy"
              />
              <div className="pointer-events-none absolute inset-0 bg-gradient-to-t from-background/60 via-background/10 to-transparent" />
            </div>
          </FadeUp>
        </div>
      </div>
    </section>
  )
}

export default AboutSection
