import { Link } from "react-router"

import FadeUp from "@/components/animations/FadeUp"
import { cn } from "@/lib/utils"

type BadgeVariant = "emerald" | "amber" | "blue" | "purple" | "rose" | "cyan" | "indigo" | "teal" | "black"

type CatalogCardProps = {
  title: string
  slug: string
  description?: string
  image: string
  variants?: string[]
  badge?: string
  badgeVariant?: BadgeVariant
  featuredBadge?: string
  featuredBadgeVariant?: BadgeVariant
}

const badgeClassMap: Record<BadgeVariant, string> = {
  emerald: "bg-primary text-primary-foreground",
  amber: "bg-secondary text-secondary-foreground",
  blue: "bg-accent text-accent-foreground",
  purple: "bg-muted text-foreground",
  rose: "bg-destructive text-destructive-foreground",
  cyan: "bg-card text-card-foreground",
  indigo: "bg-accent text-accent-foreground",
  teal: "bg-primary text-primary-foreground",
  black: "bg-foreground text-background",
}

export default function CatalogCard({
  title,
  slug,
  description,
  image,
  variants,
  badge = "",
  badgeVariant,
  featuredBadge = "",
  featuredBadgeVariant,
}: CatalogCardProps) {
  const showBulkBadge = badge.trim().length > 0
  const showFeaturedBadge = featuredBadge.trim().length > 0

  return (
    <FadeUp>
      <Link to={`/catalog/${slug}`} className="group block">
        <div className="relative overflow-hidden rounded-xl border border-primary/30 bg-card transition-all duration-300 hover:-translate-y-2 hover:border-primary/60 hover:shadow-xl">
          {showBulkBadge ? (
            <span
              className={cn(
                "absolute left-4 top-4 z-10 rounded-full px-4 py-1.5 text-xs font-semibold shadow-md",
                badgeClassMap[badgeVariant ?? "emerald"] ?? badgeClassMap.emerald,
              )}
            >
              {badge.trim()}
            </span>
          ) : null}

          {showFeaturedBadge ? (
            <span
              className={cn(
                "absolute right-4 top-4 z-10 rounded-full px-3 py-1 text-xs font-medium shadow",
                badgeClassMap[featuredBadgeVariant ?? "amber"] ?? badgeClassMap.amber,
              )}
            >
              {featuredBadge.trim()}
            </span>
          ) : null}

          <img
            src={image}
            alt={title}
            loading="lazy"
            className="h-64 w-full object-contain transition-transform duration-500 group-hover:scale-105"
          />

          <div className="p-6">
            <h3 className="text-xl font-semibold text-foreground transition-colors group-hover:text-primary">{title}</h3>

            {description ? <p className="mt-2 text-sm leading-relaxed text-muted-foreground group-hover:text-primary/70">{description}</p> : null}

            <div className="mt-5 flex flex-wrap gap-2">
              {(variants ?? []).map((variant) => (
                <span
                  key={`${slug}-${variant}`}
                  className="rounded-full border border-border bg-muted px-3 py-1 text-xs font-medium text-primary/80"
                >
                  {variant}
                </span>
              ))}
            </div>
          </div>
        </div>
      </Link>
    </FadeUp>
  )
}
