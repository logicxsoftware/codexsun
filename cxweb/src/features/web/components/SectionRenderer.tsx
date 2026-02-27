import type { ReactElement } from "react"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import type {
  BlogShowSectionData,
  BrandSliderSectionData,
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
import { SectionType as SectionTypeValues } from "@/features/web/services/web-page-api"
import { Button } from "@/components/ui/button"
import { Link } from "react-router"

type SectionProps<T> = {
  data: T
}

const MenuSection = ({ data }: SectionProps<MenuSectionData>) => (
  <nav className="rounded-lg border bg-card p-4">
    <div className="flex flex-wrap gap-2">
      {data.items.map((item) => (
        <Button key={`${item.label}-${item.href}`} asChild size="sm" variant="ghost">
          <Link to={item.href}>{item.label}</Link>
        </Button>
      ))}
    </div>
  </nav>
)

const SliderSection = ({ data }: SectionProps<SliderSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.slides.map((slide, index) => (
      <Card key={`${slide.title}-${index}`}>
        <CardHeader>
          <CardTitle>{slide.title}</CardTitle>
        </CardHeader>
        {slide.subtitle ? <CardContent className="text-muted-foreground">{slide.subtitle}</CardContent> : null}
      </Card>
    ))}
  </div>
)

const HeroSection = ({ data }: SectionProps<HeroSectionData>) => (
  <Card>
    <CardHeader>
      <CardTitle className="text-3xl">{data.title}</CardTitle>
    </CardHeader>
    <CardContent className="grid gap-4">
      {data.subtitle ? <p className="text-muted-foreground">{data.subtitle}</p> : null}
      {data.primaryCtaLabel && data.primaryCtaHref ? (
        <div>
          <Button asChild>
            <Link to={data.primaryCtaHref}>{data.primaryCtaLabel}</Link>
          </Button>
        </div>
      ) : null}
    </CardContent>
  </Card>
)

const FeaturesSection = ({ data }: SectionProps<FeaturesSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.items.map((item) => (
      <Card key={item.title}>
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
      <div key={`${image.url}-${index}`} className="overflow-hidden rounded-lg border">
        <img alt={image.alt ?? "Gallery"} className="h-40 w-full object-cover" src={image.url} />
      </div>
    ))}
  </div>
)

const ProductRangeSection = ({ data }: SectionProps<ProductRangeSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.products.map((product) => (
      <Card key={product.name}>
        <CardHeader>
          <CardTitle>{product.name}</CardTitle>
        </CardHeader>
        {product.description ? <CardContent className="text-muted-foreground">{product.description}</CardContent> : null}
      </Card>
    ))}
  </div>
)

const WhyChooseUsSection = ({ data }: SectionProps<WhyChooseUsSectionData>) => (
  <div className="grid gap-4 md:grid-cols-3">
    {data.items.map((item) => (
      <Card key={item.title}>
        <CardHeader>
          <CardTitle>{item.title}</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">{item.description}</CardContent>
      </Card>
    ))}
  </div>
)

const StatsSection = ({ data }: SectionProps<StatsSectionData>) => (
  <div className="grid gap-4 md:grid-cols-4">
    {data.items.map((item) => (
      <Card key={item.label}>
        <CardHeader>
          <CardTitle>{item.value}</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">{item.label}</CardContent>
      </Card>
    ))}
  </div>
)

const BrandSliderSection = ({ data }: SectionProps<BrandSliderSectionData>) => (
  <div className="grid grid-cols-2 gap-4 md:grid-cols-5">
    {data.brands.map((brand) => (
      <Card key={brand.name}>
        <CardContent className="py-6 text-center text-sm font-medium">{brand.name}</CardContent>
      </Card>
    ))}
  </div>
)

const BlogShowSection = ({ data }: SectionProps<BlogShowSectionData>) => (
  <Card>
    <CardHeader>
      <CardTitle>{data.title ?? "Blog Highlights"}</CardTitle>
    </CardHeader>
    <CardContent className="text-muted-foreground">Latest {data.limit} posts available on the blog page.</CardContent>
  </Card>
)

const TestimonialSection = ({ data }: SectionProps<TestimonialSectionData>) => (
  <div className="grid gap-4 md:grid-cols-2">
    {data.items.map((item) => (
      <Card key={`${item.author}-${item.quote}`}>
        <CardContent className="grid gap-2 py-6">
          <p className="text-muted-foreground">{item.quote}</p>
          <p className="text-sm font-medium">{item.author}</p>
        </CardContent>
      </Card>
    ))}
  </div>
)

const CallToActionSection = ({ data }: SectionProps<CallToActionSectionData>) => (
  <Card>
    <CardHeader>
      <CardTitle>{data.title}</CardTitle>
    </CardHeader>
    {data.label && data.href ? (
      <CardContent>
        <Button asChild>
          <Link to={data.href}>{data.label}</Link>
        </Button>
      </CardContent>
    ) : null}
  </Card>
)

const NewsletterSection = ({ data }: SectionProps<NewsletterSectionData>) => (
  <Card>
    <CardHeader>
      <CardTitle>{data.title}</CardTitle>
    </CardHeader>
    <CardContent className="grid gap-3 text-muted-foreground">
      {data.subtitle ? <p>{data.subtitle}</p> : null}
      <div className="flex flex-col gap-2 md:flex-row">
        <input className="h-10 flex-1 rounded-md border bg-background px-3 text-sm" placeholder={data.placeholder ?? "Email address"} />
        <Button>{data.buttonLabel ?? "Subscribe"}</Button>
      </div>
    </CardContent>
  </Card>
)

const FooterSection = ({ data }: SectionProps<FooterSectionData>) => (
  <div className="grid gap-4 border-t pt-6 md:grid-cols-4">
    {data.columns.map((column) => (
      <div key={column.title} className="grid gap-2">
        <h3 className="text-sm font-semibold">{column.title}</h3>
        <div className="grid gap-1 text-sm text-muted-foreground">
          {column.links.map((link) => (
            <Link key={`${column.title}-${link.href}-${link.label}`} to={link.href}>
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
  [SectionTypeValues.Features]: FeaturesSection,
  [SectionTypeValues.Gallery]: GallerySection,
  [SectionTypeValues.ProductRange]: ProductRangeSection,
  [SectionTypeValues.WhyChooseUs]: WhyChooseUsSection,
  [SectionTypeValues.Stats]: StatsSection,
  [SectionTypeValues.BrandSlider]: BrandSliderSection,
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
  <Card>
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
