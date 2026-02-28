import FadeUp from "@/components/animations/FadeUp"
import type { HeroSectionData } from "@/features/web/services/web-page-api"

type HeroSectionProps = {
  data: HeroSectionData
}

export default function HeroSection({ data }: HeroSectionProps) {
  const safeTitle = data.title?.trim() || "Welcome"
  const safeSubtitle = data.subtitle?.trim() || "Reliable technology solutions tailored for your business."

  return (
    <section className="w-full py-6 md:py-12 lg:py-16">
      <div className="mx-auto max-w-5xl px-5 text-center">
        <FadeUp>
          <h1 className="mb-6 wrap-break-word text-3xl font-bold leading-tight text-foreground md:text-4xl lg:text-5xl">{safeTitle}</h1>
        </FadeUp>
        <FadeUp delay={0.1}>
          <p className="mb-8 wrap-break-word text-lg leading-relaxed text-muted-foreground md:text-xl">{safeSubtitle}</p>
        </FadeUp>
        <FadeUp delay={0.15}>
          <div className="mx-auto mt-6 h-1 w-20 rounded-full bg-primary" />
        </FadeUp>
      </div>
    </section>
  )
}
