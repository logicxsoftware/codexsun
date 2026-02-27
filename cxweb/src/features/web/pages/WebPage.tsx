import { useEffect, useMemo, useState } from "react"
import { useParams } from "react-router"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { HttpError } from "@/shared/services/http-client"
import SectionRenderer from "@/features/web/components/SectionRenderer"
import { webPageApi, type WebPageResponse } from "@/features/web/services/web-page-api"

type WebPageProps = {
  defaultSlug?: string
}

export default function WebPage({ defaultSlug }: WebPageProps) {
  const params = useParams()
  const slug = useMemo(() => (params.slug && params.slug.trim().length > 0 ? params.slug : defaultSlug ?? "home"), [params.slug, defaultSlug])
  const [data, setData] = useState<WebPageResponse | null>(null)
  const [isLoading, setIsLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const orderedSections = useMemo(
    () => (data ? [...data.sections].sort((a, b) => a.displayOrder - b.displayOrder) : []),
    [data],
  )

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
      <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
        <Card>
          <CardHeader>
            <CardTitle>Loading</CardTitle>
          </CardHeader>
        </Card>
      </section>
    )
  }

  if (error || !data) {
    return (
      <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
        <Card>
          <CardHeader>
            <CardTitle>Unavailable</CardTitle>
          </CardHeader>
          <CardContent className="text-muted-foreground">{error ?? "Page not available."}</CardContent>
        </Card>
      </section>
    )
  }

  return (
    <section className="mx-auto grid w-full max-w-6xl gap-6 px-4 py-16 md:px-6">
      <header className="grid gap-2">
        <h1 className="text-3xl font-semibold tracking-tight">{data.title}</h1>
      </header>
      {orderedSections.map((section) => (
        <SectionRenderer key={section.id} section={section} />
      ))}
    </section>
  )
}
