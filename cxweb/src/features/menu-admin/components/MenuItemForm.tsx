import { useEffect, useState } from "react"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { menuItemTarget, type CreateMenuItemPayload, type MenuItemNodeDto, type UpdateMenuItemPayload } from "@/features/menu-admin/types/menu-types"

type MenuItemFormProps = {
  menuId: string
  parentId?: string | null
  initialItem?: MenuItemNodeDto | null
  onSubmit: (payload: CreateMenuItemPayload | UpdateMenuItemPayload, itemId?: string) => Promise<void>
  onCancel: () => void
}

export function MenuItemForm({ menuId, parentId = null, initialItem, onSubmit, onCancel }: MenuItemFormProps) {
  const [title, setTitle] = useState("")
  const [slug, setSlug] = useState("")
  const [url, setUrl] = useState("/")
  const [target, setTarget] = useState<string>(String(menuItemTarget.Self))
  const [icon, setIcon] = useState("")
  const [description, setDescription] = useState("")
  const [order, setOrder] = useState("0")
  const [isActive, setIsActive] = useState(true)

  useEffect(() => {
    if (!initialItem) {
      setTitle("")
      setSlug("")
      setUrl("/")
      setTarget(String(menuItemTarget.Self))
      setIcon("")
      setDescription("")
      setOrder("0")
      setIsActive(true)
      return
    }

    setTitle(initialItem.title)
    setSlug(initialItem.slug)
    setUrl(initialItem.url)
    setTarget(String(initialItem.target))
    setIcon(initialItem.icon ?? "")
    setDescription(initialItem.description ?? "")
    setOrder(String(initialItem.order))
    setIsActive(initialItem.isActive)
  }, [initialItem])

  const submit = async () => {
    const payloadBase = {
      parentId: initialItem ? (initialItem.parentId ?? null) : parentId,
      title: title.trim(),
      slug: slug.trim().toLowerCase(),
      url: url.trim(),
      target: Number(target) as CreateMenuItemPayload["target"],
      icon: icon.trim().length > 0 ? icon.trim() : null,
      description: description.trim().length > 0 ? description.trim() : null,
      order: Number(order),
      isActive,
    }

    if (initialItem) {
      await onSubmit(payloadBase, initialItem.id)
      return
    }

    await onSubmit({
      menuId,
      tenantId: null,
      ...payloadBase,
    })
  }

  return (
    <div className="grid gap-3 rounded-md border border-border/70 bg-card p-4">
      <Input placeholder="Title" value={title} onChange={(event) => setTitle(event.target.value)} />
      <Input placeholder="Slug" value={slug} onChange={(event) => setSlug(event.target.value)} />
      <Input placeholder="Url" value={url} onChange={(event) => setUrl(event.target.value)} />
      <Select value={target} onValueChange={setTarget}>
        <SelectTrigger className="w-full">
          <SelectValue placeholder="Target" />
        </SelectTrigger>
        <SelectContent>
          <SelectItem value={String(menuItemTarget.Self)}>_self</SelectItem>
          <SelectItem value={String(menuItemTarget.Blank)}>_blank</SelectItem>
        </SelectContent>
      </Select>
      <Input placeholder="Icon" value={icon} onChange={(event) => setIcon(event.target.value)} />
      <Textarea rows={2} placeholder="Description" value={description} onChange={(event) => setDescription(event.target.value)} />
      <Input placeholder="Order" type="number" value={order} onChange={(event) => setOrder(event.target.value)} />
      <label className="inline-flex items-center gap-2 text-sm text-foreground">
        <input checked={isActive} onChange={(event) => setIsActive(event.target.checked)} type="checkbox" />
        Active
      </label>
      <div className="flex gap-2">
        <Button onClick={() => { void submit() }}>{initialItem ? "Update" : "Create"}</Button>
        <Button variant="outline" onClick={onCancel}>Cancel</Button>
      </div>
    </div>
  )
}
