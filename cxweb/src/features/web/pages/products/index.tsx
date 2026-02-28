import { useEffect, useMemo, useState } from "react"
import { useSearchParams } from "react-router"

import FilterSidebar from "@/features/web/pages/products/components/FilterSidebar"
import ProductCard from "@/features/web/pages/products/components/ProductCard"
import ProductPagination from "@/features/web/pages/products/components/ProductPagination"
import { productCatalogApi, type ProductListResponse } from "@/features/web/services/product-catalog-api"
import { HttpError } from "@/shared/services/http-client"
import { cn } from "@/lib/utils"

type ViewMode = "grid" | "list"

const parseNumber = (value: string | null): number | undefined => {
  if (!value) {
    return undefined
  }
  const parsed = Number(value)
  return Number.isFinite(parsed) ? parsed : undefined
}

const parsePage = (value: string | null, fallback: number): number => {
  const parsed = Number(value)
  return Number.isFinite(parsed) && parsed > 0 ? Math.floor(parsed) : fallback
}

const parseAttributes = (params: URLSearchParams): Record<string, string[]> => {
  const output: Record<string, string[]> = {}
  for (const [key, value] of params.entries()) {
    if (!key.startsWith("attr_")) {
      continue
    }
    const attributeKey = key.slice(5).trim().toLowerCase()
    if (attributeKey.length === 0 || value.trim().length === 0) {
      continue
    }
    if (!output[attributeKey]) {
      output[attributeKey] = []
    }
    if (!output[attributeKey].includes(value)) {
      output[attributeKey].push(value)
    }
  }
  return output
}

const buildParams = (
  current: URLSearchParams,
  updates: Record<string, string | undefined>,
  attributes?: Record<string, string[]>,
): URLSearchParams => {
  const params = new URLSearchParams(current)
  for (const [key, value] of Object.entries(updates)) {
    if (typeof value === "string" && value.trim().length > 0) {
      params.set(key, value.trim())
    } else {
      params.delete(key)
    }
  }

  if (attributes) {
    for (const key of [...params.keys()]) {
      if (key.startsWith("attr_")) {
        params.delete(key)
      }
    }
    for (const [attrKey, values] of Object.entries(attributes)) {
      for (const value of values) {
        if (value.trim().length > 0) {
          params.append(`attr_${attrKey}`, value.trim())
        }
      }
    }
  }

  return params
}

const emptyResponse: ProductListResponse = {
  data: [],
  pagination: {
    page: 1,
    pageSize: 12,
    totalItems: 0,
    totalPages: 1,
    hasPrevious: false,
    hasNext: false,
  },
  filters: {
    categories: [],
    attributes: [],
    priceRange: {
      min: null,
      max: null,
    },
  },
}

export default function ProductsPage() {
  const [searchParams, setSearchParams] = useSearchParams()
  const [response, setResponse] = useState<ProductListResponse>(emptyResponse)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [viewMode, setViewMode] = useState<ViewMode>("grid")
  const [showMobileFilters, setShowMobileFilters] = useState(false)
  const [searchDraft, setSearchDraft] = useState(searchParams.get("search") ?? "")

  const parsedQuery = useMemo(() => {
    return {
      category: searchParams.get("category") ?? undefined,
      minPrice: parseNumber(searchParams.get("minPrice")),
      maxPrice: parseNumber(searchParams.get("maxPrice")),
      search: searchParams.get("search") ?? undefined,
      sort: searchParams.get("sort") ?? "latest",
      page: parsePage(searchParams.get("page"), 1),
      pageSize: parsePage(searchParams.get("pageSize"), 12),
      attributes: parseAttributes(searchParams),
    }
  }, [searchParams])

  useEffect(() => {
    setSearchDraft(searchParams.get("search") ?? "")
  }, [searchParams])

  useEffect(() => {
    const timer = window.setTimeout(() => {
      const currentSearch = searchParams.get("search") ?? ""
      const normalizedDraft = searchDraft.trim()
      const currentPage = searchParams.get("page") ?? "1"
      if (currentSearch === normalizedDraft && currentPage === "1") {
        return
      }

      const next = buildParams(
        searchParams,
        {
          search: normalizedDraft || undefined,
          page: "1",
        },
        parsedQuery.attributes,
      )
      setSearchParams(next, { replace: true })
    }, 300)

    return () => {
      window.clearTimeout(timer)
    }
  }, [searchDraft, searchParams, setSearchParams, parsedQuery.attributes])

  useEffect(() => {
    let mounted = true

    const load = async () => {
      try {
        setLoading(true)
        const data = await productCatalogApi.list(parsedQuery)
        if (!mounted) {
          return
        }
        setResponse(data)
        setError(null)
      } catch (err) {
        if (!mounted) {
          return
        }
        if (err instanceof HttpError && err.status === 404) {
          setError("Products are not available.")
        } else {
          setError("Unable to load products.")
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
  }, [parsedQuery])

  const updateParams = (updates: Record<string, string | undefined>, attributes?: Record<string, string[]>) => {
    const next = buildParams(searchParams, updates, attributes ?? parsedQuery.attributes)
    setSearchParams(next)
  }

  const handleCategoryChange = (category?: string) => {
    updateParams({ category, page: "1" })
  }

  const handleMinPriceChange = (value?: string) => {
    updateParams({ minPrice: value, page: "1" })
  }

  const handleMaxPriceChange = (value?: string) => {
    updateParams({ maxPrice: value, page: "1" })
  }

  const handleSortChange = (sort: string) => {
    updateParams({ sort, page: "1" })
  }

  const handleAttributeToggle = (key: string, value: string, checked: boolean) => {
    const current = { ...parsedQuery.attributes }
    const values = new Set(current[key] ?? [])
    if (checked) {
      values.add(value)
    } else {
      values.delete(value)
    }
    if (values.size === 0) {
      delete current[key]
    } else {
      current[key] = [...values]
    }
    updateParams({ page: "1" }, current)
  }

  const handlePageChange = (page: number) => {
    updateParams({ page: String(page) })
    window.scrollTo({ top: 0, behavior: "smooth" })
  }

  const handleClearFilters = () => {
    const next = new URLSearchParams()
    next.set("sort", "latest")
    next.set("page", "1")
    next.set("pageSize", String(parsedQuery.pageSize))
    setSearchParams(next)
  }

  return (
    <section className="bg-background py-12 md:py-16">
      <div className="mx-auto max-w-7xl px-5">
        <div className="mb-6 flex flex-wrap items-center justify-between gap-3">
          <div>
            <h1 className="text-3xl font-semibold text-foreground">Products</h1>
            <p className="text-sm text-muted-foreground">{response.pagination.totalItems} items found</p>
          </div>
          <div className="flex items-center gap-2">
            <button
              type="button"
              onClick={() => setShowMobileFilters(true)}
              className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-foreground transition hover:bg-muted lg:hidden"
            >
              Filters
            </button>
            <select
              value={parsedQuery.sort ?? "latest"}
              onChange={(event) => handleSortChange(event.target.value)}
              className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
            >
              <option value="latest">Latest</option>
              <option value="price_asc">Price Low-High</option>
              <option value="price_desc">Price High-Low</option>
              <option value="name_asc">Name A-Z</option>
              <option value="name_desc">Name Z-A</option>
            </select>
            <div className="hidden overflow-hidden rounded-lg border border-border bg-card sm:flex">
              <button
                type="button"
                onClick={() => setViewMode("grid")}
                className={cn("px-3 py-2 text-sm transition", viewMode === "grid" ? "bg-primary text-primary-foreground" : "text-foreground hover:bg-muted")}
              >
                Grid
              </button>
              <button
                type="button"
                onClick={() => setViewMode("list")}
                className={cn("px-3 py-2 text-sm transition", viewMode === "list" ? "bg-primary text-primary-foreground" : "text-foreground hover:bg-muted")}
              >
                List
              </button>
            </div>
          </div>
        </div>

        <div className="grid grid-cols-1 gap-6 lg:grid-cols-[280px_minmax(0,1fr)]">
          <div className="hidden lg:block">
            <FilterSidebar
              categories={response.filters.categories}
              attributes={response.filters.attributes}
              priceRange={response.filters.priceRange}
              selectedCategory={parsedQuery.category}
              selectedMinPrice={searchParams.get("minPrice") ?? undefined}
              selectedMaxPrice={searchParams.get("maxPrice") ?? undefined}
              selectedAttributes={parsedQuery.attributes}
              searchValue={searchDraft}
              onSearchChange={setSearchDraft}
              onCategoryChange={handleCategoryChange}
              onMinPriceChange={handleMinPriceChange}
              onMaxPriceChange={handleMaxPriceChange}
              onAttributeToggle={handleAttributeToggle}
              onClear={handleClearFilters}
            />
          </div>

          <div>
            {error ? <p className="rounded-lg border border-border bg-card p-4 text-sm text-muted-foreground">{error}</p> : null}

            {loading ? (
              <div className={cn("grid gap-4", viewMode === "list" ? "grid-cols-1" : "grid-cols-2 xl:grid-cols-3")}>
                {Array.from({ length: 6 }).map((_, index) => (
                  <div key={index} className="h-56 animate-pulse rounded-xl border border-border bg-card" />
                ))}
              </div>
            ) : response.data.length === 0 ? (
              <div className="rounded-xl border border-border bg-card p-8 text-center text-muted-foreground">No products matched your filters.</div>
            ) : (
              <div className={cn("grid gap-4", viewMode === "list" ? "grid-cols-1" : "grid-cols-2 xl:grid-cols-3")}>
                {response.data.map((product) => (
                  <ProductCard key={product.id} product={product} mode={viewMode} />
                ))}
              </div>
            )}

            <ProductPagination
              page={response.pagination.page}
              totalPages={response.pagination.totalPages}
              hasPrevious={response.pagination.hasPrevious}
              hasNext={response.pagination.hasNext}
              onPageChange={handlePageChange}
            />
          </div>
        </div>
      </div>

      {showMobileFilters ? (
        <div className="fixed inset-0 z-50 lg:hidden">
          <div className="absolute inset-0 bg-background/70" onClick={() => setShowMobileFilters(false)} />
          <div className="absolute right-0 top-0 h-full w-[88%] max-w-sm overflow-auto border-l border-border bg-card p-4">
            <div className="mb-4 flex items-center justify-between">
              <h2 className="text-base font-semibold text-foreground">Filters</h2>
              <button
                type="button"
                onClick={() => setShowMobileFilters(false)}
                className="rounded-lg border border-border bg-background px-3 py-1.5 text-sm text-foreground"
              >
                Close
              </button>
            </div>
            <FilterSidebar
              categories={response.filters.categories}
              attributes={response.filters.attributes}
              priceRange={response.filters.priceRange}
              selectedCategory={parsedQuery.category}
              selectedMinPrice={searchParams.get("minPrice") ?? undefined}
              selectedMaxPrice={searchParams.get("maxPrice") ?? undefined}
              selectedAttributes={parsedQuery.attributes}
              searchValue={searchDraft}
              onSearchChange={setSearchDraft}
              onCategoryChange={handleCategoryChange}
              onMinPriceChange={handleMinPriceChange}
              onMaxPriceChange={handleMaxPriceChange}
              onAttributeToggle={handleAttributeToggle}
              onClear={handleClearFilters}
            />
          </div>
        </div>
      ) : null}
    </section>
  )
}
