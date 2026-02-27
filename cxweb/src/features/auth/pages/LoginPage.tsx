import { useState } from "react"
import type { FormEvent } from "react"
import { Link } from "react-router"

import { Input } from "@/components/ui/input"
import { BodyText, ButtonWrapper, FormGroup, SectionHeader } from "@/shared/components/design-system"

function LoginPage() {
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")

  const onSubmit = (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault()
  }

  return (
    <div className="grid gap-5">
      <SectionHeader title="Sign in" subtitle="Use your organization credentials to access application modules." />

      <form className="grid gap-4" onSubmit={onSubmit}>
        <FormGroup>
          <label htmlFor="email" className="text-sm font-medium text-foreground">
            Email
          </label>
          <Input id="email" type="email" value={email} onChange={(event) => setEmail(event.target.value)} autoComplete="email" required />
        </FormGroup>

        <FormGroup>
          <label htmlFor="password" className="text-sm font-medium text-foreground">
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
        </FormGroup>

        <ButtonWrapper type="submit" block>
          Continue
        </ButtonWrapper>
      </form>

      <BodyText>
        Need public site access?{" "}
        <Link to="/" className="text-link hover:text-link-hover">
          Return to website
        </Link>
      </BodyText>
    </div>
  )
}

export default LoginPage
