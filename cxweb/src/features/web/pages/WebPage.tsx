import { lazy, Suspense, useEffect, useMemo, useState } from "react"
import { useParams } from "react-router"

import SectionRenderer from "@/features/web/components/SectionRenderer"
import { SectionType } from "@/features/web/services/web-page-api"
import { webPageApi, type WebPageResponse } from "@/features/web/services/web-page-api"
import SliderSkeleton from "@/features/slider/components/SliderSkeleton"
import { SliderProvider } from "@/features/slider/context/SliderProvider"
import { HttpError } from "@/shared/services/http-client"
import { CardWrapper, PageContainer, SectionContainer, SectionHeader } from "@/shared/components/design-system"

type WebPageProps = {
  defaultSlug?: string
}

const GlobalHomeSlider = lazy(async () => import("@/features/slider/components/GlobalHomeSlider"))

export default function WebPage({ defaultSlug }: WebPageProps) {
  const params = useParams()
  const slug = useMemo(() => (params.slug && params.slug.trim().length > 0 ? params.slug : defaultSlug ?? "home"), [params.slug, defaultSlug])
  const [data, setData] = useState<WebPageResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const orderedSections = useMemo(() => (data ? [...data.sections].sort((a, b) => a.displayOrder - b.displayOrder) : []), [data])
  const visibleSections = useMemo(() => orderedSections.filter((section) => section.sectionType !== SectionType.Menu), [orderedSections])
  const isFullWidthSection = (sectionType: SectionType): boolean =>
    sectionType === SectionType.Hero ||
    sectionType === SectionType.About ||
    sectionType === SectionType.Stats ||
    sectionType === SectionType.Catalog ||
    sectionType === SectionType.WhyChooseUs ||
    sectionType === SectionType.BrandSlider

  useEffect(() => {
    let isMounted = true

    const load = async (): Promise<void> => {
      setIsLoading(true)
      try {
        const response = await webPageApi.getPublishedPage(slug)
        if (isMounted) {
          setData(response)
          setError(null)
          if (typeof document !== "undefined") {
            document.title = response.seoTitle
            const meta = document.querySelector("meta[name='description']")
            if (meta) {
              meta.setAttribute("content", response.seoDescription)
            }
          }
        }
      } catch (err) {
        if (isMounted) {
          if (err instanceof HttpError && err.status === 404) {
            setError("Page not found.")
          } else {
            setError("Unable to load page.")
          }
          setData(null)
        }
      } finally {
        if (isMounted) {
          setIsLoading(false)
        }
      }
    }

    void load()

    return () => {
      isMounted = false
    }
  }, [slug])

  if (isLoading) {
    return (
      <PageContainer className="py-16">
        <CardWrapper>
          <SectionContainer className="border-0 bg-transparent p-0">
            <SectionHeader title="Loading" />
          </SectionContainer>
        </CardWrapper>
      </PageContainer>
    )
  }

  if (error || !data) {
    return (
      <PageContainer className="py-16">
        <CardWrapper>
          <SectionContainer className="border-0 bg-transparent p-0">
            <SectionHeader title="Unavailable" subtitle={error ?? "Page not available."} />
          </SectionContainer>
        </CardWrapper>
      </PageContainer>
    )
  }

  return (
    <>
      {slug === "home" ? (
        <SliderProvider>
          <Suspense fallback={<SliderSkeleton />}>
            <GlobalHomeSlider />
          </Suspense>
        </SliderProvider>
      ) : null}
      <div className="grid gap-0 py-12 md:py-16">
        {visibleSections.map((section) =>
          isFullWidthSection(section.sectionType) ? (
            <SectionRenderer key={section.id} section={section} />
          ) : (
            <PageContainer key={section.id}>
              <SectionRenderer section={section} />
            </PageContainer>
          ),
        )}
      </div>
    </>
  )
}
