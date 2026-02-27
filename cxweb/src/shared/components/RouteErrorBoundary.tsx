import { isRouteErrorResponse, Link, useRouteError } from "react-router"

import { BodyText, ButtonWrapper, CardWrapper, PageContainer, SectionContainer, SectionHeader } from "@/shared/components/design-system"

function RouteErrorBoundary() {
  const error = useRouteError()

  const title = isRouteErrorResponse(error) ? `Request failed (${error.status})` : "Unexpected error"

  const description = isRouteErrorResponse(error) ? error.statusText : "An unexpected application error occurred."

  return (
    <div className="grid min-h-screen place-items-center py-10">
      <PageContainer>
        <CardWrapper className="mx-auto w-full max-w-xl">
          <SectionContainer className="border-0 bg-transparent p-0">
            <SectionHeader title={title} />
            <BodyText>{description}</BodyText>
            <div>
              <ButtonWrapper asChild>
                <Link to="/">Return home</Link>
              </ButtonWrapper>
            </div>
          </SectionContainer>
        </CardWrapper>
      </PageContainer>
    </div>
  )
}

export default RouteErrorBoundary
