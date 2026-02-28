import { useEffect, useState } from "react"

import AboutSection from "@/features/web/components/AboutSection"
import CallToActionSection from "@/features/web/components/CallToActionSection"
import FeaturesSection from "@/features/web/components/FeaturesSection"
import HeroSection from "@/features/web/components/HeroSection"
import WhyChooseUsSection from "@/features/web/components/WhyChooseUsSection"
import RoadmapSection from "@/features/web/pages/about/blocks/RoadmapSection"
import TeamSection from "@/features/web/pages/about/blocks/TeamSection"
import TestimonialsSection from "@/features/web/pages/about/blocks/TestimonialsSection"
import { aboutPageApi, type AboutPageResponse } from "@/features/web/services/about-page-api"
import { HttpError } from "@/shared/services/http-client"

const fallbackData: AboutPageResponse = {
  hero: {
    title: "",
    subtitle: "",
  },
  about: {
    title: "",
    subtitle: "",
    content: [],
    image: {
      src: "",
      alt: "",
    },
  },
  whyChooseUs: {
    heading: "",
    subheading: "",
    items: [],
  },
  features: {
    title: "",
    description: "",
    imageSrc: "",
    imageAlt: "",
    bullets: [],
  },
  team: [],
  testimonials: [],
  roadmap: [],
  callToAction: {
    title: "",
    description: "",
    buttonText: "",
    buttonHref: "",
  },
}

export default function AboutPage() {
  const [data, setData] = useState<AboutPageResponse>(fallbackData)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        const response = await aboutPageApi.get()
        if (!mounted) {
          return
        }

        setData(response)
        setError(null)
        if (typeof document !== "undefined") {
          document.title = "About"
        }
      } catch (err) {
        if (!mounted) {
          return
        }

        if (err instanceof HttpError && err.status === 404) {
          setError("About page data is not available.")
        } else {
          setError("Unable to load about page.")
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

  if (loading) {
    return (
      <section className="bg-background py-16">
        <div className="mx-auto max-w-7xl px-5">
          <p className="text-center text-muted-foreground">Loading about page...</p>
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
      <AboutSection data={data.about} />
      <WhyChooseUsSection data={data.whyChooseUs} />
      <FeaturesSection data={data.features} />
      <TeamSection members={data.team} />
      <TestimonialsSection items={data.testimonials} />
      <RoadmapSection milestones={data.roadmap} />
      <CallToActionSection data={data.callToAction} />
    </>
  )
}
