import FadeUp from "@/components/animations/FadeUp"
import type { CallToActionSectionData } from "@/features/web/services/web-page-api"
import { Link } from "react-router"

type CallToActionSectionProps = {
  data: CallToActionSectionData
}

export default function CallToActionSection({ data }: CallToActionSectionProps) {
  const title = data.title?.trim() ?? ""
  const description = data.description?.trim() ?? ""
  const buttonText = data.buttonText?.trim() || data.label?.trim() || ""
  const buttonHref = data.buttonHref?.trim() || data.href?.trim() || ""

  if (title.length === 0 && description.length === 0 && (buttonText.length === 0 || buttonHref.length === 0)) {
    return null
  }

  return (
    <section className="bg-primary py-16 text-primary-foreground">
      <div className="mx-auto max-w-7xl px-5 text-center">
        <FadeUp>
          {title ? <h2 className="mb-4 text-2xl font-semibold md:text-3xl">{title}</h2> : null}
          {description ? <p className="mx-auto mb-6 max-w-2xl whitespace-pre-line text-primary-foreground/80">{description}</p> : null}
          {buttonText && buttonHref ? (
            <Link
              to={buttonHref}
              className="inline-block rounded-lg border border-primary-foreground/30 bg-primary-foreground px-6 py-3 font-medium text-primary transition hover:bg-primary-foreground/90"
            >
              {buttonText}
            </Link>
          ) : null}
        </FadeUp>
      </div>
    </section>
  )
}
