import { useMemo } from "react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { menuGroupType, type MenuGroupDto } from "@/features/menu-admin/types/menu-types"

type MenuGroupListProps = {
  groups: MenuGroupDto[]
  onEdit: (group: MenuGroupDto) => void
  onDelete: (groupId: string) => void
  onOpenMenus: (group: MenuGroupDto) => void
}

const typeLabelMap: Record<number, string> = {
  [menuGroupType.Header]: "Header",
  [menuGroupType.Footer]: "Footer",
  [menuGroupType.Mobile]: "Mobile",
  [menuGroupType.SideMenu]: "Side Menu",
}

export function MenuGroupList({ groups, onEdit, onDelete, onOpenMenus }: MenuGroupListProps) {
  const sortedGroups = useMemo(
    () => [...groups].sort((a, b) => a.type - b.type || a.name.localeCompare(b.name)),
    [groups],
  )

  return (
    <Card className="border-border/70 bg-card">
      <CardHeader>
        <CardTitle>Menu Groups</CardTitle>
      </CardHeader>
      <CardContent className="grid gap-3">
        {sortedGroups.map((group) => (
          <div key={group.id} className="flex flex-col gap-2 rounded-md border border-border/70 bg-background p-3 md:flex-row md:items-center md:justify-between">
            <div className="grid gap-1">
              <div className="text-sm font-semibold text-foreground">{group.name}</div>
              <div className="text-xs text-muted-foreground">{typeLabelMap[group.type]} • {group.slug} • {group.tenantId ? "Tenant" : "Global"}</div>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button size="sm" variant="secondary" onClick={() => onOpenMenus(group)}>
                Menus
              </Button>
              <Button size="sm" variant="outline" onClick={() => onEdit(group)}>
                Edit
              </Button>
              <Button size="sm" variant="destructive" onClick={() => onDelete(group.id)}>
                Delete
              </Button>
            </div>
          </div>
        ))}
      </CardContent>
    </Card>
  )
}
