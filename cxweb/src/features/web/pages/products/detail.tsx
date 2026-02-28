import { useEffect, useState } from "react"
import { Link, useParams } from "react-router"

import type { ProductDetailResponse } from "@/features/web/services/product-catalog-api"
import ProductGallery from "@/features/web/pages/products/components/ProductGallery"
import ProductSpecifications from "@/features/web/pages/products/components/ProductSpecifications"
import RelatedProducts from "@/features/web/pages/products/components/RelatedProducts"
import { productCatalogApi } from "@/features/web/services/product-catalog-api"
import { HttpError } from "@/shared/services/http-client"

const formatCurrency = (value: number): string =>
  new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 0,
  }).format(value)

export default function ProductDetailPage() {
  const { slug } = useParams()
  const [data, setData] = useState<ProductDetailResponse | null>(null)
  const [loading, setLoading] = useState(true)
  const [error, setError] = useState<string | null>(null)
  const [selectedImage, setSelectedImage] = useState(0)

  useEffect(() => {
    let mounted = true

    const load = async () => {
      if (!slug || slug.trim().length === 0) {
        setError("Invalid product URL.")
        setLoading(false)
        return
      }

      try {
        setLoading(true)
        const response = await productCatalogApi.getBySlug(slug)
        if (!mounted) {
          return
        }
        setData(response)
        setSelectedImage(0)
        setError(null)
      } catch (err) {
        if (!mounted) {
          return
        }
        if (err instanceof HttpError && err.status === 404) {
          setError("Product not found.")
        } else {
          setError("Unable to load product details.")
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
  }, [slug])

  useEffect(() => {
    if (typeof window !== "undefined") {
      window.scrollTo({ top: 0, behavior: "auto" })
    }
  }, [slug])

  useEffect(() => {
    if (!data || typeof document === "undefined") {
      return
    }

    const title = `${data.product.name} | ${data.product.categoryName}`
    const description = data.product.shortDescription?.trim() || data.product.description.slice(0, 160)
    const canonicalUrl = `${window.location.origin}/products/${data.product.slug}`
    const ogImage = data.product.images[0]

    document.title = title
    const upsertMeta = (attr: "name" | "property", key: string, value: string) => {
      const selector = `meta[${attr}="${key}"]`
      let tag = document.head.querySelector(selector) as HTMLMetaElement | null
      if (!tag) {
        tag = document.createElement("meta")
        tag.setAttribute(attr, key)
        document.head.appendChild(tag)
      }
      tag.content = value
    }

    let canonical = document.head.querySelector('link[rel="canonical"]') as HTMLLinkElement | null
    if (!canonical) {
      canonical = document.createElement("link")
      canonical.rel = "canonical"
      document.head.appendChild(canonical)
    }
    canonical.href = canonicalUrl

    upsertMeta("name", "description", description)
    upsertMeta("property", "og:title", title)
    upsertMeta("property", "og:description", description)
    upsertMeta("property", "og:url", canonicalUrl)
    if (ogImage) {
      upsertMeta("property", "og:image", ogImage)
    }
  }, [data])

  if (loading) {
    return (
      <section className="bg-background py-12 md:py-16">
        <div className="mx-auto max-w-7xl px-5">
          <div className="h-96 animate-pulse rounded-xl border border-border bg-card" />
        </div>
      </section>
    )
  }

  if (error || !data) {
    return (
      <section className="bg-background py-12 md:py-16">
        <div className="mx-auto max-w-7xl px-5">
          <p className="rounded-lg border border-border bg-card p-6 text-sm text-muted-foreground">{error ?? "Product unavailable."}</p>
        </div>
      </section>
    )
  }

  const inStock = data.product.inStock
  const product = data.product

  return (
    <section className="bg-background py-12 md:py-16">
      <div className="mx-auto max-w-7xl px-5">
        <div className="mb-6 flex items-center gap-2 text-sm text-muted-foreground">
          <Link to="/products" className="hover:text-primary">
            Products
          </Link>
          <span>/</span>
          <span>{product.categoryName}</span>
        </div>

        <div className="grid grid-cols-1 gap-8 lg:grid-cols-2">
          <ProductGallery productName={product.name} images={product.images} selectedImage={selectedImage} onSelectImage={setSelectedImage} />

          <div className="rounded-xl border border-border bg-card p-6">
            <h1 className="text-3xl font-semibold text-foreground md:text-4xl">{product.name}</h1>
            {product.shortDescription ? <p className="mt-2 text-muted-foreground">{product.shortDescription}</p> : null}

            <div className="mt-5 flex flex-wrap items-center gap-3">
              <p className="text-2xl font-semibold text-foreground">{formatCurrency(product.price)}</p>
              {typeof product.comparePrice === "number" ? <p className="text-sm text-muted-foreground line-through">{formatCurrency(product.comparePrice)}</p> : null}
              <span className={`rounded-full px-3 py-1 text-xs ${inStock ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground"}`}>
                {inStock ? "In stock" : "Out of stock"}
              </span>
            </div>

            <div className="mt-6 space-y-2 text-sm text-muted-foreground">
              <p>
                <span className="text-foreground">SKU:</span> {product.sku || "NA"}
              </p>
              <p>
                <span className="text-foreground">Category:</span> {product.categoryName}
              </p>
            </div>

            <ProductSpecifications specifications={product.specifications} />

            <div className="mt-6">
              <Link
                to={`/web-contacts?product=${encodeURIComponent(product.name)}`}
                className="inline-flex items-center rounded-lg bg-primary px-5 py-3 text-sm font-medium text-primary-foreground transition hover:bg-primary/80"
              >
                Add to Quote
              </Link>
            </div>
          </div>
        </div>

        <div className="mt-10 rounded-xl border border-border bg-card p-6">
          <h2 className="text-lg font-semibold text-foreground">Description</h2>
          <p className="mt-3 whitespace-pre-line text-muted-foreground">{product.description}</p>
        </div>

        <RelatedProducts items={data.relatedProducts} categoryName={product.categoryName} categorySlug={product.categorySlug} />
      </div>
    </section>
  )
}
