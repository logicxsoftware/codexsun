import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type MobileMenuPreviewProps = {
  group?: MenuRenderGroupDto
}

export function MobileMenuPreview({ group }: MobileMenuPreviewProps) {
  return (
    <Card className="border-border/70 bg-card">
      <CardHeader>
        <CardTitle>Mobile Menu Preview</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="grid gap-1 text-sm">
          {group?.menus.flatMap((menu) => menu.items).map((item) => (
            <div key={`${item.slug}-${item.url}`} className="rounded-md border border-border/70 bg-background px-2 py-1">
              {item.title}
            </div>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
