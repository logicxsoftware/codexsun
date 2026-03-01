import { useState } from "react"

import { showToast } from "@/shared/components/ui/toast/showToast"

type CommentFormProps = {
  onSubmit: (value: string) => Promise<void>
}

export default function CommentForm({ onSubmit }: CommentFormProps) {
  const [body, setBody] = useState<string>("")
  const [busy, setBusy] = useState<boolean>(false)

  const submit = async (event: React.FormEvent<HTMLFormElement>) => {
    event.preventDefault()

    const normalized = body.trim()
    if (normalized.length < 2) {
      showToast({ title: "Comment too short", variant: "warning" })
      return
    }

    try {
      setBusy(true)
      await onSubmit(normalized)
      setBody("")
      showToast({ title: "Comment added", variant: "success" })
    } catch {
      showToast({ title: "Comment failed", variant: "error" })
    } finally {
      setBusy(false)
    }
  }

  return (
    <form onSubmit={submit} className="space-y-3 rounded-xl border border-border bg-card p-4">
      <label htmlFor="blog-comment" className="text-sm font-medium text-card-foreground">
        Add comment
      </label>
      <textarea
        id="blog-comment"
        value={body}
        onChange={(event) => setBody(event.target.value)}
        rows={4}
        className="w-full rounded-lg border border-border bg-background px-3 py-2 text-sm text-foreground focus:border-primary focus:outline-none"
        placeholder="Share your thoughts"
      />
      <button
        type="submit"
        disabled={busy}
        className="rounded-lg bg-primary px-4 py-2 text-sm font-medium text-primary-foreground transition hover:bg-primary/90 disabled:cursor-not-allowed disabled:opacity-60"
      >
        {busy ? "Submitting..." : "Submit"}
      </button>
    </form>
  )
}
