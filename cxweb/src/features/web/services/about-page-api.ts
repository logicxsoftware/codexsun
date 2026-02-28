import type {
  AboutSectionData,
  CallToActionSectionData,
  FeaturesSectionData,
  HeroSectionData,
  WhyChooseUsSectionData,
} from "@/features/web/services/web-page-api"
import { httpClient } from "@/shared/services/http-client"

export type TeamMemberData = {
  name: string
  role: string
  bio: string
  image: string
  order: number
}

export type AboutTestimonialData = {
  name: string
  company?: string | null
  quote: string
  rating?: number | null
  order: number
}

export type RoadmapMilestoneData = {
  year: string
  title: string
  description: string
  order: number
}

export type AboutPageResponse = {
  hero: HeroSectionData
  about: AboutSectionData
  whyChooseUs: WhyChooseUsSectionData
  features: FeaturesSectionData
  team: TeamMemberData[]
  testimonials: AboutTestimonialData[]
  roadmap: RoadmapMilestoneData[]
  callToAction: CallToActionSectionData
}

export const aboutPageApi = {
  get: (): Promise<AboutPageResponse> => httpClient.get<AboutPageResponse>("/about-page"),
}
