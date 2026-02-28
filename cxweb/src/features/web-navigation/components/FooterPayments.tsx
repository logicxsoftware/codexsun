type FooterPaymentsProps = {
  enabled: boolean
  providers: string[]
}

function FooterPayments({ enabled, providers }: FooterPaymentsProps) {
  if (!enabled || providers.length === 0) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-foreground">Payments</h3>
      <div className="flex flex-wrap gap-2">
        {providers.map((provider) => (
          <span key={provider} className="inline-flex rounded-md border border-border bg-card px-2 py-1 text-xs text-muted-foreground">
            {provider}
          </span>
        ))}
      </div>
    </section>
  )
}

export default FooterPayments
