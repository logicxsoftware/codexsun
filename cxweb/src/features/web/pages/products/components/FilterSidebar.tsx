import type {
  ProductAttributeFilterGroup,
  ProductCategoryFilterOption,
  ProductPriceRangeFilter,
} from "@/features/web/services/product-catalog-api"

type FilterSidebarProps = {
  categories: ProductCategoryFilterOption[]
  attributes: ProductAttributeFilterGroup[]
  priceRange: ProductPriceRangeFilter
  selectedCategory?: string
  selectedMinPrice?: string
  selectedMaxPrice?: string
  selectedAttributes: Record<string, string[]>
  searchValue: string
  onSearchChange: (value: string) => void
  onCategoryChange: (category?: string) => void
  onMinPriceChange: (value?: string) => void
  onMaxPriceChange: (value?: string) => void
  onAttributeToggle: (key: string, value: string, checked: boolean) => void
  onClear: () => void
}

const formatKey = (value: string): string => value.split("_").map((part) => part.charAt(0).toUpperCase() + part.slice(1)).join(" ")

export default function FilterSidebar({
  categories,
  attributes,
  priceRange,
  selectedCategory,
  selectedMinPrice,
  selectedMaxPrice,
  selectedAttributes,
  searchValue,
  onSearchChange,
  onCategoryChange,
  onMinPriceChange,
  onMaxPriceChange,
  onAttributeToggle,
  onClear,
}: FilterSidebarProps) {
  return (
    <aside className="space-y-6 rounded-xl border border-border bg-card p-5">
      <div>
        <label htmlFor="product-search" className="mb-2 block text-sm font-medium text-foreground">
          Search
        </label>
        <input
          id="product-search"
          value={searchValue}
          onChange={(event) => onSearchChange(event.target.value)}
          placeholder="Search products..."
          className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
        />
      </div>

      <div>
        <h3 className="mb-3 text-sm font-semibold text-foreground">Category</h3>
        <div className="space-y-2">
          <label className="flex cursor-pointer items-center gap-2 text-sm text-muted-foreground">
            <input
              type="radio"
              name="category"
              checked={!selectedCategory}
              onChange={() => onCategoryChange(undefined)}
              className="h-4 w-4 accent-primary"
            />
            All categories
          </label>
          {categories.map((category) => (
            <label key={category.slug} className="flex cursor-pointer items-center gap-2 text-sm text-muted-foreground">
              <input
                type="radio"
                name="category"
                checked={selectedCategory === category.slug}
                onChange={() => onCategoryChange(category.slug)}
                className="h-4 w-4 accent-primary"
              />
              <span>{category.name}</span>
              <span className="text-xs text-muted-foreground">({category.count})</span>
            </label>
          ))}
        </div>
      </div>

      <div>
        <h3 className="mb-3 text-sm font-semibold text-foreground">Price Range</h3>
        <div className="grid grid-cols-2 gap-2">
          <input
            type="number"
            inputMode="decimal"
            value={selectedMinPrice ?? ""}
            onChange={(event) => onMinPriceChange(event.target.value || undefined)}
            min={priceRange.min ?? undefined}
            max={priceRange.max ?? undefined}
            placeholder={typeof priceRange.min === "number" ? String(priceRange.min) : "Min"}
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
          />
          <input
            type="number"
            inputMode="decimal"
            value={selectedMaxPrice ?? ""}
            onChange={(event) => onMaxPriceChange(event.target.value || undefined)}
            min={priceRange.min ?? undefined}
            max={priceRange.max ?? undefined}
            placeholder={typeof priceRange.max === "number" ? String(priceRange.max) : "Max"}
            className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground placeholder:text-muted-foreground focus:border-primary focus:outline-none focus:ring-2 focus:ring-primary/20"
          />
        </div>
      </div>

      {attributes.map((group) => (
        <div key={group.key}>
          <h3 className="mb-3 text-sm font-semibold text-foreground">{formatKey(group.key)}</h3>
          <div className="max-h-44 space-y-2 overflow-auto pr-1">
            {group.options.map((option) => {
              const checked = selectedAttributes[group.key]?.includes(option.value) ?? false
              return (
                <label key={`${group.key}-${option.value}`} className="flex cursor-pointer items-center gap-2 text-sm text-muted-foreground">
                  <input
                    type="checkbox"
                    checked={checked}
                    onChange={(event) => onAttributeToggle(group.key, option.value, event.target.checked)}
                    className="h-4 w-4 accent-primary"
                  />
                  <span>{option.value}</span>
                  <span className="text-xs text-muted-foreground">({option.count})</span>
                </label>
              )
            })}
          </div>
        </div>
      ))}

      <button
        type="button"
        onClick={onClear}
        className="w-full rounded-lg border border-border bg-background px-4 py-2 text-sm font-medium text-foreground transition hover:bg-muted"
      >
        Clear Filters
      </button>
    </aside>
  )
}
