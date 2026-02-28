type ProductSpecificationsProps = {
  specifications: Record<string, string>
}

const formatSpecificationKey = (key: string): string => key.split("_").map((part) => part.charAt(0).toUpperCase() + part.slice(1)).join(" ")

export default function ProductSpecifications({ specifications }: ProductSpecificationsProps) {
  const entries = Object.entries(specifications)
  if (entries.length === 0) {
    return null
  }

  return (
    <div className="mt-6">
      <h2 className="mb-2 text-sm font-semibold text-foreground">Specifications</h2>
      <div className="grid grid-cols-1 gap-2 sm:grid-cols-2">
        {entries.map(([key, value]) => (
          <div key={key} className="rounded-lg border border-border bg-background px-3 py-2 text-sm">
            <span className="text-muted-foreground">{formatSpecificationKey(key)}:</span> <span className="text-foreground">{value}</span>
          </div>
        ))}
      </div>
    </div>
  )
}
