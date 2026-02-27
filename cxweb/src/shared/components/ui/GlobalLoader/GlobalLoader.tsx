type GlobalLoaderProps = {
  active: boolean
}

export function GlobalLoader({ active }: GlobalLoaderProps) {
  if (!active) {
    return null
  }

  return (
    <div
      aria-busy="true"
      aria-live="polite"
      className="pointer-events-none fixed inset-0 z-50 flex items-center justify-center bg-background/65 backdrop-blur-sm"
      role="status"
    >
      <div className="relative flex h-24 w-24 items-center justify-center">
        <div className="h-24 w-24 animate-spin rounded-full border-[6px] border-muted border-t-brand" />
        <div className="absolute flex h-14 w-14 items-center justify-center rounded-full border-2 border-border bg-card text-xl font-semibold text-brand">
          C
        </div>
      </div>
    </div>
  )
}
