import { useEffect, useMemo, useState } from "react"
import type { FormEvent } from "react"

import FadeUp from "@/components/animations/FadeUp"
import HeroSection from "@/features/web/components/HeroSection"
import { contactPageApi, type ContactPageResponse } from "@/features/web/services/contact-page-api"
import { HttpError } from "@/shared/services/http-client"
import { showToast } from "@/shared/components/ui/toast"

type ContactFormState = {
  name: string
  email: string
  subject: string
  message: string
}

const fallbackData: ContactPageResponse = {
  hero: {
    title: "",
    subtitle: "",
  },
  location: {
    displayName: "",
    title: "",
    address: "",
    buttonText: "Send Message",
    buttonHref: "",
    imageSrc: "https://images.unsplash.com/photo-1581091870627-3b5de9e1e6b6",
    imageAlt: "Storefront and customer support desk",
    imageClassName: "h-[340px] w-full rounded-xl object-cover md:h-[420px]",
    mapEmbedUrl: "",
    mapTitle: "Company location",
    timings: [],
    contact: {
      phone: "",
      email: "",
    },
  },
}

const initialFormState: ContactFormState = {
  name: "",
  email: "",
  subject: "",
  message: "",
}

const sanitizePhoneForTel = (raw: string): string => raw.replace(/[^\d+]/g, "")
const sanitizePhoneForWhatsApp = (raw: string): string => raw.replace(/\D/g, "")

export default function ContactPage() {
  const [data, setData] = useState<ContactPageResponse>(fallbackData)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [submitting, setSubmitting] = useState(false)
  const [form, setForm] = useState<ContactFormState>(initialFormState)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        const response = await contactPageApi.get()
        if (!mounted) {
          return
        }

        setData(response)
        setError(null)
        if (typeof document !== "undefined") {
          document.title = "Contact"
        }
      } catch (err) {
        if (!mounted) {
          return
        }

        if (err instanceof HttpError && err.status === 404) {
          setError("Contact page data is not available.")
        } else {
          setError("Unable to load contact page.")
        }
      } finally {
        if (mounted) {
          setLoading(false)
        }
      }
    }

    void load()

    return () => {
      mounted = false
    }
  }, [])

  const addressLines = useMemo(() => (data.location.address ?? "").split("\n").map((line) => line.trim()).filter((line) => line.length > 0), [data.location.address])

  const timingItems = useMemo(
    () => [...(data.location.timings ?? [])].sort((a, b) => (a.order ?? 0) - (b.order ?? 0)),
    [data.location.timings],
  )

  const phoneRaw = data.location.contact?.phone?.trim() ?? ""
  const phoneHref = phoneRaw.length > 0 ? `tel:${sanitizePhoneForTel(phoneRaw)}` : ""
  const whatsappHref = phoneRaw.length > 0 ? `https://wa.me/${sanitizePhoneForWhatsApp(phoneRaw)}` : ""
  const email = data.location.contact?.email?.trim() ?? ""
  const mapEmbedUrl = data.location.mapEmbedUrl?.trim() ?? ""

  const onSubmit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    if (submitting) {
      return
    }

    setSubmitting(true)
    try {
      await contactPageApi.submit({
        name: form.name,
        email: form.email,
        subject: form.subject,
        message: form.message,
      })
      setForm(initialFormState)
      showToast({
        variant: "success",
        title: "Message sent",
        description: "Our team will contact you soon.",
      })
    } catch {
      showToast({
        variant: "error",
        title: "Submission failed",
        description: "Please verify your details and try again.",
      })
    } finally {
      setSubmitting(false)
    }
  }

  if (loading) {
    return (
      <section className="bg-background py-16">
        <div className="mx-auto max-w-7xl px-5">
          <p className="text-center text-muted-foreground">Loading contact page...</p>
        </div>
      </section>
    )
  }

  if (error) {
    return (
      <section className="bg-background py-16">
        <div className="mx-auto max-w-7xl px-5">
          <p className="text-center text-muted-foreground">{error}</p>
        </div>
      </section>
    )
  }

  return (
    <>
      <HeroSection data={data.hero} />

      <section className="bg-muted py-16 md:py-20">
        <div className="mx-auto max-w-7xl px-5">
          <div className="grid grid-cols-1 gap-8 lg:grid-cols-2 lg:gap-12">
            <FadeUp>
              <div className="space-y-6">
                <h2 className="text-3xl font-bold text-foreground">{data.location.title?.trim() || "Visit Our Store"}</h2>
                {data.location.displayName?.trim() ? <p className="text-xl font-semibold text-foreground">{data.location.displayName.trim()}</p> : null}
                {addressLines.length > 0 ? (
                  <p className="text-muted-foreground">
                    {addressLines.map((line, index) => (
                      <span key={`${line}-${index}`}>
                        {line}
                        <br />
                      </span>
                    ))}
                  </p>
                ) : null}

                {timingItems.length > 0 ? (
                  <div className="space-y-1 text-sm text-muted-foreground">
                    {timingItems.map((item, index) => (
                      <p key={`${item.day}-${item.hours}-${index}`}>
                        {item.day}: {item.hours}
                      </p>
                    ))}
                  </div>
                ) : null}

                <div className="space-y-2 text-sm text-muted-foreground">
                  {phoneRaw.length > 0 ? (
                    <p>
                      Phone:{" "}
                      <a href={phoneHref} className="text-foreground hover:text-primary">
                        {phoneRaw}
                      </a>
                    </p>
                  ) : null}
                  {email.length > 0 ? (
                    <p>
                      Email:{" "}
                      <a href={`mailto:${email}`} className="text-foreground hover:text-primary">
                        {email}
                      </a>
                    </p>
                  ) : null}
                </div>

                {whatsappHref.length > 0 ? (
                  <a
                    href={whatsappHref}
                    target="_blank"
                    rel="noreferrer"
                    className="inline-flex items-center gap-3 rounded-xl bg-primary px-6 py-3.5 font-medium text-primary-foreground transition hover:bg-primary/80"
                  >
                    WhatsApp Us
                  </a>
                ) : null}
              </div>
            </FadeUp>

            <FadeUp delay={0.1}>
              <form onSubmit={onSubmit} className="rounded-2xl border border-border bg-card p-8 shadow-sm">
                <div className="grid gap-5">
                  <label className="grid gap-2 text-sm text-muted-foreground" htmlFor="contact-name">
                    Name
                    <input
                      id="contact-name"
                      required
                      value={form.name}
                      onChange={(event) => setForm((current) => ({ ...current, name: event.target.value }))}
                      className="rounded-xl border border-border bg-background px-5 py-4 text-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
                    />
                  </label>

                  <label className="grid gap-2 text-sm text-muted-foreground" htmlFor="contact-email">
                    Email
                    <input
                      id="contact-email"
                      type="email"
                      required
                      value={form.email}
                      onChange={(event) => setForm((current) => ({ ...current, email: event.target.value }))}
                      className="rounded-xl border border-border bg-background px-5 py-4 text-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
                    />
                  </label>

                  <label className="grid gap-2 text-sm text-muted-foreground" htmlFor="contact-subject">
                    Subject
                    <input
                      id="contact-subject"
                      value={form.subject}
                      onChange={(event) => setForm((current) => ({ ...current, subject: event.target.value }))}
                      className="rounded-xl border border-border bg-background px-5 py-4 text-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
                    />
                  </label>

                  <label className="grid gap-2 text-sm text-muted-foreground" htmlFor="contact-message">
                    Message
                    <textarea
                      id="contact-message"
                      required
                      rows={5}
                      value={form.message}
                      onChange={(event) => setForm((current) => ({ ...current, message: event.target.value }))}
                      className="rounded-xl border border-border bg-background px-5 py-4 text-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
                    />
                  </label>

                  <button
                    type="submit"
                    disabled={submitting}
                    className="w-full rounded-xl bg-primary py-4 text-lg font-semibold text-primary-foreground transition hover:bg-primary/80 active:scale-[0.98] disabled:cursor-not-allowed disabled:opacity-70"
                  >
                    {submitting ? "Sending..." : "Send Message"}
                  </button>
                </div>
              </form>
            </FadeUp>
          </div>
        </div>
      </section>

      <section className="bg-background py-16">
        <div className="mx-auto max-w-7xl px-5">
          <FadeUp>
            <div className="mb-6 flex justify-center">
              <img
                src={data.location.imageSrc?.trim() || fallbackData.location.imageSrc || ""}
                alt={data.location.imageAlt?.trim() || fallbackData.location.imageAlt || "Location"}
                loading="lazy"
                className="h-48 w-full max-w-xl rounded-xl object-cover"
              />
            </div>
          </FadeUp>

          {mapEmbedUrl.length > 0 ? (
            <FadeUp delay={0.1}>
              <h3 className="mb-6 text-center text-3xl font-bold text-foreground">Find Us</h3>
              <div className="overflow-hidden rounded-2xl border border-border shadow-md">
                <iframe
                  title={data.location.mapTitle?.trim() || "Location map"}
                  src={mapEmbedUrl}
                  className="h-[420px] w-full border-0"
                  loading="lazy"
                  referrerPolicy="no-referrer-when-downgrade"
                  allowFullScreen
                />
              </div>
            </FadeUp>
          ) : null}
        </div>
      </section>
    </>
  )
}
