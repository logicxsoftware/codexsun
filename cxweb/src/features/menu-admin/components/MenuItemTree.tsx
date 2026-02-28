import { ChevronDown, ChevronRight } from "lucide-react"
import { useState } from "react"

import { Button } from "@/components/ui/button"
import type { MenuItemNodeDto } from "@/features/menu-admin/types/menu-types"

type MenuItemTreeProps = {
  nodes: MenuItemNodeDto[]
  onEdit: (item: MenuItemNodeDto) => void
  onDelete: (itemId: string) => void
  onAddChild: (parentId: string | null) => void
  onMoveUp: (itemId: string) => void
  onMoveDown: (itemId: string) => void
}

type TreeNodeProps = {
  node: MenuItemNodeDto
  depth: number
  onEdit: (item: MenuItemNodeDto) => void
  onDelete: (itemId: string) => void
  onAddChild: (parentId: string | null) => void
  onMoveUp: (itemId: string) => void
  onMoveDown: (itemId: string) => void
}

function TreeNode({ node, depth, onEdit, onDelete, onAddChild, onMoveUp, onMoveDown }: TreeNodeProps) {
  const [expanded, setExpanded] = useState(true)
  const hasChildren = node.children.length > 0

  return (
    <div className="grid gap-2">
      <div className="flex flex-col gap-2 rounded-md border border-border/70 bg-background p-2 md:flex-row md:items-center md:justify-between" style={{ marginLeft: `${depth * 12}px` }}>
        <div className="flex items-center gap-2">
          {hasChildren ? (
            <Button size="icon-xs" variant="ghost" onClick={() => setExpanded((prev) => !prev)}>
              {expanded ? <ChevronDown className="h-3 w-3" /> : <ChevronRight className="h-3 w-3" />}
            </Button>
          ) : (
            <span className="inline-flex h-6 w-6" />
          )}
          <div className="grid gap-0.5">
            <span className="text-sm font-medium text-foreground">{node.title}</span>
            <span className="text-xs text-muted-foreground">{node.slug} • {node.url}</span>
          </div>
        </div>
        <div className="flex flex-wrap gap-1">
          <Button size="sm" variant="ghost" onClick={() => onMoveUp(node.id)}>Up</Button>
          <Button size="sm" variant="ghost" onClick={() => onMoveDown(node.id)}>Down</Button>
          <Button size="sm" variant="secondary" onClick={() => onAddChild(node.id)}>Add Child</Button>
          <Button size="sm" variant="outline" onClick={() => onEdit(node)}>Edit</Button>
          <Button size="sm" variant="destructive" onClick={() => onDelete(node.id)}>Delete</Button>
        </div>
      </div>

      {expanded && hasChildren
        ? node.children.map((child) => (
            <TreeNode
              key={child.id}
              node={child}
              depth={depth + 1}
              onEdit={onEdit}
              onDelete={onDelete}
              onAddChild={onAddChild}
              onMoveUp={onMoveUp}
              onMoveDown={onMoveDown}
            />
          ))
        : null}
    </div>
  )
}

export function MenuItemTree({ nodes, onEdit, onDelete, onAddChild, onMoveUp, onMoveDown }: MenuItemTreeProps) {
  return (
    <div className="grid gap-2">
      <div className="flex justify-end">
        <Button size="sm" onClick={() => onAddChild(null)}>Add Root Item</Button>
      </div>
      {nodes.map((node) => (
        <TreeNode
          key={node.id}
          node={node}
          depth={0}
          onEdit={onEdit}
          onDelete={onDelete}
          onAddChild={onAddChild}
          onMoveUp={onMoveUp}
          onMoveDown={onMoveDown}
        />
      ))}
    </div>
  )
}
