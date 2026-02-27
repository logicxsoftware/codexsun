import { useEffect, useState } from "react"

import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { httpClient } from "@/shared/services/http-client"

type HealthResponse = {
  status?: string
}

function DashboardPage() {
  const [status, setStatus] = useState<string>("Checking")

  useEffect(() => {
    let active = true

    const run = async () => {
      try {
        await httpClient.get<HealthResponse>("/alive")
        if (active) {
          setStatus("Operational")
        }
      } catch {
        if (active) {
          setStatus("Unavailable")
        }
      }
    }

    void run()

    return () => {
      active = false
    }
  }, [])

  return (
    <section className="mx-auto w-full max-w-5xl">
      <Card>
        <CardHeader>
          <CardTitle>Dashboard</CardTitle>
          <CardDescription>Application shell is active and ready for feature modules.</CardDescription>
        </CardHeader>
        <CardContent>
          <p className="text-sm text-muted-foreground">Backend status: {status}</p>
        </CardContent>
      </Card>
    </section>
  )
}

export default DashboardPage
