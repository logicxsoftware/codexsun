import { useEffect, useMemo, useState } from "react"
import { Link, useSearchParams } from "react-router"

import BlogPostCard from "@/features/blog/components/BlogPostCard"
import { useBlogPosts } from "@/features/blog/hooks/useBlogPosts"
import { blogApi } from "@/features/blog/services/blog-api"
import type { BlogCategoryItem, BlogTagItem } from "@/features/blog/types/blog-types"

const parsePage = (value: string | null): number => {
  const parsed = Number(value)
  return Number.isFinite(parsed) && parsed > 0 ? Math.floor(parsed) : 1
}

export default function BlogList() {
  const [params, setParams] = useSearchParams()
  const [categories, setCategories] = useState<BlogCategoryItem[]>([])
  const [tags, setTags] = useState<BlogTagItem[]>([])

  const query = useMemo(() => {
    return {
      category: params.get("category") ?? undefined,
      tag: params.get("tag") ?? undefined,
      sort: (params.get("sort") as "newest" | "oldest" | null) ?? "newest",
      page: parsePage(params.get("page")),
      pageSize: 12,
    }
  }, [params])

  const { data, loading, error } = useBlogPosts(query)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      const [categoryData, tagData] = await Promise.all([blogApi.getCategories(), blogApi.getTags()])
      if (!mounted) {
        return
      }

      setCategories(categoryData)
      setTags(tagData)
    }

    void load()

    return () => {
      mounted = false
    }
  }, [])

  const update = (next: Record<string, string | undefined>) => {
    const merged = new URLSearchParams(params)

    for (const [key, value] of Object.entries(next)) {
      if (!value || value.trim().length === 0) {
        merged.delete(key)
      } else {
        merged.set(key, value)
      }
    }

    if (!next.page) {
      merged.set("page", "1")
    }

    setParams(merged)
  }

  return (
    <section className="bg-background py-12">
      <div className="mx-auto max-w-7xl space-y-6 px-5">
        <div className="flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-semibold text-foreground">Blog</h1>
            <p className="text-sm text-muted-foreground">{data.pagination.totalItems} articles</p>
          </div>
          <Link to="/blog/search" className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-card-foreground transition hover:bg-muted">
            Advanced search
          </Link>
        </div>

        <div className="grid gap-3 rounded-xl border border-border bg-card p-4 md:grid-cols-4">
          <select
            value={query.category ?? ""}
            onChange={(event) => update({ category: event.target.value || undefined, page: "1" })}
            className="rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground"
          >
            <option value="">All categories</option>
            {categories.map((category) => (
              <option key={category.id} value={category.slug}>
                {category.name}
              </option>
            ))}
          </select>

          <select
            value={query.tag ?? ""}
            onChange={(event) => update({ tag: event.target.value || undefined, page: "1" })}
            className="rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground"
          >
            <option value="">All tags</option>
            {tags.map((tag) => (
              <option key={tag.id} value={tag.slug}>
                {tag.name}
              </option>
            ))}
          </select>

          <select
            value={query.sort}
            onChange={(event) => update({ sort: event.target.value, page: "1" })}
            className="rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground"
          >
            <option value="newest">Newest</option>
            <option value="oldest">Oldest</option>
          </select>

          <Link to="/blog/search" className="rounded-lg bg-primary px-3 py-2 text-center text-sm font-medium text-primary-foreground transition hover:bg-primary/90">
            Search operators: "exact" AND OR -exclude word*
          </Link>
        </div>

        {error ? <p className="rounded-lg border border-border bg-card p-4 text-sm text-muted-foreground">{error}</p> : null}

        {loading ? (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {Array.from({ length: 6 }).map((_, index) => (
              <div key={index} className="h-56 animate-pulse rounded-xl border border-border bg-card" />
            ))}
          </div>
        ) : data.data.length === 0 ? (
          <p className="rounded-lg border border-border bg-card p-8 text-center text-muted-foreground">No blog posts match current filters.</p>
        ) : (
          <div className="grid gap-4 sm:grid-cols-2 lg:grid-cols-3">
            {data.data.map((post) => (
              <BlogPostCard key={post.id} post={post} />
            ))}
          </div>
        )}

        <div className="flex items-center justify-center gap-2">
          <button
            type="button"
            disabled={!data.pagination.hasPrevious}
            onClick={() => update({ page: String(Math.max(1, data.pagination.page - 1)) })}
            className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-card-foreground disabled:opacity-50"
          >
            Previous
          </button>
          <span className="text-sm text-muted-foreground">
            Page {data.pagination.page} of {data.pagination.totalPages}
          </span>
          <button
            type="button"
            disabled={!data.pagination.hasNext}
            onClick={() => update({ page: String(data.pagination.page + 1) })}
            className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-card-foreground disabled:opacity-50"
          >
            Next
          </button>
        </div>
      </div>
    </section>
  )
}
