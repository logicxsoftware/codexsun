import { useEffect, useState } from "react"
import { Link, useNavigate, useParams } from "react-router"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { showToast } from "@/shared/components/ui/toast"
import { menuApi } from "@/features/menu-admin/services/menu-api"
import { menuVariant, type CreateMenuPayload, type MenuDto, type UpdateMenuPayload } from "@/features/menu-admin/types/menu-types"

function MenuGroupMenusPage() {
  const { id } = useParams()
  const navigate = useNavigate()
  const [menus, setMenus] = useState<MenuDto[]>([])
  const [editingId, setEditingId] = useState<string | null>(null)
  const [name, setName] = useState("")
  const [slug, setSlug] = useState("")
  const [order, setOrder] = useState("0")
  const [isMegaMenu, setIsMegaMenu] = useState(false)
  const [isActive, setIsActive] = useState(true)

  const menuGroupId = id ?? ""

  const load = async () => {
    if (!menuGroupId) {
      return
    }
    const data = await menuApi.getMenusByGroup(menuGroupId, false)
    setMenus(data)
  }

  useEffect(() => {
    void load()
  }, [menuGroupId])

  const reset = () => {
    setEditingId(null)
    setName("")
    setSlug("")
    setOrder("0")
    setIsMegaMenu(false)
    setIsActive(true)
  }

  const submit = async () => {
    if (!menuGroupId) {
      return
    }

    if (editingId) {
      const payload: UpdateMenuPayload = {
        name: name.trim(),
        slug: slug.trim().toLowerCase(),
        variant: menuVariant.Custom,
        isMegaMenu,
        order: Number(order),
        isActive,
      }

      await menuApi.updateMenu(editingId, payload)
      showToast({ title: "Menu updated", variant: "success" })
    } else {
      const payload: CreateMenuPayload = {
        menuGroupId,
        tenantId: null,
        name: name.trim(),
        slug: slug.trim().toLowerCase(),
        variant: menuVariant.Custom,
        isMegaMenu,
        order: Number(order),
        isActive,
      }

      await menuApi.createMenu(payload)
      showToast({ title: "Menu created", variant: "success" })
    }

    reset()
    await load()
  }

  return (
    <section className="grid gap-4">
      <div className="flex items-center justify-between">
        <div className="grid gap-1">
          <h1 className="text-xl font-semibold tracking-tight">Menus</h1>
          <Link className="text-sm text-link hover:text-link-hover" to="/admin/menu-groups">Back to Menu Groups</Link>
        </div>
        <Button variant="outline" onClick={() => navigate(-1)}>Back</Button>
      </div>

      <div className="grid gap-3 rounded-md border border-border/70 bg-card p-4 md:grid-cols-2">
        <Input placeholder="Name" value={name} onChange={(event) => setName(event.target.value)} />
        <Input placeholder="Slug" value={slug} onChange={(event) => setSlug(event.target.value)} />
        <Input placeholder="Order" type="number" value={order} onChange={(event) => setOrder(event.target.value)} />
        <div className="flex items-center gap-4">
          <label className="inline-flex items-center gap-2 text-sm">
            <input type="checkbox" checked={isMegaMenu} onChange={(event) => setIsMegaMenu(event.target.checked)} />
            Mega Menu
          </label>
          <label className="inline-flex items-center gap-2 text-sm">
            <input type="checkbox" checked={isActive} onChange={(event) => setIsActive(event.target.checked)} />
            Active
          </label>
        </div>
        <div className="flex gap-2 md:col-span-2">
          <Button onClick={() => { void submit() }}>{editingId ? "Update" : "Create"}</Button>
          <Button variant="outline" onClick={reset}>Reset</Button>
        </div>
      </div>

      <div className="grid gap-2">
        {menus.map((menu) => (
          <div key={menu.id} className="flex flex-col gap-2 rounded-md border border-border/70 bg-background p-3 md:flex-row md:items-center md:justify-between">
            <div className="grid gap-1">
              <div className="text-sm font-semibold">{menu.name}</div>
              <div className="text-xs text-muted-foreground">{menu.slug} • order {menu.order} • {menu.isMegaMenu ? "mega" : "standard"}</div>
            </div>
            <div className="flex flex-wrap gap-2">
              <Button size="sm" variant="secondary" onClick={() => navigate(`/admin/menus/${menu.id}/items`)}>Items</Button>
              <Button size="sm" variant="outline" onClick={() => {
                setEditingId(menu.id)
                setName(menu.name)
                setSlug(menu.slug)
                setOrder(String(menu.order))
                setIsMegaMenu(menu.isMegaMenu)
                setIsActive(menu.isActive)
              }}>Edit</Button>
              <Button size="sm" variant="destructive" onClick={() => { void menuApi.deleteMenu(menu.id).then(load) }}>Delete</Button>
            </div>
          </div>
        ))}
      </div>
    </section>
  )
}

export default MenuGroupMenusPage
