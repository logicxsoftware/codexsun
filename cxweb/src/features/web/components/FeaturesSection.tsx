import FadeUpStagger, { FadeUpItem } from "@/components/animations/FadeUpStagger"
import type { FeaturesSectionData } from "@/features/web/services/web-page-api"
import { CheckCircle2 } from "lucide-react"

type FeaturesSectionProps = {
  data: FeaturesSectionData
}

const fallbackImageSrc = "https://images.unsplash.com/photo-1593642532973-d31b6557fa68"

export default function FeaturesSection({ data }: FeaturesSectionProps) {
  const title = data.title?.trim() ?? ""
  const description = data.description?.trim() ?? ""
  const imageSrc = data.imageSrc?.trim() || fallbackImageSrc
  const imageAlt = data.imageAlt?.trim() || "Performance workstation and laptop setup"

  const bullets = (data.bullets ?? [])
    .filter((item) => item.text.trim().length > 0)
    .sort((a, b) => (a.order ?? 0) - (b.order ?? 0))

  const legacyFallbackBullets = (data.items ?? [])
    .map((item, index) => ({
      text: item.title.trim(),
      order: index,
    }))
    .filter((item) => item.text.length > 0)

  const bulletItems = bullets.length > 0 ? bullets : legacyFallbackBullets

  if (title.length === 0 && description.length === 0 && bulletItems.length === 0) {
    return null
  }

  return (
    <section className="bg-muted py-16 md:py-20 lg:py-24">
      <div className="mx-auto max-w-7xl px-5">
        <FadeUpStagger className="grid grid-cols-1 items-center gap-12 lg:grid-cols-2 lg:gap-16" staggerChildren={0.2} delay={0.05}>
          <FadeUpItem>
            <FadeUpStagger staggerChildren={0.12} delay={0.04}>
              <FadeUpItem>
                {title ? <h2 className="text-4xl font-bold tracking-tighter drop-shadow-lg text-foreground md:text-5xl lg:text-6xl">{title}</h2> : null}
              </FadeUpItem>

              <FadeUpItem>
                {description ? <p className="py-3 whitespace-pre-line text-lg leading-relaxed text-muted-foreground">{description}</p> : null}
              </FadeUpItem>

              {bulletItems.length > 0 ? (
                <ul className="mt-6 space-y-4">
                  {bulletItems.map((item, index) => (
                    <FadeUpItem key={`${item.order ?? index}-${item.text}`}>
                      <li className="flex items-start gap-3">
                        <CheckCircle2 className="mt-0.5 size-6 shrink-0 text-primary" aria-hidden />
                        <span className="text-lg text-muted-foreground">{item.text}</span>
                      </li>
                    </FadeUpItem>
                  ))}
                </ul>
              ) : null}
            </FadeUpStagger>
          </FadeUpItem>

          <FadeUpItem>
            <div className="relative mx-auto max-w-md lg:max-w-xl">
              <img
                src={imageSrc}
                alt={imageAlt}
                loading="lazy"
                className="h-auto w-full object-contain transition-transform duration-700 hover:scale-105"
              />
            </div>
          </FadeUpItem>
        </FadeUpStagger>
      </div>
    </section>
  )
}
