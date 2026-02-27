import type { ComponentPropsWithoutRef, ReactNode } from "react"

import { Button, type buttonVariants } from "@/components/ui/button"
import { Card } from "@/components/ui/card"
import { Separator } from "@/components/ui/separator"
import { cn } from "@/lib/utils"

type WidthMode = "fixed" | "wide" | "full"
type GapMode = "sm" | "md" | "lg"
type ColumnsMode = 1 | 2 | 3

type ContainerProps = ComponentPropsWithoutRef<"div"> & {
  width?: WidthMode
}

const widthClassMap: Record<WidthMode, string> = {
  fixed: "max-w-6xl",
  wide: "max-w-7xl",
  full: "max-w-none",
}

function PageContainer({ className, width = "fixed", ...props }: ContainerProps) {
  return <div className={cn("mx-auto w-full px-4 md:px-6", widthClassMap[width], className)} {...props} />
}

function SectionContainer({ className, ...props }: ComponentPropsWithoutRef<"section">) {
  return <section className={cn("rounded-xl border border-border/70 bg-card/95 p-4 md:p-6", className)} {...props} />
}

function ContentWrapper({ className, ...props }: ComponentPropsWithoutRef<"div">) {
  return <div className={cn("grid gap-4 md:gap-6", className)} {...props} />
}

function Title({ className, ...props }: ComponentPropsWithoutRef<"h1">) {
  return <h1 className={cn("text-3xl font-semibold tracking-tight text-foreground md:text-4xl", className)} {...props} />
}

function Subtitle({ className, ...props }: ComponentPropsWithoutRef<"h2">) {
  return <h2 className={cn("text-xl font-semibold tracking-tight text-foreground md:text-2xl", className)} {...props} />
}

function BodyText({ className, ...props }: ComponentPropsWithoutRef<"p">) {
  return <p className={cn("text-sm leading-6 text-muted-foreground md:text-base", className)} {...props} />
}

function Divider({ className, ...props }: ComponentPropsWithoutRef<typeof Separator>) {
  return <Separator className={cn("bg-border/70", className)} {...props} />
}

function CardWrapper({ className, ...props }: ComponentPropsWithoutRef<typeof Card>) {
  return <Card className={cn("border-border/70 bg-card text-card-foreground", className)} {...props} />
}

type ButtonWrapperProps = ComponentPropsWithoutRef<typeof Button> & {
  block?: boolean
}

function ButtonWrapper({ className, block = false, ...props }: ButtonWrapperProps) {
  return <Button className={cn(block && "w-full", className)} {...props} />
}

function FormGroup({ className, ...props }: ComponentPropsWithoutRef<"div">) {
  return <div className={cn("grid gap-2", className)} {...props} />
}

type SectionHeaderProps = ComponentPropsWithoutRef<"div"> & {
  title: ReactNode
  subtitle?: ReactNode
}

function SectionHeader({ className, title, subtitle, ...props }: SectionHeaderProps) {
  return (
    <div className={cn("grid gap-1", className)} {...props}>
      <Subtitle>{title}</Subtitle>
      {subtitle ? <BodyText>{subtitle}</BodyText> : null}
    </div>
  )
}

type GridLayoutProps = ComponentPropsWithoutRef<"div"> & {
  columns?: ColumnsMode
  gap?: GapMode
}

const columnsClassMap: Record<ColumnsMode, string> = {
  1: "grid-cols-1",
  2: "grid-cols-1 md:grid-cols-2",
  3: "grid-cols-1 md:grid-cols-2 xl:grid-cols-3",
}

const gapClassMap: Record<GapMode, string> = {
  sm: "gap-3",
  md: "gap-4",
  lg: "gap-6",
}

function GridLayout({ className, columns = 1, gap = "md", ...props }: GridLayoutProps) {
  return <div className={cn("grid", columnsClassMap[columns], gapClassMap[gap], className)} {...props} />
}

export {
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
}

export type { ButtonWrapperProps }
export type ButtonVariant = NonNullable<ComponentPropsWithoutRef<typeof Button>["variant"]>
export type ButtonSize = NonNullable<ComponentPropsWithoutRef<typeof Button>["size"]>
export type ButtonVariantConfig = Parameters<typeof buttonVariants>[0]
