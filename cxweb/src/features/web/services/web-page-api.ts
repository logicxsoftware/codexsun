import { httpClient } from "@/shared/services/http-client"

export const SectionType = {
  Menu: 1,
  Slider: 2,
  Hero: 3,
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
export type FeaturesSectionData = { items: Array<{ title: string; description: string }> }
export type GallerySectionData = { images: Array<{ url: string; alt?: string }> }
export type ProductRangeSectionData = { products: Array<{ name: string; description?: string }> }
export type WhyChooseUsSectionData = { items: Array<{ title: string; description: string }> }
export type StatsSectionData = { items: Array<{ label: string; value: string }> }
export type BrandSliderSectionData = { brands: Array<{ name: string; logoUrl?: string }> }
export type BlogShowSectionData = { limit: number; title?: string }
export type TestimonialSectionData = { items: Array<{ author: string; quote: string }> }
export type CallToActionSectionData = { title: string; label?: string; href?: string }
export type NewsletterSectionData = { title: string; subtitle?: string; placeholder?: string; buttonLabel?: string }
export type FooterSectionData = { columns: Array<{ title: string; links: Array<{ label: string; href: string }> }> }

export type SectionDataMap = {
  [SectionType.Menu]: MenuSectionData
  [SectionType.Slider]: SliderSectionData
  [SectionType.Hero]: HeroSectionData
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
