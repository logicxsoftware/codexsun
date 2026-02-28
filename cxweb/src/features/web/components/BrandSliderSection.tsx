import { Marquee } from "@/components/ui/marquee"
import type { BrandSliderSectionData } from "@/features/web/services/web-page-api"

type BrandSliderSectionProps = {
  data: BrandSliderSectionData
}

const durationClassBySeconds = (duration?: number): string => {
  if (duration === 30) {
    return "[--duration:30s]"
  }

  if (duration === 50) {
    return "[--duration:50s]"
  }

  return "[--duration:40s]"
}

export default function BrandSliderSection({ data }: BrandSliderSectionProps) {
  const heading = data.heading?.trim() ?? ""
  const pauseOnHover = data.pauseOnHover ?? true
  const normalizedLogos = data.logos
    ? data.logos.map((item) => ({
        name: item.name,
        logo: item.logo,
        order: item.order ?? 0,
      }))
    : (data.brands ?? []).map((item) => ({
        name: item.name,
        logo: item.logo ?? item.logoUrl ?? "",
        order: item.order ?? 0,
      }))

  const logos = normalizedLogos
    .filter((item) => item.logo.trim().length > 0)
    .sort((a, b) => a.order - b.order)

  if (logos.length === 0) {
    return null
  }

  const duplicated = [...logos, ...logos]

  return (
    <section className="border-t border-border bg-background py-12 md:py-16">
      <div className="mx-auto max-w-7xl px-5">
        {heading ? <h2 className="mb-8 text-center text-xl font-semibold text-foreground md:text-2xl">{heading}</h2> : null}

        <div className="relative overflow-hidden">
          <div className="pointer-events-none absolute inset-y-0 left-0 z-10 w-16 bg-gradient-to-r from-background to-transparent" />
          <div className="pointer-events-none absolute inset-y-0 right-0 z-10 w-16 bg-gradient-to-l from-background to-transparent" />

          <Marquee pauseOnHover={pauseOnHover} repeat={1} className={durationClassBySeconds(data.animationDuration)}>
            {duplicated.map((brand, index) => (
              <div
                key={`${brand.name}-${index}`}
                className="group flex h-28 w-44 items-center justify-center rounded-xl border border-border bg-muted p-4 transition-all duration-300"
              >
                <img
                  src={brand.logo}
                  alt={brand.name}
                  loading="lazy"
                  className="h-full w-full object-contain transition-all duration-500 group-hover:scale-110 group-hover:brightness-110"
                />
              </div>
            ))}
          </Marquee>
        </div>
      </div>
    </section>
  )
}
