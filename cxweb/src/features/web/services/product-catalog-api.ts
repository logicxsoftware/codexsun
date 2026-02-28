import { httpClient } from "@/shared/services/http-client"

export type ProductListItem = {
  id: string
  name: string
  slug: string
  shortDescription?: string | null
  price: number
  comparePrice?: number | null
  stockQuantity: number
  categoryName: string
  categorySlug: string
  imageUrl?: string | null
}

export type ProductPaginationMeta = {
  page: number
  pageSize: number
  totalItems: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
}

export type ProductCategoryFilterOption = {
  name: string
  slug: string
  count: number
}

export type ProductAttributeFilterOption = {
  value: string
  count: number
}

export type ProductAttributeFilterGroup = {
  key: string
  options: ProductAttributeFilterOption[]
}

export type ProductPriceRangeFilter = {
  min?: number | null
  max?: number | null
}

export type ProductFilterMeta = {
  categories: ProductCategoryFilterOption[]
  attributes: ProductAttributeFilterGroup[]
  priceRange: ProductPriceRangeFilter
}

export type ProductListResponse = {
  data: ProductListItem[]
  pagination: ProductPaginationMeta
  filters: ProductFilterMeta
}

export type ProductDetailImage = {
  imageUrl: string
  order: number
}

export type ProductRelatedItem = {
  id: string
  name: string
  slug: string
  price: number
  comparePrice?: number | null
  imageUrl?: string | null
}

export type ProductDetailResponse = {
  product: {
    id: string
    name: string
    slug: string
    description: string
    shortDescription?: string | null
    price: number
    comparePrice?: number | null
    inStock: boolean
    categoryName: string
    categorySlug: string
    images: string[]
    specifications: Record<string, string>
    sku?: string | null
  }
  relatedProducts: ProductRelatedItem[]
}

export type ProductListQuery = {
  category?: string
  minPrice?: number
  maxPrice?: number
  search?: string
  sort?: string
  page?: number
  pageSize?: number
  attributes?: Record<string, string[]>
}

const buildQueryString = (query: ProductListQuery): string => {
  const params = new URLSearchParams()

  if (query.category) {
    params.set("category", query.category)
  }
  if (typeof query.minPrice === "number" && Number.isFinite(query.minPrice)) {
    params.set("minPrice", String(query.minPrice))
  }
  if (typeof query.maxPrice === "number" && Number.isFinite(query.maxPrice)) {
    params.set("maxPrice", String(query.maxPrice))
  }
  if (query.search && query.search.trim().length > 0) {
    params.set("search", query.search.trim())
  }
  if (query.sort && query.sort.trim().length > 0) {
    params.set("sort", query.sort.trim())
  }
  if (typeof query.page === "number") {
    params.set("page", String(query.page))
  }
  if (typeof query.pageSize === "number") {
    params.set("pageSize", String(query.pageSize))
  }

  if (query.attributes) {
    for (const [key, values] of Object.entries(query.attributes)) {
      const normalizedKey = key.trim().toLowerCase()
      if (normalizedKey.length === 0) {
        continue
      }

      for (const value of values) {
        const normalizedValue = value.trim()
        if (normalizedValue.length > 0) {
          params.append(`attr_${normalizedKey}`, normalizedValue)
        }
      }
    }
  }

  const queryString = params.toString()
  return queryString.length > 0 ? `?${queryString}` : ""
}

export const productCatalogApi = {
  list: (query: ProductListQuery): Promise<ProductListResponse> => {
    const suffix = buildQueryString(query)
    return httpClient.get<ProductListResponse>(`/products${suffix}`)
  },
  getBySlug: (slug: string): Promise<ProductDetailResponse> => httpClient.get<ProductDetailResponse>(`/products/${encodeURIComponent(slug)}`),
}
