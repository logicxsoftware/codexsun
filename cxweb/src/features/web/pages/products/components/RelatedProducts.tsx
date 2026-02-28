import ProductCard from "@/features/web/pages/products/components/ProductCard"
import type { ProductRelatedItem } from "@/features/web/services/product-catalog-api"

type RelatedProductsProps = {
  items: ProductRelatedItem[]
  categoryName: string
  categorySlug: string
}

export default function RelatedProducts({ items, categoryName, categorySlug }: RelatedProductsProps) {
  if (items.length === 0) {
    return null
  }

  return (
    <div className="mt-10">
      <h2 className="mb-4 text-xl font-semibold text-foreground">Related Products</h2>
      <div className="grid grid-cols-2 gap-4 lg:grid-cols-4">
        {items.map((item) => (
          <ProductCard
            key={item.id}
            mode="grid"
            product={{
              id: item.id,
              name: item.name,
              slug: item.slug,
              shortDescription: null,
              price: item.price,
              comparePrice: item.comparePrice ?? null,
              stockQuantity: 1,
              categoryName,
              categorySlug,
              imageUrl: item.imageUrl ?? null,
            }}
          />
        ))}
      </div>
    </div>
  )
}
