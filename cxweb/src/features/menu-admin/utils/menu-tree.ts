import type { MenuItemNodeDto } from "@/features/menu-admin/types/menu-types"

export type FlatMenuNode = {
  id: string
  parentId: string | null
  depth: number
  title: string
  order: number
}

export const flattenMenuTree = (nodes: MenuItemNodeDto[], depth = 0): FlatMenuNode[] => {
  return nodes.flatMap((node) => [
    {
      id: node.id,
      parentId: node.parentId,
      depth,
      title: node.title,
      order: node.order,
    },
    ...flattenMenuTree(node.children, depth + 1),
  ])
}

export const collectReorderPayload = (nodes: MenuItemNodeDto[]) => {
  const result: Array<{ itemId: string; parentId: string | null; order: number }> = []

  const walk = (items: MenuItemNodeDto[], parentId: string | null) => {
    items.forEach((item, index) => {
      result.push({
        itemId: item.id,
        parentId,
        order: index,
      })

      walk(item.children, item.id)
    })
  }

  walk(nodes, null)

  return result
}

export const updateNodeOrder = (nodes: MenuItemNodeDto[], nodeId: string, direction: "up" | "down"): MenuItemNodeDto[] => {
  const moveInArray = (items: MenuItemNodeDto[]) => {
    const index = items.findIndex((item) => item.id === nodeId)
    if (index === -1) {
      return items
    }

    const targetIndex = direction === "up" ? index - 1 : index + 1
    if (targetIndex < 0 || targetIndex >= items.length) {
      return items
    }

    const next = [...items]
    const [moved] = next.splice(index, 1)
    next.splice(targetIndex, 0, moved)

    return next.map((item, order) => ({
      ...item,
      order,
    }))
  }

  const apply = (items: MenuItemNodeDto[]): MenuItemNodeDto[] => {
    const local = moveInArray(items)

    return local.map((item) => ({
      ...item,
      children: apply(item.children),
    }))
  }

  return apply(nodes)
}
