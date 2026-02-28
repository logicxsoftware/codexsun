import { Marquee } from "@/components/ui/marquee"
import type { BrandSliderSectionData } from "@/features/web/services/web-page-api"

type BrandSliderSectionProps = {
  data: BrandSliderSectionData
}

const normalizeLogoUrl = (logo: string): string => {
  const value = logo.trim()
  if (value.length === 0) {
    return ""
  }

  const normalized = value
    .replace(/^https?:\/\//i, "")
    .replace(/^\/\//, "")
    .replace(/^www\./i, "")

  // Handle raw domains like "hp.com" or "//hp.com" by resolving via logo proxy.
  if (/^[a-z0-9.-]+\.[a-z]{2,}\/?$/i.test(normalized)) {
    return `https://logo.clearbit.com/${normalized.replace(/\/$/, "")}`
  }

  if (value.startsWith("http://") || value.startsWith("https://")) {
    return value
  }

  if (value.startsWith("//")) {
    return `https:${value}`
  }

  const domain = value.replace(/^https?:\/\//, "").replace(/^www\./, "")
  return `https://logo.clearbit.com/${domain}`
}

const fallbackLogoDataUri = (name: string): string => {
  const safeName = name.trim().length > 0 ? name.trim() : "Brand"
  const svg = `<svg xmlns="http://www.w3.org/2000/svg" width="176" height="112" viewBox="0 0 176 112"><rect width="176" height="112" rx="14" fill="#f1f5f9"/><text x="88" y="61" text-anchor="middle" font-family="Segoe UI, Arial, sans-serif" font-size="15" fill="#334155">${safeName}</text></svg>`
  return `data:image/svg+xml;utf8,${encodeURIComponent(svg)}`
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
        logo: normalizeLogoUrl(item.logo),
        order: item.order ?? 0,
      }))
    : (data.brands ?? []).map((item) => ({
        name: item.name,
        logo: normalizeLogoUrl(item.logo ?? item.logoUrl ?? ""),
        order: item.order ?? 0,
      }))

  const logos = normalizedLogos
    .filter((item) => item.logo.trim().length > 0)
    .sort((a, b) => a.order - b.order)

  if (logos.length === 0) {
    return null
  }

  return (
    <section className="border-t border-border bg-background py-12 md:py-16">
      <div className="mx-auto max-w-7xl px-5">
        {heading ? <h2 className="mb-8 text-center text-xl font-semibold text-foreground md:text-2xl">{heading}</h2> : null}

        <div className="relative overflow-hidden">
          <div className="pointer-events-none absolute inset-y-0 left-0 z-10 w-16 bg-gradient-to-r from-background to-transparent" />
          <div className="pointer-events-none absolute inset-y-0 right-0 z-10 w-16 bg-gradient-to-l from-background to-transparent" />

          <Marquee pauseOnHover={pauseOnHover} repeat={3} className={durationClassBySeconds(data.animationDuration)}>
            {logos.map((brand) => (
              <div
                key={`${brand.order}-${brand.name}`}
                className="group flex h-28 w-44 items-center justify-center rounded-xl border border-border bg-muted p-4 transition-all duration-300"
              >
                <img
                  src={brand.logo}
                  alt={brand.name}
                  loading="lazy"
                  className="h-full w-full object-contain transition-all duration-500 group-hover:scale-110 group-hover:brightness-110"
                  onError={(event) => {
                    if (event.currentTarget.dataset.fallbackApplied === "1") {
                      return
                    }

                    event.currentTarget.dataset.fallbackApplied = "1"
                    event.currentTarget.src = fallbackLogoDataUri(brand.name)
                  }}
                />
              </div>
            ))}
          </Marquee>
        </div>
      </div>
    </section>
  )
}
