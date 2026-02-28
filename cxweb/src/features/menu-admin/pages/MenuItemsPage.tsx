import { useEffect, useMemo, useState } from "react"
import { Link, useParams } from "react-router"

import { Button } from "@/components/ui/button"
import { showToast } from "@/shared/components/ui/toast"
import { MenuItemForm } from "@/features/menu-admin/components/MenuItemForm"
import { MenuItemTree } from "@/features/menu-admin/components/MenuItemTree"
import { menuApi } from "@/features/menu-admin/services/menu-api"
import type { CreateMenuItemPayload, MenuItemNodeDto, UpdateMenuItemPayload } from "@/features/menu-admin/types/menu-types"
import { collectReorderPayload, updateNodeOrder } from "@/features/menu-admin/utils/menu-tree"

function MenuItemsPage() {
  const { id } = useParams()
  const menuId = id ?? ""

  const [nodes, setNodes] = useState<MenuItemNodeDto[]>([])
  const [editing, setEditing] = useState<MenuItemNodeDto | null>(null)
  const [createParentId, setCreateParentId] = useState<string | null>(null)

  const load = async () => {
    if (!menuId) {
      return
    }

    const data = await menuApi.getMenuItemTree(menuId, false)
    setNodes(data)
  }

  useEffect(() => {
    void load()
  }, [menuId])

  const submit = async (payload: CreateMenuItemPayload | UpdateMenuItemPayload, itemId?: string) => {
    if (itemId) {
      await menuApi.updateMenuItem(itemId, payload as UpdateMenuItemPayload)
      showToast({ title: "Menu item updated", variant: "success" })
      setEditing(null)
    } else {
      await menuApi.createMenuItem(payload as CreateMenuItemPayload)
      showToast({ title: "Menu item created", variant: "success" })
      setCreateParentId(null)
    }

    await load()
  }

  const remove = async (itemId: string) => {
    await menuApi.deleteMenuItem(itemId)
    showToast({ title: "Menu item deleted", variant: "success" })
    await load()
  }

  const reorderSave = async (nextNodes: MenuItemNodeDto[]) => {
    setNodes(nextNodes)
    await menuApi.reorderMenuItems(menuId, {
      items: collectReorderPayload(nextNodes),
    })
  }

  const treeKey = useMemo(() => nodes.map((node) => node.id).join("-"), [nodes])

  return (
    <section className="grid gap-4">
      <div className="grid gap-1">
        <h1 className="text-xl font-semibold tracking-tight">Menu Items</h1>
        <Link className="text-sm text-link hover:text-link-hover" to="/admin/menu-groups">Back to Menu Groups</Link>
      </div>

      {createParentId !== null ? (
        <MenuItemForm
          menuId={menuId}
          parentId={createParentId}
          onSubmit={submit}
          onCancel={() => {
            setCreateParentId(null)
          }}
        />
      ) : null}

      {editing ? (
        <MenuItemForm
          menuId={menuId}
          initialItem={editing}
          onSubmit={submit}
          onCancel={() => {
            setEditing(null)
          }}
        />
      ) : null}

      <MenuItemTree
        key={treeKey}
        nodes={nodes}
        onEdit={setEditing}
        onDelete={(itemId) => {
          void remove(itemId)
        }}
        onAddChild={(parentId) => {
          setEditing(null)
          setCreateParentId(parentId)
        }}
        onMoveUp={(itemId) => {
          const next = updateNodeOrder(nodes, itemId, "up")
          void reorderSave(next)
        }}
        onMoveDown={(itemId) => {
          const next = updateNodeOrder(nodes, itemId, "down")
          void reorderSave(next)
        }}
      />

      <div>
        <Button onClick={() => setCreateParentId(null)}>Create Root Item</Button>
      </div>
    </section>
  )
}

export default MenuItemsPage
