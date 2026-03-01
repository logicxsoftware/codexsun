import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react"
import type { ReactNode } from "react"

import { sliderApi } from "@/features/slider/services/slider-api"
import { sliderBackgroundMode, sliderContainerMode, sliderContentAlignment, sliderCtaColor, sliderDirection, sliderHeightMode, sliderIntensity, sliderMediaType, sliderVariant } from "@/features/slider/types/slider-types"
import type { SliderConfigDto } from "@/features/slider/types/slider-types"

type SliderContextValue = {
  isLoading: boolean
  config: SliderConfigDto | null
  homeSlider: SliderConfigDto | null
  preloadNext: boolean
  lazyMedia: boolean
  gpuAcceleration: boolean
  optimizedRendering: boolean
  refresh: () => Promise<void>
}

const SliderContext = createContext<SliderContextValue | null>(null)

type SliderProviderProps = {
  children: ReactNode
}

const hasActiveSlides = (value: SliderConfigDto | null): value is SliderConfigDto => {
  if (!value || !value.isActive) {
    return false
  }

  return value.slides.some((slide) => slide.isActive)
}

const createCodexsunFallbackSlider = (): SliderConfigDto => ({
  id: "fallback-codexsun-slider",
  tenantId: null,
  isActive: true,
  heightMode: sliderHeightMode.Fullscreen,
  heightValue: 100,
  containerMode: sliderContainerMode.Containered,
  contentAlignment: sliderContentAlignment.Left,
  autoplay: true,
  loop: true,
  showProgress: true,
  showNavArrows: true,
  showDots: true,
  parallax: false,
  particles: false,
  defaultVariant: sliderVariant.Saas,
  defaultIntensity: sliderIntensity.Medium,
  defaultDirection: sliderDirection.Left,
  defaultBackgroundMode: sliderBackgroundMode.Normal,
  scrollBehavior: 1,
  slides: [
    {
      id: "fallback-codexsun-slide-1",
      order: 0,
      title: "Codexsun Software Platform",
      tagline: "Reliable multi-tenant software products and implementation services for growing businesses.",
      actionText: "Explore Platform",
      actionHref: "/about",
      ctaColor: sliderCtaColor.Primary,
      duration: 6500,
      direction: sliderDirection.Left,
      variant: sliderVariant.Saas,
      intensity: sliderIntensity.Medium,
      backgroundMode: sliderBackgroundMode.Normal,
      showOverlay: true,
      overlayToken: "muted/70",
      backgroundUrl: "https://images.unsplash.com/photo-1518773553398-650c184e0bb3?w=1920",
      mediaType: sliderMediaType.Image,
      youtubeVideoId: null,
      isActive: true,
      layers: [],
      highlights: [
        { id: "fallback-highlight-1", text: "Multi-Tenant Platform", variant: "primary", order: 0 },
        { id: "fallback-highlight-2", text: "Scalable Architecture", variant: "success", order: 1 },
      ],
    },
  ],
})

function SliderProvider({ children }: SliderProviderProps) {
  const [isLoading, setIsLoading] = useState(true)
  const [config, setConfig] = useState<SliderConfigDto | null>(null)

  const load = useCallback(async () => {
    setIsLoading(true)
    try {
      let response: SliderConfigDto | null = null

      try {
        response = await sliderApi.getHome()
      } catch {
        response = null
      }

      if (!hasActiveSlides(response)) {
        try {
          const homeDataSlider = await sliderApi.getHomeDataSlider()
          response = homeDataSlider
        } catch {
          // Keep primary response when fallback endpoint is unavailable.
        }
      }

      if (!hasActiveSlides(response)) {
        response = createCodexsunFallbackSlider()
      }

      setConfig(response)
    } finally {
      setIsLoading(false)
    }
  }, [])

  useEffect(() => {
    void load()
  }, [load])

  const homeSlider = useMemo(() => {
    if (!config || !config.isActive) {
      return null
    }

    return {
      ...config,
      slides: config.slides.filter((slide) => slide.isActive).sort((a, b) => a.order - b.order),
    }
  }, [config])

  const value = useMemo<SliderContextValue>(
    () => ({
      isLoading,
      config,
      homeSlider,
      preloadNext: true,
      lazyMedia: true,
      gpuAcceleration: true,
      optimizedRendering: true,
      refresh: load,
    }),
    [config, homeSlider, isLoading, load],
  )

  return <SliderContext.Provider value={value}>{children}</SliderContext.Provider>
}

const useSlider = (): SliderContextValue => {
  const context = useContext(SliderContext)
  if (!context) {
    throw new Error("useSlider must be used within SliderProvider")
  }

  return context
}

export { SliderProvider, useSlider }
