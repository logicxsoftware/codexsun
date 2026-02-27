import { useState } from "react"
import type { FormEvent } from "react"
import { Link } from "react-router"

import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
  }

  return (
    <div className="space-y-5">
      <div className="space-y-1">
        <h1 className="text-2xl font-semibold">Sign in</h1>
        <p className="text-sm text-muted-foreground">Use your organization credentials to access application modules.</p>
      </div>

      <form className="space-y-4" onSubmit={onSubmit}>
        <div className="space-y-2">
          <label htmlFor="email" className="text-sm font-medium">
            Email
          </label>
          <Input
            id="email"
            type="email"
            value={email}
            onChange={(event) => setEmail(event.target.value)}
            autoComplete="email"
            required
          />
        </div>

        <div className="space-y-2">
          <label htmlFor="password" className="text-sm font-medium">
            Password
          </label>
          <Input
            id="password"
            type="password"
            value={password}
            onChange={(event) => setPassword(event.target.value)}
            autoComplete="current-password"
            required
          />
        </div>

        <Button type="submit" className="w-full">
          Continue
        </Button>
      </form>

      <p className="text-sm text-muted-foreground">
        Need public site access? <Link to="/" className="text-foreground underline">Return to website</Link>
      </p>
    </div>
  )
}

export default LoginPage
