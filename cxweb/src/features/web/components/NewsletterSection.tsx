import FadeUp from "@/components/animations/FadeUp"
import type { NewsletterSectionData } from "@/features/web/services/web-page-api"

type NewsletterSectionProps = {
  data: NewsletterSectionData
}

const fallbackImageSrc = "https://images.unsplash.com/photo-1587202372775-e229f172b9d7"

export default function NewsletterSection({ data }: NewsletterSectionProps) {
  const title = data.title?.trim() ?? ""
  const description = data.description?.trim() || data.subtitle?.trim() || ""
  const placeholderName = data.placeholderName?.trim() || "Your Name"
  const placeholderEmail = data.placeholderEmail?.trim() || data.placeholder?.trim() || "Your Email"
  const buttonText = data.buttonText?.trim() || data.buttonLabel?.trim() || "Subscribe Now"
  const trustNote = data.trustNote?.trim() || "We respect your privacy. No spam. Only useful tech updates & offers."
  const imageSrc = data.imageSrc?.trim() || data.image?.trim() || fallbackImageSrc
  const imageAlt = data.imageAlt?.trim() || "Technology workstation and components"

  if (title.length === 0 && description.length === 0) {
    return null
  }

  return (
    <section className="relative overflow-hidden bg-newsletter-bg pb-0 pt-16 md:pb-0 md:pt-20">
      <div className="mx-auto max-w-7xl px-5">
        <FadeUp delay={0.02}>
          <div className="mb-8 flex justify-center opacity-90">
            <img
              src={imageSrc}
              alt={imageAlt}
              loading="lazy"
              className="h-36 w-full max-w-xs rounded-xl object-cover sm:h-40 sm:max-w-sm"
            />
          </div>
        </FadeUp>

        <FadeUp>
          <div className="mx-auto max-w-3xl text-center">
            {title ? <h2 className="text-3xl font-semibold text-foreground md:text-4xl">{title}</h2> : null}
            {description ? <p className="mt-4 whitespace-pre-line text-sm leading-relaxed text-muted-foreground">{description}</p> : null}
          </div>
        </FadeUp>

        <FadeUp delay={0.06}>
          <div className="mx-auto mt-10 grid max-w-3xl gap-5 bg-transparent py-16">
            <div className="grid gap-4 sm:grid-cols-2">
              <label className="grid gap-2">
                <span className="text-xs text-muted-foreground">Name</span>
                <input
                  type="text"
                  placeholder={placeholderName}
                  className="w-full border-b-2 border-black bg-transparent py-2.5 text-foreground placeholder:text-muted-foreground transition-colors focus:border-black focus:outline-none"
                />
              </label>
              <label className="grid gap-2">
                <span className="text-xs text-muted-foreground">Email</span>
                <input
                  type="email"
                  placeholder={placeholderEmail}
                  className="w-full border-b-2 border-black bg-transparent py-2.5 text-foreground placeholder:text-muted-foreground transition-colors focus:border-black focus:outline-none"
                />
              </label>
            </div>

            <div className="flex justify-center">
              <button
                type="button"
                className="inline-flex items-center justify-center rounded-full bg-red-600 px-8 py-3 text-sm font-medium text-white transition hover:-translate-y-0.5 hover:bg-red-700 sm:px-10"
              >
                {buttonText}
              </button>
            </div>

            {trustNote ? <p className="text-center text-xs text-muted-foreground">{trustNote}</p> : null}
          </div>
        </FadeUp>
      </div>
    </section>
  )
}
