import { useEffect, useMemo, useState } from "react"
import { Link } from "react-router"

import { blogApi } from "@/features/blog/services/blog-api"
import type { BlogPostListItem } from "@/features/blog/types/blog-types"
import type { BlogShowSectionData } from "@/features/web/services/web-page-api"

type BlogShowcaseSectionProps = {
  data: BlogShowSectionData
}

const normalizeLimit = (value: number | undefined): number => {
  if (typeof value !== "number" || !Number.isFinite(value)) {
    return 3
  }

  return Math.max(1, Math.min(6, Math.floor(value)))
}

export default function BlogShowcaseSection({ data }: BlogShowcaseSectionProps) {
  const [posts, setPosts] = useState<BlogPostListItem[]>([])
  const [loading, setLoading] = useState<boolean>(true)
  const [error, setError] = useState<string | null>(null)

  const limit = useMemo(() => normalizeLimit(data.limit), [data.limit])

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        setLoading(true)
        const response = await blogApi.getPosts({ page: 1, pageSize: limit, sort: "newest" })
        if (!mounted) {
          return
        }
        setPosts(response.data.slice(0, limit))
        setError(null)
      } catch {
        if (!mounted) {
          return
        }
        setError("Blog posts are not available right now.")
        setPosts([])
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
  }, [limit])

  return (
    <section className="bg-background py-16">
      <div className="mx-auto max-w-7xl space-y-6 px-5">
        <div className="flex flex-wrap items-end justify-between gap-3">
          <div className="space-y-2">
            <h2 className="text-3xl font-semibold text-foreground">{data.title ?? "From Our Blog"}</h2>
            <p className="text-sm text-muted-foreground">{data.subtitle ?? "Latest updates, reviews, tutorials, and expert tips."}</p>
          </div>
          <Link
            to={data.buttonHref ?? "/blog"}
            className="rounded-lg border border-border bg-card px-4 py-2 text-sm font-medium text-card-foreground transition hover:bg-muted"
          >
            {data.buttonText ?? "View all posts"}
          </Link>
        </div>

        {loading ? (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: limit }).map((_, index) => (
              <div key={index} className="h-56 animate-pulse rounded-xl border border-border bg-card" />
            ))}
          </div>
        ) : null}

        {!loading && error ? (
          <p className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">{error}</p>
        ) : null}

        {!loading && !error && posts.length === 0 ? (
          <p className="rounded-xl border border-border bg-card p-6 text-sm text-muted-foreground">No blog posts published yet.</p>
        ) : null}

        {!loading && !error && posts.length > 0 ? (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {posts.map((post) => (
              <article key={post.id} className="overflow-hidden rounded-xl border border-border bg-card">
                {post.featuredImage ? <img src={post.featuredImage} alt={post.title} className="h-44 w-full object-cover" loading="lazy" /> : null}
                <div className="space-y-3 p-4">
                  <p className="text-xs text-muted-foreground">{post.categoryName}</p>
                  <h3 className="text-lg font-semibold text-card-foreground">{post.title}</h3>
                  {post.excerpt ? <p className="line-clamp-3 text-sm text-muted-foreground">{post.excerpt}</p> : null}
                  <Link to={`/blog/${post.slug}`} className="inline-flex text-sm font-medium text-link hover:text-link-hover">
                    Read post
                  </Link>
                </div>
              </article>
            ))}
          </div>
        ) : null}
      </div>
    </section>
  )
}
