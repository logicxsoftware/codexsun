import { httpClient } from "@/shared/services/http-client"

export const SectionType = {
  Menu: 1,
  Slider: 2,
  Hero: 3,
  About: 15,
  Catalog: 16,
  Location: 17,
  Features: 4,
  Gallery: 5,
  ProductRange: 6,
  WhyChooseUs: 7,
  Stats: 8,
  BrandSlider: 9,
  BlogShow: 10,
  Testimonial: 11,
  CallToAction: 12,
  Newsletter: 13,
  Footer: 14,
} as const

export type SectionType = (typeof SectionType)[keyof typeof SectionType]

export type MenuSectionData = { items: Array<{ label: string; href: string }> }
export type SliderSectionData = { slides: Array<{ title: string; subtitle?: string; imageUrl?: string }> }
export type HeroSectionData = { title: string; subtitle?: string; primaryCtaLabel?: string; primaryCtaHref?: string }
export type AboutSectionData = {
  title: string
  subtitle?: string
  content: string[]
  image: { src: string; alt: string }
}
export type FeaturesSectionData = {
  title?: string
  description?: string
  imageSrc?: string
  imageAlt?: string
  bullets?: Array<{ text: string; order?: number }>
  items?: Array<{ title: string; description: string }>
}
export type GallerySectionData = { images: Array<{ url: string; alt?: string }> }
export type ProductRangeSectionData = { products: Array<{ name: string; description?: string }> }
export type WhyChooseUsSectionData = {
  heading?: string
  subheading?: string
  items: Array<{ title: string; description: string; icon?: string; order?: number }>
}
export type StatsItem = { label: string; value: string | number; suffix?: string; order?: number }
export type CatalogBadgeVariant = "emerald" | "amber" | "blue" | "purple" | "rose" | "cyan" | "indigo" | "teal" | "black"
export type StatsSectionData = {
  backgroundToken?: string
  borderToken?: string
  stats?: StatsItem[]
  items?: StatsItem[]
}
export type CatalogSectionData = {
  heading: string
  subheading?: string
  categories: Array<{
    title: string
    slug: string
    description?: string
    image: string
    variants?: string[]
    bulkBadge?: string
    featuredBadge?: string
    badgeVariant?: CatalogBadgeVariant
    featuredBadgeVariant?: CatalogBadgeVariant
    order?: number
  }>
}
export type BrandSliderSectionData = {
  heading?: string
  pauseOnHover?: boolean
  animationDuration?: number
  logos?: Array<{ name: string; logo: string; order?: number }>
  brands?: Array<{ name: string; logo?: string; logoUrl?: string; order?: number }>
}
export type BlogShowSectionData = { limit: number; title?: string }
export type TestimonialSectionData = { items: Array<{ author: string; quote: string }> }
export type CallToActionSectionData = {
  title?: string
  description?: string
  buttonText?: string
  buttonHref?: string
  label?: string
  href?: string
}
export type LocationSectionData = {
  displayName?: string
  title?: string
  address?: string
  buttonText?: string
  buttonHref?: string
  imageSrc?: string
  imageAlt?: string
  imageClassName?: string
  mapEmbedUrl?: string
  mapTitle?: string
  placeId?: string
  latitude?: number
  longitude?: number
  timings?: Array<{ day: string; hours: string; order?: number }>
  contact?: { phone?: string; email?: string }
}
export type NewsletterSectionData = {
  title?: string
  description?: string
  placeholderName?: string
  placeholderEmail?: string
  buttonText?: string
  trustNote?: string
  imageSrc?: string
  imageAlt?: string
  image?: string
  subtitle?: string
  placeholder?: string
  buttonLabel?: string
}
export type FooterSectionData = { columns: Array<{ title: string; links: Array<{ label: string; href: string }> }> }

export type SectionDataMap = {
  [SectionType.Menu]: MenuSectionData
  [SectionType.Slider]: SliderSectionData
  [SectionType.Hero]: HeroSectionData
  [SectionType.About]: AboutSectionData
  [SectionType.Catalog]: CatalogSectionData
  [SectionType.Location]: LocationSectionData
  [SectionType.Features]: FeaturesSectionData
  [SectionType.Gallery]: GallerySectionData
  [SectionType.ProductRange]: ProductRangeSectionData
  [SectionType.WhyChooseUs]: WhyChooseUsSectionData
  [SectionType.Stats]: StatsSectionData
  [SectionType.BrandSlider]: BrandSliderSectionData
  [SectionType.BlogShow]: BlogShowSectionData
  [SectionType.Testimonial]: TestimonialSectionData
  [SectionType.CallToAction]: CallToActionSectionData
  [SectionType.Newsletter]: NewsletterSectionData
  [SectionType.Footer]: FooterSectionData
}

export type WebPageSectionResponse = {
  id: string
  sectionType: SectionType
  displayOrder: number
  sectionData: unknown
  updatedAtUtc: string
}

export type WebPageResponse = {
  slug: string
  title: string
  seoTitle: string
  seoDescription: string
  updatedAtUtc: string
  sections: WebPageSectionResponse[]
}

export const webPageApi = {
  getPublishedPage: (slug: string): Promise<WebPageResponse> => httpClient.get<WebPageResponse>(`/web/${encodeURIComponent(slug)}`),
}
