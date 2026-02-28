import { Link } from "react-router"

import type { ProductListItem } from "@/features/web/services/product-catalog-api"
import { cn } from "@/lib/utils"

type ProductCardProps = {
  product: ProductListItem
  mode: "grid" | "list"
}

const formatCurrency = (value: number): string =>
  new Intl.NumberFormat("en-IN", {
    style: "currency",
    currency: "INR",
    maximumFractionDigits: 0,
  }).format(value)

export default function ProductCard({ product, mode }: ProductCardProps) {
  const inStock = product.stockQuantity > 0

  return (
    <article className={cn("rounded-xl border border-border bg-card shadow-sm", mode === "list" ? "flex gap-4 p-4" : "p-4")}>
      <div className={cn("overflow-hidden rounded-lg bg-muted", mode === "list" ? "h-32 w-32 shrink-0" : "mb-4 aspect-[4/3] w-full")}>
        {product.imageUrl ? (
          <img src={product.imageUrl} alt={product.name} loading="lazy" className="h-full w-full object-cover" />
        ) : (
          <div className="flex h-full w-full items-center justify-center text-sm text-muted-foreground">No image</div>
        )}
      </div>

      <div className={cn("flex min-w-0 flex-1 flex-col", mode === "grid" ? "gap-2" : "gap-2")}>
        <p className="text-xs uppercase tracking-wide text-muted-foreground">{product.categoryName}</p>
        <h3 className="line-clamp-2 text-base font-semibold text-foreground">{product.name}</h3>
        {product.shortDescription ? <p className="line-clamp-2 text-sm text-muted-foreground">{product.shortDescription}</p> : null}
        <div className="mt-auto flex flex-wrap items-center gap-3">
          <p className="text-lg font-semibold text-foreground">{formatCurrency(product.price)}</p>
          {typeof product.comparePrice === "number" ? <p className="text-sm text-muted-foreground line-through">{formatCurrency(product.comparePrice)}</p> : null}
          <span className={cn("rounded-full px-2.5 py-1 text-xs", inStock ? "bg-primary/10 text-primary" : "bg-muted text-muted-foreground")}>
            {inStock ? `${product.stockQuantity} in stock` : "Out of stock"}
          </span>
        </div>
        <div>
          <Link
            to={`/products/${product.slug}`}
            className="mt-2 inline-flex items-center rounded-lg bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition hover:bg-primary/80"
          >
            View Details
          </Link>
        </div>
      </div>
    </article>
  )
}
