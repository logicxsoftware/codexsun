import { Outlet } from "react-router"

import { BodyText, CardWrapper, PageContainer, SectionContainer, SectionHeader } from "@/shared/components/design-system"

function AuthLayout() {
  return (
    <div className="grid min-h-screen place-items-center bg-muted/35 py-10">
      <PageContainer>
        <CardWrapper className="mx-auto w-full max-w-md">
          <SectionContainer className="border-0 bg-transparent p-0">
            <SectionHeader title="Authentication" />
            <BodyText>Sign in to access protected application modules.</BodyText>
            <Outlet />
          </SectionContainer>
        </CardWrapper>
      </PageContainer>
    </div>
  )
}

export default AuthLayout
