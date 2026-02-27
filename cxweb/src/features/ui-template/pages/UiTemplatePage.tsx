import { useState } from "react"
import { Bell, Loader2, Menu } from "lucide-react"
import { Link } from "react-router"

import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert"
import { Input } from "@/components/ui/input"
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select"
import { Textarea } from "@/components/ui/textarea"
import { showToast } from "@/shared/components/ui/toast"
import {
  BodyText,
  ButtonWrapper,
  CardWrapper,
  ContentWrapper,
  Divider,
  FormGroup,
  GridLayout,
  PageContainer,
  SectionContainer,
  SectionHeader,
  Subtitle,
  Title,
} from "@/shared/components/design-system"

function UiTemplatePage() {
  const [showLoader, setShowLoader] = useState(false)

  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="border-b border-border/70 bg-header-bg text-header-foreground">
        <PageContainer width="wide" className="flex h-16 items-center justify-between">
          <div className="flex items-center gap-2">
            <div className="h-8 w-8 rounded-md bg-brand/20" />
            <span className="text-sm font-semibold">UI Template</span>
          </div>
          <div className="flex items-center gap-2">
            <ButtonWrapper variant="ghost" size="sm" className="hover:bg-menu-hover">
              <Menu className="h-4 w-4" />
            </ButtonWrapper>
            <ButtonWrapper size="sm" className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
              Primary Action
            </ButtonWrapper>
          </div>
        </PageContainer>
      </header>

      <main className="py-8 md:py-10">
        <PageContainer width="wide">
          <ContentWrapper>
            <SectionContainer>
              <SectionHeader
                title="Design System Baseline"
                subtitle="Canonical template for spacing, typography, layout, components, and token usage."
              />
              <Divider className="my-4" />
              <Title>Unified SaaS UI Standard</Title>
              <BodyText>
                This page demonstrates approved primitives and token-driven components across fixed, full-width, and multi-column layouts.
              </BodyText>
              <div className="flex flex-wrap gap-2">
                <ButtonWrapper>Default</ButtonWrapper>
                <ButtonWrapper variant="secondary">Secondary</ButtonWrapper>
                <ButtonWrapper variant="outline">Outline</ButtonWrapper>
                <ButtonWrapper variant="destructive">Destructive</ButtonWrapper>
                <ButtonWrapper variant="ghost">Ghost</ButtonWrapper>
                <ButtonWrapper variant="link" asChild>
                  <Link to="/">Token Link</Link>
                </ButtonWrapper>
              </div>
            </SectionContainer>

            <GridLayout columns={2} gap="lg">
              <SectionContainer>
                <SectionHeader title="Typography Scale" subtitle="Title, subtitle, and body hierarchy." />
                <div className="grid gap-2">
                  <Title className="text-2xl md:text-3xl">Title Example</Title>
                  <Subtitle>Subtitle Example</Subtitle>
                  <BodyText>Body text example using muted foreground tokens and consistent line-height scale.</BodyText>
                </div>
              </SectionContainer>

              <SectionContainer>
                <SectionHeader title="Form Pattern" subtitle="Standardized input, select, textarea, and grouping." />
                <form className="grid gap-4">
                  <FormGroup>
                    <label className="text-sm font-medium text-foreground" htmlFor="template-name">
                      Name
                    </label>
                    <Input id="template-name" placeholder="Product Name" />
                  </FormGroup>
                  <FormGroup>
                    <label className="text-sm font-medium text-foreground" htmlFor="template-plan">
                      Plan
                    </label>
                    <Select defaultValue="pro">
                      <SelectTrigger id="template-plan" className="w-full">
                        <SelectValue placeholder="Select plan" />
                      </SelectTrigger>
                      <SelectContent>
                        <SelectItem value="starter">Starter</SelectItem>
                        <SelectItem value="pro">Pro</SelectItem>
                        <SelectItem value="enterprise">Enterprise</SelectItem>
                      </SelectContent>
                    </Select>
                  </FormGroup>
                  <FormGroup>
                    <label className="text-sm font-medium text-foreground" htmlFor="template-notes">
                      Notes
                    </label>
                    <Textarea id="template-notes" rows={4} placeholder="Write details..." />
                  </FormGroup>
                  <ButtonWrapper block className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
                    Submit
                  </ButtonWrapper>
                </form>
              </SectionContainer>
            </GridLayout>

            <SectionContainer>
              <SectionHeader title="Card Grid" subtitle="Standard card wrapper and content rhythm." />
              <GridLayout columns={3} gap="md">
                <CardWrapper>
                  <div className="grid gap-2 p-4">
                    <Subtitle className="text-lg">Usage Analytics</Subtitle>
                    <BodyText>Tokenized card surfaces ensure consistent contrast and elevation behavior.</BodyText>
                    <ButtonWrapper variant="outline" size="sm">View</ButtonWrapper>
                  </div>
                </CardWrapper>
                <CardWrapper>
                  <div className="grid gap-2 p-4">
                    <Subtitle className="text-lg">Blog Card</Subtitle>
                    <BodyText>February 27, 2026 • Theme governance and consistency workflows.</BodyText>
                    <ButtonWrapper size="sm" className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
                      Read Post
                    </ButtonWrapper>
                  </div>
                </CardWrapper>
                <CardWrapper>
                  <div className="grid gap-2 p-4">
                    <Subtitle className="text-lg">Action Center</Subtitle>
                    <BodyText>Centralized actions follow shared button and spacing patterns.</BodyText>
                    <div className="flex gap-2">
                      <ButtonWrapper size="sm">Run</ButtonWrapper>
                      <ButtonWrapper size="sm" variant="ghost">Config</ButtonWrapper>
                    </div>
                  </div>
                </CardWrapper>
              </GridLayout>
            </SectionContainer>

            <GridLayout columns={2} gap="lg">
              <SectionContainer>
                <SectionHeader title="Notification + Loader" subtitle="Toast and loader triggers." />
                <div className="flex flex-wrap items-center gap-2">
                  <ButtonWrapper
                    onClick={() => {
                      showToast({
                        title: "Template Toast",
                        description: "UI template notification preview.",
                        variant: "success",
                      })
                    }}
                  >
                    Trigger Toast
                  </ButtonWrapper>
                  <ButtonWrapper
                    variant="outline"
                    onClick={() => {
                      setShowLoader((prev) => !prev)
                    }}
                  >
                    Toggle Loader
                  </ButtonWrapper>
                </div>
                {showLoader ? (
                  <div className="mt-4 inline-flex items-center gap-2 rounded-md border border-border bg-muted/40 px-3 py-2 text-sm text-muted-foreground">
                    <Loader2 className="h-4 w-4 animate-spin text-brand" />
                    Processing...
                  </div>
                ) : null}
              </SectionContainer>

              <SectionContainer>
                <SectionHeader title="Alert + Navigation" subtitle="Alert and link behavior baseline." />
                <Alert>
                  <Bell className="h-4 w-4" />
                  <AlertTitle>Governance Rule</AlertTitle>
                  <AlertDescription>Only token-driven classes and approved primitives should be used.</AlertDescription>
                </Alert>
                <div className="mt-4 grid gap-2 text-sm">
                  <Link className="text-link hover:text-link-hover" to="/">
                    Home
                  </Link>
                  <Link className="text-link hover:text-link-hover" to="/ui-template">
                    UI Template
                  </Link>
                  <Link className="text-link hover:text-link-hover" to="/theme-preview">
                    Theme Preview
                  </Link>
                </div>
              </SectionContainer>
            </GridLayout>

            <SectionContainer className="bg-cta-bg text-cta-foreground">
              <SectionHeader
                title="CTA Section"
                subtitle="Call-to-action surfaces follow dedicated CTA tokens across modes."
                className="[&_h2]:text-cta-foreground [&_p]:text-cta-foreground/90"
              />
              <div className="mt-3 flex flex-wrap gap-2">
                <ButtonWrapper variant="secondary">Secondary CTA</ButtonWrapper>
                <ButtonWrapper variant="outline" className="border-cta-foreground/40 bg-transparent text-cta-foreground hover:bg-cta-foreground/10">
                  Outline CTA
                </ButtonWrapper>
              </div>
            </SectionContainer>

            <SectionContainer className="bg-footer-bg text-footer-foreground">
              <div className="flex flex-col gap-3 md:flex-row md:items-center md:justify-between">
                <BodyText className="text-footer-foreground">Footer pattern for token-based backgrounds and links.</BodyText>
                <Link className="text-link hover:text-link-hover" to="/">
                  Back to site
                </Link>
              </div>
            </SectionContainer>
          </ContentWrapper>
        </PageContainer>
      </main>
    </div>
  )
}

export default UiTemplatePage
