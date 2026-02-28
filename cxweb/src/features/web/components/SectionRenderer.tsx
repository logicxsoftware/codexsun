import type { ReactElement } from "react"
import { Link } from "react-router"

import FadeUp from "@/components/animations/FadeUp"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import AboutSection from "@/features/web/components/AboutSection"
import BrandSliderSection from "@/features/web/components/BrandSliderSection"
import CatalogSection from "@/features/web/components/CatalogSection"
import StatsSection from "@/features/web/components/StatsSection"
import WhyChooseUsSection from "@/features/web/components/WhyChooseUsSection"
import { SectionType as SectionTypeValues } from "@/features/web/services/web-page-api"
import type {
  AboutSectionData,
  BlogShowSectionData,
  BrandSliderSectionData,
  CatalogSectionData,
  CallToActionSectionData,
  FeaturesSectionData,
  FooterSectionData,
  GallerySectionData,
  HeroSectionData,
  MenuSectionData,
  NewsletterSectionData,
  ProductRangeSectionData,
  SectionDataMap,
  SectionType,
  SliderSectionData,
  StatsSectionData,
  TestimonialSectionData,
  WebPageSectionResponse,
  WhyChooseUsSectionData,
} from "@/features/web/services/web-page-api"

type SectionProps<T> = {
  data: T
}

const MenuSection = ({ data }: SectionProps<MenuSectionData>) => (
  <nav className="rounded-lg border border-border/80 bg-card/90 p-4">
    <div className="flex flex-wrap gap-2">
      {data.items.map((item) => (
        <Button key={`${item.label}-${item.href}`} asChild size="sm" variant="ghost" className="hover:bg-menu-hover">
          <Link to={item.href}>{item.label}</Link>
        </Button>
      ))}
    </div>
  </nav>
)

const SliderSection = ({ data }: SectionProps<SliderSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.slides.map((slide, index) => (
      <Card key={`${slide.title}-${index}`} className="border-border/80 bg-card/95">
        <CardHeader>
          <CardTitle>{slide.title}</CardTitle>
        </CardHeader>
        {slide.subtitle ? <CardContent className="text-muted-foreground">{slide.subtitle}</CardContent> : null}
      </Card>
    ))}
  </div>
)

const HeroSection = ({ data }: SectionProps<HeroSectionData>) => {
  const safeTitle = data.title?.trim() || "Welcome"
  const safeSubtitle = data.subtitle?.trim() || "Reliable technology solutions tailored for your business."

  return (
    <section className="w-full">
      <div className="mx-auto max-w-5xl px-5 text-center">
        <FadeUp>
          <h1 className="mb-6 wrap-break-word text-3xl font-bold leading-tight text-foreground md:text-4xl lg:text-5xl">{safeTitle}</h1>
        </FadeUp>
        <FadeUp delay={0.1}>
          <p className="mb-8 wrap-break-word text-lg leading-relaxed text-muted-foreground md:text-xl">{safeSubtitle}</p>
        </FadeUp>
        <FadeUp delay={0.15}>
          <div className="mx-auto mt-6 h-1 w-20 rounded-full bg-primary" />
        </FadeUp>
      </div>
    </section>
  )
}

const AboutSectionRenderer = ({ data }: SectionProps<AboutSectionData>) => <AboutSection data={data} />
const CatalogSectionRenderer = ({ data }: SectionProps<CatalogSectionData>) => <CatalogSection data={data} />

const FeaturesSection = ({ data }: SectionProps<FeaturesSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.items.map((item) => (
      <Card key={item.title} className="border-border/80 bg-card/95">
        <CardHeader>
          <CardTitle>{item.title}</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">{item.description}</CardContent>
      </Card>
    ))}
  </div>
)

const GallerySection = ({ data }: SectionProps<GallerySectionData>) => (
  <div className="grid grid-cols-2 gap-4 md:grid-cols-3">
    {data.images.map((image, index) => (
      <div key={`${image.url}-${index}`} className="overflow-hidden rounded-lg border border-border/80 bg-card/90">
        <img alt={image.alt ?? "Gallery"} className="h-40 w-full object-cover" src={image.url} />
      </div>
    ))}
  </div>
)

const ProductRangeSection = ({ data }: SectionProps<ProductRangeSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.products.map((product) => (
      <Card key={product.name} className="border-border/80 bg-card/95">
        <CardHeader>
          <CardTitle>{product.name}</CardTitle>
        </CardHeader>
        {product.description ? <CardContent className="text-muted-foreground">{product.description}</CardContent> : null}
      </Card>
    ))}
  </div>
)

const WhyChooseUsSectionRenderer = ({ data }: SectionProps<WhyChooseUsSectionData>) => <WhyChooseUsSection data={data} />

const StatsSectionRenderer = ({ data }: SectionProps<StatsSectionData>) => <StatsSection data={data} />

const BrandSliderSectionRenderer = ({ data }: SectionProps<BrandSliderSectionData>) => <BrandSliderSection data={data} />

const BlogShowSection = ({ data }: SectionProps<BlogShowSectionData>) => (
  <Card className="border-border/80 bg-card/95">
    <CardHeader>
      <CardTitle>{data.title ?? "Blog Highlights"}</CardTitle>
    </CardHeader>
    <CardContent className="text-muted-foreground">Latest {data.limit} posts available on the blog page.</CardContent>
  </Card>
)

const TestimonialSection = ({ data }: SectionProps<TestimonialSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.items.map((item) => (
      <Card key={`${item.author}-${item.quote}`} className="border-border/80 bg-card/95">
        <CardContent className="grid gap-2 py-6">
          <p className="text-muted-foreground">{item.quote}</p>
          <p className="text-sm font-medium text-foreground">{item.author}</p>
        </CardContent>
      </Card>
    ))}
  </div>
)

const CallToActionSection = ({ data }: SectionProps<CallToActionSectionData>) => (
  <Card className="border-border/80 bg-card/95">
    <CardHeader>
      <CardTitle>{data.title}</CardTitle>
    </CardHeader>
    {data.label && data.href ? (
      <CardContent>
        <Button asChild className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">
          <Link to={data.href}>{data.label}</Link>
        </Button>
      </CardContent>
    ) : null}
  </Card>
)

const NewsletterSection = ({ data }: SectionProps<NewsletterSectionData>) => (
  <Card className="border-border/80 bg-card/95">
    <CardHeader>
      <CardTitle>{data.title}</CardTitle>
    </CardHeader>
    <CardContent className="grid gap-3 text-muted-foreground">
      {data.subtitle ? <p>{data.subtitle}</p> : null}
      <div className="flex flex-col gap-2 md:flex-row">
        <Input
          className="bg-background/80"
          placeholder={data.placeholder ?? "Email address"}
          type="email"
        />
        <Button className="bg-cta-bg text-cta-foreground hover:bg-cta-bg/90">{data.buttonLabel ?? "Subscribe"}</Button>
      </div>
    </CardContent>
  </Card>
)

const FooterSection = ({ data }: SectionProps<FooterSectionData>) => (
  <div className="grid gap-4 border-t border-border/70 pt-6 md:grid-cols-4">
    {data.columns.map((column) => (
      <div key={column.title} className="grid gap-2">
        <h3 className="text-sm font-semibold text-foreground">{column.title}</h3>
        <div className="grid gap-1 text-sm text-footer-foreground">
          {column.links.map((link) => (
            <Link key={`${column.title}-${link.href}-${link.label}`} to={link.href} className="text-link hover:text-link-hover">
              {link.label}
            </Link>
          ))}
        </div>
      </div>
    ))}
  </div>
)

type SectionRendererMap = {
  [K in SectionType]: (props: SectionProps<SectionDataMap[K]>) => ReactElement
}

const sectionRenderers: SectionRendererMap = {
  [SectionTypeValues.Menu]: MenuSection,
  [SectionTypeValues.Slider]: SliderSection,
  [SectionTypeValues.Hero]: HeroSection,
  [SectionTypeValues.About]: AboutSectionRenderer,
  [SectionTypeValues.Catalog]: CatalogSectionRenderer,
  [SectionTypeValues.Features]: FeaturesSection,
  [SectionTypeValues.Gallery]: GallerySection,
  [SectionTypeValues.ProductRange]: ProductRangeSection,
  [SectionTypeValues.WhyChooseUs]: WhyChooseUsSectionRenderer,
  [SectionTypeValues.Stats]: StatsSectionRenderer,
  [SectionTypeValues.BrandSlider]: BrandSliderSectionRenderer,
  [SectionTypeValues.BlogShow]: BlogShowSection,
  [SectionTypeValues.Testimonial]: TestimonialSection,
  [SectionTypeValues.CallToAction]: CallToActionSection,
  [SectionTypeValues.Newsletter]: NewsletterSection,
  [SectionTypeValues.Footer]: FooterSection,
}

type SectionRendererProps = {
  section: WebPageSectionResponse
}

const UnknownSection = ({ sectionType }: { sectionType: number }) => (
  <Card className="border-border/80 bg-card/95">
    <CardHeader>
      <CardTitle>Unsupported Section</CardTitle>
    </CardHeader>
    <CardContent className="text-muted-foreground">Section type {sectionType} is not supported by this client build.</CardContent>
  </Card>
)

export default function SectionRenderer({ section }: SectionRendererProps) {
  const sectionType = section.sectionType
  const renderer = sectionRenderers[sectionType]
  if (!renderer) {
    return <UnknownSection sectionType={sectionType} />
  }

  return renderer({ data: section.sectionData as never })
}
