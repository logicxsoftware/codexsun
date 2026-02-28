import { createContext, useCallback, useContext, useEffect, useMemo, useState } from "react"
import type { ReactNode } from "react"

import { sliderApi } from "@/features/slider/services/slider-api"
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

function SliderProvider({ children }: SliderProviderProps) {
  const [isLoading, setIsLoading] = useState(true)
  const [config, setConfig] = useState<SliderConfigDto | null>(null)

  const load = useCallback(async () => {
    setIsLoading(true)
    try {
      const response = await sliderApi.getHome()
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
