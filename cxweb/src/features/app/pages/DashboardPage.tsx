import { useEffect, useState } from "react"

import { BodyText, CardWrapper, PageContainer, SectionContainer, SectionHeader } from "@/shared/components/design-system"
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
    <PageContainer width="wide">
      <CardWrapper>
        <SectionContainer className="border-0 bg-transparent p-0">
          <SectionHeader title="Dashboard" subtitle="Application shell is active and ready for feature modules." />
          <BodyText>Backend status: {status}</BodyText>
        </SectionContainer>
      </CardWrapper>
    </PageContainer>
  )
}

export default DashboardPage
