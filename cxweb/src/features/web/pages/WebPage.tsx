import { useEffect, useMemo, useState } from "react"
import { useParams } from "react-router"

import SectionRenderer from "@/features/web/components/SectionRenderer"
import { webPageApi, type WebPageResponse } from "@/features/web/services/web-page-api"
import { HttpError } from "@/shared/services/http-client"
import { CardWrapper, PageContainer, SectionContainer, SectionHeader, Title } from "@/shared/components/design-system"

type WebPageProps = {
  defaultSlug?: string
}

export default function WebPage({ defaultSlug }: WebPageProps) {
  const params = useParams()
  const slug = useMemo(() => (params.slug && params.slug.trim().length > 0 ? params.slug : defaultSlug ?? "home"), [params.slug, defaultSlug])
  const [data, setData] = useState<WebPageResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)

  const orderedSections = useMemo(() => (data ? [...data.sections].sort((a, b) => a.displayOrder - b.displayOrder) : []), [data])

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
    <PageContainer className="py-12 md:py-16">
      <div className="grid gap-6">
        <header className="grid gap-2 border-b border-border/50 pb-5">
          <Title>{data.title}</Title>
        </header>
        {orderedSections.map((section) => (
          <SectionRenderer key={section.id} section={section} />
        ))}
      </div>
    </PageContainer>
  )
}
