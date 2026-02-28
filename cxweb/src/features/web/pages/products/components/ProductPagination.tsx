import { cn } from "@/lib/utils"

type ProductPaginationProps = {
  page: number
  totalPages: number
  hasPrevious: boolean
  hasNext: boolean
  onPageChange: (page: number) => void
}

const buildPages = (page: number, totalPages: number): number[] => {
  const pages = new Set<number>()
  pages.add(1)
  pages.add(totalPages)
  for (let i = page - 2; i <= page + 2; i += 1) {
    if (i >= 1 && i <= totalPages) {
      pages.add(i)
    }
  }
  return [...pages].sort((a, b) => a - b)
}

export default function ProductPagination({ page, totalPages, hasPrevious, hasNext, onPageChange }: ProductPaginationProps) {
  if (totalPages <= 1) {
    return null
  }

  const pages = buildPages(page, totalPages)

  return (
    <nav className="mt-8 flex flex-wrap items-center justify-center gap-2" aria-label="Pagination">
      <button
        type="button"
        disabled={!hasPrevious}
        onClick={() => onPageChange(page - 1)}
        className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-foreground transition hover:bg-muted disabled:cursor-not-allowed disabled:opacity-50"
      >
        Prev
      </button>

      {pages.map((number, index) => {
        const previous = pages[index - 1]
        const showGap = previous && number - previous > 1

        return (
          <span key={number} className="contents">
            {showGap ? <span className="px-1 text-sm text-muted-foreground">...</span> : null}
            <button
              type="button"
              onClick={() => onPageChange(number)}
              className={cn(
                "rounded-lg border px-3 py-2 text-sm transition",
                number === page
                  ? "border-primary bg-primary text-primary-foreground"
                  : "border-border bg-card text-foreground hover:bg-muted",
              )}
            >
              {number}
            </button>
          </span>
        )
      })}

      <button
        type="button"
        disabled={!hasNext}
        onClick={() => onPageChange(page + 1)}
        className="rounded-lg border border-border bg-card px-3 py-2 text-sm text-foreground transition hover:bg-muted disabled:cursor-not-allowed disabled:opacity-50"
      >
        Next
      </button>
    </nav>
  )
}
