import type { HeroSectionData, LocationSectionData } from "@/features/web/services/web-page-api"
import { httpClient } from "@/shared/services/http-client"

export type ContactPageResponse = {
  hero: HeroSectionData
  location: LocationSectionData
}

export type SubmitContactRequest = {
  name: string
  email: string
  subject?: string
  message: string
}

export type SubmitContactResponse = {
  id: string
  createdAtUtc: string
}

export const contactPageApi = {
  get: (): Promise<ContactPageResponse> => httpClient.get<ContactPageResponse>("/contact-page"),
  submit: (payload: SubmitContactRequest): Promise<SubmitContactResponse> => httpClient.post<SubmitContactResponse>("/contact", payload),
}
