import { useMemo } from "react"
import { Link, useSearchParams } from "react-router"

import { useBlogSearch } from "@/features/blog/hooks/useBlogSearch"

export default function BlogSearchPage() {
  const [params, setParams] = useSearchParams()

  const query = useMemo(() => {
    return {
      q: params.get("q") ?? "",
      category: params.get("category") ?? undefined,
      tag: params.get("tag") ?? undefined,
      sort: (params.get("sort") as "relevance" | "newest" | "oldest" | null) ?? "relevance",
      page: Number(params.get("page") ?? "1"),
      pageSize: 12,
    }
  }, [params])

  const { data, loading, error } = useBlogSearch(query)

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
      <div className="mx-auto max-w-6xl space-y-6 px-5">
        <div className="flex items-center justify-between gap-3">
          <h1 className="text-3xl font-semibold text-foreground">Blog Search</h1>
          <Link to="/blog" className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-card-foreground">
            Back to blog list
          </Link>
        </div>

        <div className="grid gap-3 rounded-xl border border-border bg-card p-4 md:grid-cols-[minmax(0,1fr)_180px]">
          <input
            value={query.q}
            onChange={(event) => update({ q: event.target.value })}
            placeholder='Try: "gaming pc" AND upgrade -budget gpu*'
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground"
          />
          <select value={query.sort} onChange={(event) => update({ sort: event.target.value })} className="rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground">
            <option value="relevance">Relevance</option>
            <option value="newest">Newest</option>
            <option value="oldest">Oldest</option>
          </select>
        </div>

        {error ? <p className="rounded-lg border border-border bg-card p-4 text-sm text-muted-foreground">{error}</p> : null}

        {loading ? (
          <div className="space-y-3">
            {Array.from({ length: 5 }).map((_, index) => (
              <div key={index} className="h-24 animate-pulse rounded-lg border border-border bg-card" />
            ))}
          </div>
        ) : data.data.length === 0 ? (
          <p className="rounded-lg border border-border bg-card p-6 text-sm text-muted-foreground">No results for this query.</p>
        ) : (
          <div className="space-y-3">
            {data.data.map((item) => (
              <article key={item.id} className="rounded-lg border border-border bg-card p-4">
                <Link to={`/blog/${item.slug}`} className="text-lg font-semibold text-card-foreground hover:underline">
                  {item.title}
                </Link>
                {item.headline ? <p className="mt-2 text-sm text-muted-foreground" dangerouslySetInnerHTML={{ __html: item.headline }} /> : null}
                <p className="mt-2 text-xs text-muted-foreground">Rank {item.rank.toFixed(3)}</p>
              </article>
            ))}
          </div>
        )}
      </div>
    </section>
  )
}
