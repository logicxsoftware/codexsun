type FooterBottomBarProps = {
  enabled: boolean
  showDynamicYear: boolean
  copyright: string
  developedBy: {
    enabled: boolean
    label: string
    url: string
  }
}

function FooterBottomBar({ enabled, showDynamicYear, copyright, developedBy }: FooterBottomBarProps) {
  if (!enabled) {
    return null
  }

  const year = showDynamicYear ? ` ${new Date().getFullYear()}` : ""

  return (
    <section className="mt-6 flex flex-col gap-1 border-t border-border/35 pt-4 text-sm md:flex-row md:items-center md:justify-between">
      <p className="text-footer-foreground">
        {copyright}
        {year}
      </p>
      {developedBy.enabled && developedBy.label && developedBy.url ? (
        <a href={developedBy.url} target="_blank" rel="noreferrer" className="text-footer-foreground hover:text-footer-foreground/80">
          {developedBy.label}
        </a>
      ) : null}
    </section>
  )
}

export default FooterBottomBar
