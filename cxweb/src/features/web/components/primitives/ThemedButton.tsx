import type { ComponentPropsWithoutRef } from "react"

import { Button } from "@/components/ui/button"

type ThemedButtonProps = ComponentPropsWithoutRef<typeof Button>

function ThemedButton(props: ThemedButtonProps) {
  return <Button variant="secondary" size="sm" {...props} />
}

export default ThemedButton
