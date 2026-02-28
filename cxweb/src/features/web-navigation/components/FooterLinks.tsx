import { useMemo } from "react"

import { menuGroupType } from "@/features/menu-admin/types/menu-types"
import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

type FooterLinksProps = {
  enabled: boolean
  menuGroupSlug: string
  groups: MenuRenderGroupDto[]
}

function FooterLinks({ enabled, menuGroupSlug, groups }: FooterLinksProps) {
  const links = useMemo(() => {
    if (!enabled) {
      return []
    }

    const group =
      groups.find((entry) => entry.groupSlug === menuGroupSlug) ??
      groups.find((entry) => entry.groupType === menuGroupType.Footer)

    if (!group) {
      return []
    }

    const flattened = group.menus.flatMap((menu) => menu.items)
    const unique = new Map<string, (typeof flattened)[number]>()
    for (const item of flattened) {
      const key = `${item.slug.trim().toLowerCase()}|${item.url.trim().toLowerCase()}`
      if (!unique.has(key)) {
        unique.set(key, item)
      }
    }

    return Array.from(unique.values())
  }, [enabled, groups, menuGroupSlug])

  if (!enabled || links.length === 0) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-footer-foreground">Links</h3>
      <div className="grid gap-1">
        {links.map((item) => (
          <a key={`${item.slug}-${item.url}`} href={item.url} target={item.target === 2 ? "_blank" : "_self"} rel={item.target === 2 ? "noreferrer" : undefined} className="text-sm text-footer-foreground hover:text-footer-foreground/80">
            {item.title}
          </a>
        ))}
      </div>
    </section>
  )
}

export default FooterLinks
