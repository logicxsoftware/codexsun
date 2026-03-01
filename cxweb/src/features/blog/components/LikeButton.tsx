import { useState } from "react"

import { showToast } from "@/shared/components/ui/toast/showToast"

type LikeButtonProps = {
  initialCount: number
  onToggle: (liked: boolean) => Promise<void>
}

export default function LikeButton({ initialCount, onToggle }: LikeButtonProps) {
  const [liked, setLiked] = useState<boolean>(false)
  const [count, setCount] = useState<number>(initialCount)
  const [busy, setBusy] = useState<boolean>(false)

  const toggle = async () => {
    const next = !liked

    try {
      setBusy(true)
      await onToggle(next)
      setLiked(next)
      setCount((current) => Math.max(0, current + (next ? 1 : -1)))
    } catch {
      showToast({ title: "Like action failed", variant: "error" })
    } finally {
      setBusy(false)
    }
  }

  return (
    <button
      type="button"
      disabled={busy}
      onClick={() => {
        void toggle()
      }}
      className="rounded-lg border border-border bg-card px-4 py-2 text-sm text-card-foreground transition hover:bg-muted disabled:cursor-not-allowed disabled:opacity-60"
    >
      {liked ? "Unlike" : "Like"} ({count})
    </button>
  )
}
