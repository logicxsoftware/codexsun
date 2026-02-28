import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type FooterMenuPreviewProps = {
  group?: MenuRenderGroupDto
}

export function FooterMenuPreview({ group }: FooterMenuPreviewProps) {
  return (
    <Card className="border-border/70 bg-card">
      <CardHeader>
        <CardTitle>Footer Menu Preview</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid gap-2 text-sm">
          {group?.menus.flatMap((menu) => menu.items).map((item) => (
            <div key={`${item.slug}-${item.url}`} className="text-link hover:text-link-hover">
              {item.title}
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
