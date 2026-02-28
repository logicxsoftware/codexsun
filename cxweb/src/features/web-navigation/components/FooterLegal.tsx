import type { FooterLinkItem } from "@/features/web-navigation/types/navigation-types"

type FooterLegalProps = {
  enabled: boolean
  items: FooterLinkItem[]
}

function FooterLegal({ enabled, items }: FooterLegalProps) {
  if (!enabled || items.length === 0) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-footer-foreground">Legal</h3>
      <div className="grid gap-1">
        {items.map((item) => (
          <a key={`${item.label}-${item.url}`} href={item.url} target={item.target} rel={item.target === "_blank" ? "noreferrer" : undefined} className="text-sm text-footer-foreground hover:text-footer-foreground/80">
            {item.label}
          </a>
        ))}
      </div>
    </section>
  )
}

export default FooterLegal
