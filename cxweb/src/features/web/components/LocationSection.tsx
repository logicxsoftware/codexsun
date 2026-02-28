import FadeUp from "@/components/animations/FadeUp"
import type { LocationSectionData } from "@/features/web/services/web-page-api"
import { Link } from "react-router"

type LocationSectionProps = {
  data: LocationSectionData
}

const fallbackImageSrc = "https://images.unsplash.com/photo-1517336714731-489689fd1ca8"

const splitLines = (value?: string): string[] =>
  (value ?? "")
    .split("\n")
    .map((line) => line.trim())
    .filter((line) => line.length > 0)

export default function LocationSection({ data }: LocationSectionProps) {
  const displayName = data.displayName?.trim() ?? ""
  const title = data.title?.trim() ?? ""
  const buttonText = data.buttonText?.trim() ?? ""
  const buttonHref = data.buttonHref?.trim() ?? ""
  const imageSrc = data.imageSrc?.trim() || fallbackImageSrc
  const imageAlt = data.imageAlt?.trim() || "Storefront and customer service desk"
  const mapEmbedUrl = data.mapEmbedUrl?.trim() ?? ""
  const mapTitle = data.mapTitle?.trim() || "Store location map"

  const addressLines = splitLines(data.address)
  const timingItems = (data.timings ?? [])
    .filter((item) => item.day.trim().length > 0 || item.hours.trim().length > 0)
    .sort((a, b) => (a.order ?? 0) - (b.order ?? 0))
  const hasContact = (data.contact?.phone?.trim()?.length ?? 0) > 0 || (data.contact?.email?.trim()?.length ?? 0) > 0

  if (displayName.length === 0 && title.length === 0 && addressLines.length === 0 && timingItems.length === 0 && !hasContact && mapEmbedUrl.length === 0) {
    return null
  }

  return (
    <section className="bg-background py-20 md:py-24">
      <div className="mx-auto max-w-7xl px-5">
        <div className="grid grid-cols-1 gap-10 lg:grid-cols-2 lg:gap-14">
          <FadeUp>
            <div className="space-y-6">
              {title ? <h2 className="mb-6 font-serif text-3xl text-foreground sm:text-4xl">{title}</h2> : null}

              {displayName || addressLines.length > 0 ? (
                <p className="mb-6 leading-relaxed text-muted-foreground">
                  {displayName ? <strong className="text-xl font-semibold text-foreground sm:text-2xl">{displayName}</strong> : null}
                  {displayName ? <br /> : null}
                  {addressLines.map((line, index) => (
                    <span key={`${line}-${index}`}>
                      {line}
                      <br />
                    </span>
                  ))}
                </p>
              ) : null}

              {timingItems.length > 0 ? (
                <div className="mb-6 space-y-1.5">
                  {timingItems.map((item, index) => (
                    <p key={`${item.order ?? index}-${item.day}-${item.hours}`} className="text-sm text-muted-foreground">
                      {item.day}: {item.hours}
                    </p>
                  ))}
                </div>
              ) : null}

              {hasContact ? (
                <div className="mb-8 space-y-1 text-sm text-muted-foreground">
                  {data.contact?.phone?.trim() ? <p>Phone: {data.contact.phone.trim()}</p> : null}
                  {data.contact?.email?.trim() ? <p>Email: {data.contact.email.trim()}</p> : null}
                </div>
              ) : null}

              {buttonText && buttonHref ? (
                <Link to={buttonHref} className="inline-block rounded-lg bg-primary px-6 py-3 text-primary-foreground transition hover:bg-primary/80">
                  {buttonText}
                </Link>
              ) : null}
            </div>
          </FadeUp>

          <FadeUp delay={0.1}>
            <div className="relative">
              <img
                src={imageSrc}
                alt={imageAlt}
                loading="lazy"
                className="h-56 w-full max-w-sm rounded-xl object-cover md:h-64 lg:max-w-md"
              />
              {mapEmbedUrl ? (
                <div className="mt-4 rounded-xl border border-border bg-card p-3 shadow-xl lg:absolute lg:-bottom-10 lg:left-4 lg:mt-0 lg:w-[78%]">
                  <iframe
                    src={mapEmbedUrl}
                    title={mapTitle}
                    loading="lazy"
                    referrerPolicy="no-referrer-when-downgrade"
                    className="h-52 w-full rounded-lg border-0"
                    allowFullScreen
                  />
                </div>
              ) : null}
            </div>
          </FadeUp>
        </div>
      </div>
    </section>
  )
}
