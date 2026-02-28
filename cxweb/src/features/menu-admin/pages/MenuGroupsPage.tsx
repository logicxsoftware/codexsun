import { useEffect, useState } from "react"
import { useNavigate } from "react-router"

import { showToast } from "@/shared/components/ui/toast"
import { MenuForm } from "@/features/menu-admin/components/MenuForm"
import { MenuGroupList } from "@/features/menu-admin/components/MenuGroupList"
import { menuApi } from "@/features/menu-admin/services/menu-api"
import type { CreateMenuGroupPayload, MenuGroupDto, UpdateMenuGroupPayload } from "@/features/menu-admin/types/menu-types"

function MenuGroupsPage() {
  const navigate = useNavigate()
  const [groups, setGroups] = useState<MenuGroupDto[]>([])
  const [editing, setEditing] = useState<MenuGroupDto | null>(null)
  const [showCreate, setShowCreate] = useState(false)

  const load = async () => {
    const data = await menuApi.getMenuGroups(true, false)
    setGroups(data)
  }

  useEffect(() => {
    void load()
  }, [])

  const submit = async (payload: CreateMenuGroupPayload | UpdateMenuGroupPayload) => {
    if (editing) {
      await menuApi.updateMenuGroup(editing.id, payload as UpdateMenuGroupPayload)
      showToast({ title: "Menu group updated", variant: "success" })
      setEditing(null)
    } else {
      await menuApi.createMenuGroup(payload as CreateMenuGroupPayload)
      showToast({ title: "Menu group created", variant: "success" })
      setShowCreate(false)
    }

    await load()
  }

  const remove = async (groupId: string) => {
    await menuApi.deleteMenuGroup(groupId)
    showToast({ title: "Menu group deleted", variant: "success" })
    await load()
  }

  return (
    <section className="grid gap-4">
      <div className="flex flex-wrap items-center justify-between gap-2">
        <h1 className="text-xl font-semibold tracking-tight">Menu Groups</h1>
        <button
          type="button"
          className="inline-flex h-9 items-center rounded-md bg-primary px-3 text-sm font-medium text-primary-foreground hover:bg-primary/90"
          onClick={() => {
            setShowCreate((prev) => !prev)
            setEditing(null)
          }}
        >
          {showCreate ? "Close" : "Create Group"}
        </button>
      </div>

      {showCreate ? <MenuForm mode="create-group" onSubmitGroup={submit} onCancel={() => setShowCreate(false)} /> : null}
      {editing ? <MenuForm mode="edit-group" initialGroup={editing} onSubmitGroup={submit} onCancel={() => setEditing(null)} /> : null}

      <MenuGroupList
        groups={groups}
        onEdit={setEditing}
        onDelete={(groupId) => {
          void remove(groupId)
        }}
        onOpenMenus={(group) => {
          void navigate(`/admin/menu-groups/${group.id}/menus`)
        }}
      />
    </section>
  )
}

export default MenuGroupsPage
