import { useEffect, useState } from "react"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { menuGroupType, type CreateMenuGroupPayload, type MenuGroupDto, type UpdateMenuGroupPayload } from "@/features/menu-admin/types/menu-types"

type MenuFormProps = {
  mode: "create-group" | "edit-group" | "create-menu" | "edit-menu"
  initialGroup?: MenuGroupDto | null
  onSubmitGroup: (payload: CreateMenuGroupPayload | UpdateMenuGroupPayload) => Promise<void>
  onCancel: () => void
}

export function MenuForm({ mode, initialGroup, onSubmitGroup, onCancel }: MenuFormProps) {
  const [name, setName] = useState("")
  const [slug, setSlug] = useState("")
  const [description, setDescription] = useState("")
  const [isActive, setIsActive] = useState(true)
  const [type, setType] = useState<string>(String(menuGroupType.Header))

  useEffect(() => {
    if (!initialGroup) {
      setName("")
      setSlug("")
      setDescription("")
      setIsActive(true)
      setType(String(menuGroupType.Header))
      return
    }

    setName(initialGroup.name)
    setSlug(initialGroup.slug)
    setDescription(initialGroup.description ?? "")
    setIsActive(initialGroup.isActive)
    setType(String(initialGroup.type))
  }, [initialGroup])

  const submit = async () => {
    const payload = {
      name: name.trim(),
      slug: slug.trim().toLowerCase(),
      description: description.trim().length > 0 ? description.trim() : null,
      isActive,
    }

    if (mode === "create-group") {
      await onSubmitGroup({
        ...payload,
        tenantId: null,
        type: Number(type) as CreateMenuGroupPayload["type"],
      })
      return
    }

    await onSubmitGroup(payload)
  }

  return (
    <div className="grid gap-3 rounded-md border border-border/70 bg-card p-4">
      {mode === "create-group" ? (
        <Select value={type} onValueChange={setType}>
          <SelectTrigger className="w-full">
            <SelectValue placeholder="Select group type" />
          </SelectTrigger>
          <SelectContent>
            <SelectItem value={String(menuGroupType.Header)}>Header</SelectItem>
            <SelectItem value={String(menuGroupType.Footer)}>Footer</SelectItem>
            <SelectItem value={String(menuGroupType.Mobile)}>Mobile</SelectItem>
            <SelectItem value={String(menuGroupType.SideMenu)}>Side Menu</SelectItem>
          </SelectContent>
        </Select>
      ) : null}

      <Input placeholder="Name" value={name} onChange={(event) => setName(event.target.value)} />
      <Input placeholder="Slug" value={slug} onChange={(event) => setSlug(event.target.value)} />
      <Textarea rows={3} placeholder="Description" value={description} onChange={(event) => setDescription(event.target.value)} />

      <label className="inline-flex items-center gap-2 text-sm text-foreground">
        <input checked={isActive} onChange={(event) => setIsActive(event.target.checked)} type="checkbox" />
        Active
      </label>

      <div className="flex gap-2">
        <Button onClick={() => { void submit() }}>{mode.startsWith("create") ? "Create" : "Update"}</Button>
        <Button variant="outline" onClick={onCancel}>Cancel</Button>
      </div>
    </div>
  )
}
