function SliderSkeleton() {
  return (
    <div className="relative w-full overflow-hidden bg-card">
      <div className="h-[70vh] w-full animate-pulse bg-muted" />
      <div className="pointer-events-none absolute inset-0 flex items-center">
        <div className="mx-auto w-full max-w-7xl px-4 sm:px-6">
          <div className="max-w-2xl space-y-3">
            <div className="h-4 w-28 rounded bg-muted-foreground/30" />
            <div className="h-10 w-3/4 rounded bg-muted-foreground/20" />
            <div className="h-5 w-2/3 rounded bg-muted-foreground/20" />
            <div className="h-10 w-32 rounded bg-muted-foreground/20" />
          </div>
        </div>
      </div>
    </div>
  )
}

export default SliderSkeleton
