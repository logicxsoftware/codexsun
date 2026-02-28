import { Globe } from "lucide-react"

import type { FooterSocialItem } from "@/features/web-navigation/types/navigation-types"

type FooterSocialProps = {
  enabled: boolean
  items: FooterSocialItem[]
}

function FooterSocial({ enabled, items }: FooterSocialProps) {
  if (!enabled || items.length === 0) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-footer-foreground">Social</h3>
      <div className="grid gap-1">
        {items.map((item) => (
          <a key={`${item.label}-${item.url}`} href={item.url} target={item.target} rel={item.target === "_blank" ? "noreferrer" : undefined} className="inline-flex items-center gap-2 text-sm text-footer-foreground hover:text-footer-foreground/80">
            <Globe className="h-3.5 w-3.5" />
            {item.label}
          </a>
        ))}
      </div>
    </section>
  )
}

export default FooterSocial
