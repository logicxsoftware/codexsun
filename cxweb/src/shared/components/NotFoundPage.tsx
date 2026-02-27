import { Link } from "react-router"

import { BodyText, ButtonWrapper, CardWrapper, PageContainer, SectionContainer, SectionHeader } from "@/shared/components/design-system"

function NotFoundPage() {
  return (
    <div className="grid min-h-[70vh] place-items-center py-10">
      <PageContainer>
        <CardWrapper className="mx-auto w-full max-w-lg">
          <SectionContainer className="border-0 bg-transparent p-0">
            <SectionHeader title="Page not found" />
            <BodyText>The requested route is not available.</BodyText>
            <div>
              <ButtonWrapper asChild>
                <Link to="/">Go to home</Link>
              </ButtonWrapper>
            </div>
          </SectionContainer>
        </CardWrapper>
      </PageContainer>
    </div>
  )
}

export default NotFoundPage
