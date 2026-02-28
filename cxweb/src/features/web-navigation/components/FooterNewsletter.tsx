import { useState } from "react"
import type { FormEvent } from "react"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

type FooterNewsletterProps = {
  enabled: boolean
  title: string
  description: string
}

function FooterNewsletter({ enabled, title, description }: FooterNewsletterProps) {
  const [email, setEmail] = useState("")

  if (!enabled) {
    return null
  }

  const handleSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
    setEmail("")
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-footer-foreground">{title}</h3>
      <p className="text-sm text-muted-foreground">{description}</p>
      <form className="flex gap-2" onSubmit={handleSubmit}>
        <Input value={email} onChange={(event) => setEmail(event.target.value)} type="email" placeholder="Email address" className="h-9 bg-footer-bg" required />
        <Button type="submit" className="h-9 bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
          Join
        </Button>
      </form>
    </section>
  )
}

export default FooterNewsletter
