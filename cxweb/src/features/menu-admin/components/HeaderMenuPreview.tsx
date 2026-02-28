import type { MenuRenderGroupDto } from "@/features/menu-admin/types/menu-types"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

type HeaderMenuPreviewProps = {
  group?: MenuRenderGroupDto
}

export function HeaderMenuPreview({ group }: HeaderMenuPreviewProps) {
  return (
    <Card className="border-border/70 bg-card">
      <CardHeader>
        <CardTitle>Header Menu Preview</CardTitle>
      </CardHeader>
      <CardContent>
        <div className="flex flex-wrap gap-2 text-sm">
          {group?.menus.flatMap((menu) => menu.items).map((item) => (
            <span key={`${item.slug}-${item.url}`} className="rounded-md border border-border/70 bg-background px-2 py-1">
              {item.title}
            </span>
          ))}
        </div>
      </CardContent>
    </Card>
  )
}
