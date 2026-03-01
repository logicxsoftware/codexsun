import { httpClient } from "@/shared/services/http-client"
import type { CreateLayerPayload, CreateSlidePayload, SlideDto, SliderConfigDto, SliderLayerDto, UpdateSliderConfigPayload } from "@/features/slider/types/slider-types"

type HomeDataSliderResponse = {
  slider: SliderConfigDto
}

export const sliderApi = {
  get: (): Promise<SliderConfigDto> => httpClient.get<SliderConfigDto>("/slider"),
  getHome: (): Promise<SliderConfigDto> => httpClient.get<SliderConfigDto>("/slider", { skipGlobalLoading: true }),
  getHomeDataSlider: (): Promise<SliderConfigDto> =>
    httpClient.get<HomeDataSliderResponse>("/home-data", { skipGlobalLoading: true }).then((response) => response.slider),
  updateConfig: (payload: UpdateSliderConfigPayload): Promise<SliderConfigDto> => httpClient.put<SliderConfigDto>("/slider", payload),
  createSlide: (payload: CreateSlidePayload): Promise<SlideDto> => httpClient.post<SlideDto>("/slider/slides", payload),
  updateSlide: (id: string, payload: CreateSlidePayload): Promise<SlideDto> => httpClient.patch<SlideDto>(`/slider/slides/${encodeURIComponent(id)}`, payload),
  deleteSlide: (id: string): Promise<void> => httpClient.delete<void>(`/slider/slides/${encodeURIComponent(id)}`),
  reorderSlides: (items: Array<{ slideId: string; order: number }>): Promise<void> => httpClient.patch<void>("/slider/slides/reorder", { items }),
  createLayer: (payload: CreateLayerPayload): Promise<SliderLayerDto> => httpClient.post<SliderLayerDto>("/slider/layers", payload),
  updateLayer: (id: string, payload: Omit<CreateLayerPayload, "slideId">): Promise<SliderLayerDto> => httpClient.patch<SliderLayerDto>(`/slider/layers/${encodeURIComponent(id)}`, payload),
  deleteLayer: (id: string): Promise<void> => httpClient.delete<void>(`/slider/layers/${encodeURIComponent(id)}`),
}
