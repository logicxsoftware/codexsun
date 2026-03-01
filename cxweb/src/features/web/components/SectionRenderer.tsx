import type { ReactElement } from "react"
import { Link } from "react-router"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import AboutSection from "@/features/web/components/AboutSection"
import BlogShowcaseSection from "@/features/web/components/BlogShowcaseSection"
import BrandSliderSection from "@/features/web/components/BrandSliderSection"
import CallToActionSection from "@/features/web/components/CallToActionSection"
import CatalogSection from "@/features/web/components/CatalogSection"
import FeaturesSection from "@/features/web/components/FeaturesSection"
import HeroSection from "@/features/web/components/HeroSection"
import LocationSection from "@/features/web/components/LocationSection"
import NewsletterSection from "@/features/web/components/NewsletterSection"
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
  LocationSectionData,
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

const HeroSectionRenderer = ({ data }: SectionProps<HeroSectionData>) => <HeroSection data={data} />

const AboutSectionRenderer = ({ data }: SectionProps<AboutSectionData>) => <AboutSection data={data} />
const CatalogSectionRenderer = ({ data }: SectionProps<CatalogSectionData>) => <CatalogSection data={data} />
const LocationSectionRenderer = ({ data }: SectionProps<LocationSectionData>) => <LocationSection data={data} />

const FeaturesSectionRenderer = ({ data }: SectionProps<FeaturesSectionData>) => <FeaturesSection data={data} />

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

const BlogShowSection = ({ data }: SectionProps<BlogShowSectionData>) => <BlogShowcaseSection data={data} />

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

const CallToActionSectionRenderer = ({ data }: SectionProps<CallToActionSectionData>) => <CallToActionSection data={data} />

const NewsletterSectionRenderer = ({ data }: SectionProps<NewsletterSectionData>) => <NewsletterSection data={data} />

const FooterSection = ({ data }: SectionProps<FooterSectionData>) => (
  <div className="grid gap-4 md:grid-cols-4">
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
  [SectionTypeValues.Hero]: HeroSectionRenderer,
  [SectionTypeValues.About]: AboutSectionRenderer,
  [SectionTypeValues.Catalog]: CatalogSectionRenderer,
  [SectionTypeValues.Location]: LocationSectionRenderer,
  [SectionTypeValues.Features]: FeaturesSectionRenderer,
  [SectionTypeValues.Gallery]: GallerySection,
  [SectionTypeValues.ProductRange]: ProductRangeSection,
  [SectionTypeValues.WhyChooseUs]: WhyChooseUsSectionRenderer,
  [SectionTypeValues.Stats]: StatsSectionRenderer,
  [SectionTypeValues.BrandSlider]: BrandSliderSectionRenderer,
  [SectionTypeValues.BlogShow]: BlogShowSection,
  [SectionTypeValues.Testimonial]: TestimonialSection,
  [SectionTypeValues.CallToAction]: CallToActionSectionRenderer,
  [SectionTypeValues.Newsletter]: NewsletterSectionRenderer,
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
