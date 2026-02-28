import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type MegaMenuPreviewProps = {
  group?: MenuRenderGroupDto
}

export function MegaMenuPreview({ group }: MegaMenuPreviewProps) {
  return (
    <Card className="border-border/70 bg-card">
      <CardHeader>
        <CardTitle>Mega Menu Preview</CardTitle>
      </CardHeader>
      <CardContent className="grid gap-3 md:grid-cols-2">
        {group?.menus.filter((menu) => menu.isMegaMenu).map((menu) => (
          <div key={menu.slug} className="grid gap-2 rounded-md border border-border/70 bg-background p-3">
            <div className="text-sm font-semibold text-foreground">{menu.name}</div>
            <div className="grid gap-1 text-xs text-muted-foreground">
              {menu.items.map((item) => (
                <div key={`${menu.slug}-${item.slug}`}>
                  {item.title}
                </div>
              ))}
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  )
}
