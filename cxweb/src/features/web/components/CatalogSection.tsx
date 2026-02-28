import type { CatalogSectionData } from "@/features/web/services/web-page-api"
import CatalogCard from "@/features/web/components/CatalogCard"

type CatalogSectionProps = {
  data: CatalogSectionData
}

export default function CatalogSection({ data }: CatalogSectionProps) {
  const heading = data.heading?.trim() || "Catalog"
  const subheading = data.subheading?.trim() || ""
  const categories = data.categories
    .slice()
    .sort((a, b) => (a.order ?? 0) - (b.order ?? 0))

  if (categories.length === 0) {
    return null
  }

  return (
    <section className="bg-muted/70 py-20 md:py-24">
      <div className="mx-auto container px-5">
        <div className="mx-auto mb-12 max-w-3xl text-center">
          <h2 className="text-3xl font-bold text-foreground md:text-4xl">{heading}</h2>
          {subheading ? <p className="mt-4 text-lg text-muted-foreground">{subheading}</p> : null}
        </div>

        <div className="grid grid-cols-1 gap-8 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4">
          {categories.map((category) => (
            <CatalogCard
              key={category.slug}
              title={category.title}
              slug={category.slug}
              description={category.description ?? ""}
              image={category.image}
              variants={category.variants ?? []}
              badge={category.bulkBadge ?? ""}
              badgeVariant={category.badgeVariant}
              featuredBadge={category.featuredBadge ?? ""}
              featuredBadgeVariant={category.featuredBadgeVariant}
            />
          ))}
        </div>
      </div>
    </section>
  )
}
