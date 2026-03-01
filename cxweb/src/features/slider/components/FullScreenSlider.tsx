import { memo, useCallback, useEffect, useMemo, useRef, useState } from "react"
import type { CSSProperties } from "react"
import { ChevronLeft, ChevronRight } from "lucide-react"

import { Button } from "@/components/ui/button"
import type { SlideDto, SliderConfigDto, SliderLayerDto } from "@/features/slider/types/slider-types"
import { sliderContentAlignment, sliderContainerMode, sliderCtaColor, sliderHeightMode } from "@/features/slider/types/slider-types"
import { getLayerFromClass } from "@/features/slider/utils/slider-animation"
import { detectMediaType, getYoutubeEmbedUrl } from "@/features/slider/utils/slider-media"
import { cn } from "@/lib/utils"

const ctaClassMap: Record<number, string> = {
  [sliderCtaColor.Primary]: "bg-primary text-primary-foreground hover:bg-primary/90",
  [sliderCtaColor.Secondary]: "bg-secondary text-secondary-foreground hover:bg-secondary/90",
  [sliderCtaColor.Success]: "bg-accent text-accent-foreground hover:bg-accent/90",
  [sliderCtaColor.Warning]: "bg-muted text-foreground hover:bg-muted/90",
  [sliderCtaColor.Info]: "bg-card text-card-foreground hover:bg-card/90",
  [sliderCtaColor.Danger]: "bg-destructive text-destructive-foreground hover:bg-destructive/90",
}

const alignmentClassMap: Record<number, string> = {
  [sliderContentAlignment.Left]: "items-start text-left",
  [sliderContentAlignment.Center]: "items-center text-center",
  [sliderContentAlignment.Right]: "items-end text-right",
}

const resolveHeightStyle = (config: SliderConfigDto): CSSProperties => {
  const heightOffsetPx = 125

  if (config.heightMode === sliderHeightMode.Fullscreen) {
    return { height: `calc(100vh - ${heightOffsetPx}px)` }
  }

  if (config.heightMode === sliderHeightMode.Vh) {
    return { height: `calc(${Math.max(20, Math.min(100, config.heightValue))}vh - ${heightOffsetPx}px)` }
  }

  return { height: `${Math.max(120, config.heightValue - heightOffsetPx)}px` }
}

const getLayerVisibilityClass = (visibility: string): string => {
  if (visibility === "desktop") {
    return "hidden lg:block"
  }

  if (visibility === "mobile") {
    return "block lg:hidden"
  }

  return "block"
}

const resolveOverlayClass = (overlayToken: string): string => {
  if (overlayToken.startsWith("background")) {
    return "bg-background/18"
  }

  if (overlayToken.startsWith("muted")) {
    return "bg-muted/22"
  }

  if (overlayToken.startsWith("foreground")) {
    return "bg-foreground/8"
  }

  if (overlayToken.startsWith("card")) {
    return "bg-card/22"
  }

  return "bg-background/16"
}

type LayerProps = {
  layer: SliderLayerDto
  active: boolean
}

const SliderLayer = memo(function SliderLayer({ layer, active }: LayerProps) {
  const style = {
    left: `${layer.positionX}%`,
    top: `${layer.positionY}%`,
    width: layer.width,
    transitionDuration: `${Math.max(100, layer.animationDuration)}ms`,
    transitionDelay: `${Math.max(0, layer.animationDelay)}ms`,
    transitionTimingFunction: layer.animationEasing || "ease-out",
  } as const

  return (
    <div
      className={cn(
        "absolute z-20 transform-gpu transition-all will-change-transform",
        getLayerVisibilityClass(layer.responsiveVisibility),
        active ? "translate-x-0 translate-y-0 scale-100 opacity-100" : getLayerFromClass(layer.animationFrom),
      )}
      style={style}
    >
      {layer.type === 2 && layer.mediaUrl ? <img src={layer.mediaUrl} alt={layer.content} loading="lazy" className="h-auto w-full rounded-md object-cover" /> : null}
      {layer.type === 4 ? <span className="inline-flex rounded-full border border-border bg-card px-2 py-1 text-xs text-card-foreground">{layer.content}</span> : null}
      {layer.type !== 2 && layer.type !== 4 ? <span className="text-sm text-foreground">{layer.content}</span> : null}
    </div>
  )
})

type FullScreenSliderProps = {
  config: SliderConfigDto
}

function FullScreenSlider({ config }: FullScreenSliderProps) {
  const sliderSectionRef = useRef<HTMLElement | null>(null)
  const slides = useMemo(() => config.slides.filter((slide) => slide.isActive).sort((a, b) => a.order - b.order), [config.slides])
  const [index, setIndex] = useState(0)
  const [previousIndex, setPreviousIndex] = useState<number | null>(null)
  const [isSliding, setIsSliding] = useState(false)
  const [parallaxOffset, setParallaxOffset] = useState(0)
  const [mediaReady, setMediaReady] = useState<Record<string, boolean>>({})
  const rafRef = useRef<number | null>(null)
  const parallaxRafRef = useRef<number | null>(null)
  const slideTransitionDurationMs = 800

  const activeSlide = slides[index] ?? null
  const previousSlide = previousIndex === null ? null : (slides[previousIndex] ?? null)
  const heightStyle = useMemo(() => resolveHeightStyle(config), [config])

  useEffect(() => {
    if (slides.length === 0) {
      return
    }

    if (index >= slides.length) {
      setIndex(0)
    }
  }, [index, slides.length])

  const goTo = useCallback(
    (targetIndex: number) => {
      if (slides.length < 2 || isSliding) {
        return
      }

      const normalized = ((targetIndex % slides.length) + slides.length) % slides.length
      if (normalized === index) {
        return
      }

      setPreviousIndex(index)
      setIndex(normalized)
      setIsSliding(true)
    },
    [index, isSliding, slides.length],
  )

  const next = useCallback(() => {
    goTo(index + 1)
  }, [goTo, index])

  const prev = useCallback(() => {
    goTo(index - 1)
  }, [goTo, index])

  useEffect(() => {
    if (!config.autoplay || slides.length < 2 || !activeSlide) {
      return
    }

    const timeout = window.setTimeout(() => {
      rafRef.current = window.requestAnimationFrame(() => next())
    }, Math.max(2000, activeSlide.duration))

    return () => {
      window.clearTimeout(timeout)
      if (rafRef.current !== null) {
        window.cancelAnimationFrame(rafRef.current)
      }
    }
  }, [activeSlide, config.autoplay, next, slides.length])

  useEffect(() => {
    if (slides.length < 2) {
      return
    }

    const nextSlide = slides[(index + 1) % slides.length]
    if (!nextSlide) {
      return
    }

    if (detectMediaType(nextSlide.mediaType) === "image") {
      const image = new Image()
      image.src = nextSlide.backgroundUrl
    }
  }, [index, slides])

  useEffect(() => {
    if (!isSliding) {
      return
    }

    const timeout = window.setTimeout(() => {
      setIsSliding(false)
      setPreviousIndex(null)
    }, slideTransitionDurationMs)

    return () => {
      window.clearTimeout(timeout)
    }
  }, [isSliding, slideTransitionDurationMs])

  useEffect(() => {
    if (!config.parallax) {
      setParallaxOffset(0)
      return
    }

    const updateParallax = () => {
      const section = sliderSectionRef.current
      if (!section) {
        return
      }

      const rect = section.getBoundingClientRect()
      const rawOffset = -rect.top * 0.12
      const clampedOffset = Math.max(-40, Math.min(40, rawOffset))
      setParallaxOffset(clampedOffset)
    }

    const onScrollOrResize = () => {
      if (parallaxRafRef.current !== null) {
        window.cancelAnimationFrame(parallaxRafRef.current)
      }

      parallaxRafRef.current = window.requestAnimationFrame(updateParallax)
    }

    updateParallax()
    window.addEventListener("scroll", onScrollOrResize, { passive: true })
    window.addEventListener("resize", onScrollOrResize)

    return () => {
      window.removeEventListener("scroll", onScrollOrResize)
      window.removeEventListener("resize", onScrollOrResize)
      if (parallaxRafRef.current !== null) {
        window.cancelAnimationFrame(parallaxRafRef.current)
      }
    }
  }, [config.parallax])

  if (!config.isActive || slides.length === 0 || !activeSlide) {
    return null
  }

  const getContentFadeUpStaggerStyle = (delayMs: number, animateContent: boolean): CSSProperties | undefined => {
    if (!animateContent) {
      return undefined
    }

    return {
      animationDelay: `${delayMs}ms`,
      animationDuration: "720ms",
      animationTimingFunction: "cubic-bezier(0.22, 0.61, 0.36, 1)",
      animationFillMode: "both",
    }
  }

  const renderSlide = (slide: SlideDto, showLoadingOverlay: boolean, animateContent: boolean) => {
    const mediaType = detectMediaType(slide.mediaType)
    const mediaLoaded = mediaReady[slide.id] ?? false
    const backgroundParallaxStyle: CSSProperties | undefined = config.parallax
      ? {
          transform: `translate3d(0, ${parallaxOffset}px, 0) scale(1.08)`,
          transformOrigin: "center center",
        }
      : undefined

    return (
      <>
        <div className={cn("absolute inset-0", config.parallax ? "overflow-hidden" : "")}>
          {mediaType === "image" ? (
            <img
              src={slide.backgroundUrl}
              alt={slide.title}
              className={cn(
                "h-full w-full object-cover transition-opacity duration-500",
                config.parallax ? "transition-transform duration-150 ease-linear will-change-transform" : "",
                mediaLoaded ? "opacity-100" : "opacity-0",
              )}
              style={backgroundParallaxStyle}
              loading="eager"
              onLoad={() => setMediaReady((current) => ({ ...current, [slide.id]: true }))}
            />
          ) : null}

          {mediaType === "video" ? (
            <video
              className={cn(
                "h-full w-full object-cover transition-opacity duration-500",
                config.parallax ? "transition-transform duration-150 ease-linear will-change-transform" : "",
                mediaLoaded ? "opacity-100" : "opacity-0",
              )}
              style={backgroundParallaxStyle}
              src={slide.backgroundUrl}
              muted
              autoPlay
              loop
              playsInline
              preload="metadata"
              onLoadedData={() => setMediaReady((current) => ({ ...current, [slide.id]: true }))}
            />
          ) : null}

          {mediaType === "youtube" && slide.youtubeVideoId ? (
            <iframe
              title={slide.title}
              src={getYoutubeEmbedUrl(slide.youtubeVideoId)}
              className={cn(
                "h-full w-full transition-opacity duration-500",
                config.parallax ? "transition-transform duration-150 ease-linear will-change-transform" : "",
                mediaLoaded ? "opacity-100" : "opacity-0",
              )}
              style={backgroundParallaxStyle}
              allow="autoplay; encrypted-media; picture-in-picture"
              loading="lazy"
              onLoad={() => setMediaReady((current) => ({ ...current, [slide.id]: true }))}
            />
          ) : null}

          {slide.showOverlay ? <div className={cn("absolute inset-0", resolveOverlayClass(slide.overlayToken))} /> : null}
        </div>

        <div className={cn("relative z-10 h-full", config.containerMode === sliderContainerMode.Containered ? "mx-auto max-w-7xl px-4 sm:px-6" : "w-full px-4 sm:px-6")}>
          <div className={cn("relative flex h-full flex-col justify-center gap-4", alignmentClassMap[config.contentAlignment])}>
            {slide.layers.map((layer) => (
              <SliderLayer key={layer.id} layer={layer} active />
            ))}

            <div className="relative z-30 max-w-3xl space-y-3">
              {slide.highlights.length > 0 ? (
                <div
                  className={cn("flex flex-wrap gap-2", animateContent ? "slider-content-fade-up-stagger" : "")}
                  style={getContentFadeUpStaggerStyle(160, animateContent)}
                >
                  {slide.highlights.map((highlight) => (
                    <span key={highlight.id} className="inline-flex rounded-full border border-border/70 bg-card/70 px-2.5 py-1 text-xs text-card-foreground">
                      {highlight.text}
                    </span>
                  ))}
                </div>
              ) : null}
              <h1
                className={cn("text-3xl font-bold tracking-tight text-foreground md:text-5xl", animateContent ? "slider-content-fade-up-stagger" : "")}
                style={getContentFadeUpStaggerStyle(300, animateContent)}
              >
                {slide.title}
              </h1>
              <p
                className={cn("text-base text-foreground/90 md:text-lg", animateContent ? "slider-content-fade-up-stagger" : "")}
                style={getContentFadeUpStaggerStyle(440, animateContent)}
              >
                {slide.tagline}
              </p>
              {slide.actionText && slide.actionHref ? (
                <div
                  className={cn("pt-1", animateContent ? "slider-content-fade-up-stagger" : "")}
                  style={getContentFadeUpStaggerStyle(590, animateContent)}
                >
                  <a href={slide.actionHref}>
                    <Button className={cn("h-10 px-5", ctaClassMap[slide.ctaColor] ?? ctaClassMap[sliderCtaColor.Primary])}>{slide.actionText}</Button>
                  </a>
                </div>
              ) : null}
            </div>
          </div>
        </div>

        {showLoadingOverlay && !mediaLoaded ? <div className="absolute inset-0 z-40 animate-pulse bg-card/60" /> : null}
      </>
    )
  }

  return (
    <section ref={sliderSectionRef} className="relative w-full overflow-hidden" style={heightStyle}>
      <div className="relative h-full">
        {isSliding && previousSlide ? (
          <div
            key={`prev-${previousSlide.id}`}
            aria-hidden
            className="absolute inset-0 slider-slide-exit-left"
            style={{
              animationDuration: `${slideTransitionDurationMs}ms`,
              animationTimingFunction: "cubic-bezier(0.22, 0.61, 0.36, 1)",
            }}
          >
            {renderSlide(previousSlide, false, false)}
          </div>
        ) : null}

        <div
          key={`active-${activeSlide.id}`}
          className={cn("absolute inset-0", isSliding ? "slider-slide-enter-right" : "")}
          style={
            isSliding
              ? {
                  animationDuration: `${slideTransitionDurationMs}ms`,
                  animationTimingFunction: "cubic-bezier(0.22, 0.61, 0.36, 1)",
                }
              : undefined
          }
        >
          {renderSlide(activeSlide, true, true)}
        </div>
      </div>

      {config.showNavArrows && slides.length > 1 ? (
        <>
          <button type="button" aria-label="Previous slide" onClick={prev} className="absolute left-3 top-1/2 z-50 -translate-y-1/2 rounded-full border border-border bg-card/80 p-2 text-card-foreground hover:bg-card">
            <ChevronLeft className="h-4 w-4" />
          </button>
          <button type="button" aria-label="Next slide" onClick={next} className="absolute right-3 top-1/2 z-50 -translate-y-1/2 rounded-full border border-border bg-card/80 p-2 text-card-foreground hover:bg-card">
            <ChevronRight className="h-4 w-4" />
          </button>
        </>
      ) : null}

      {config.showDots && slides.length > 1 ? (
        <div className="absolute bottom-4 left-0 right-0 z-50 flex items-center justify-center gap-1.5">
          {slides.map((slide, dotIndex) => (
            <button
              key={slide.id}
              type="button"
              aria-label={`Go to slide ${dotIndex + 1}`}
              onClick={() => goTo(dotIndex)}
              className={cn("h-2.5 w-2.5 rounded-full border border-border transition-all", dotIndex === index ? "w-6 bg-primary" : "bg-card")}
            />
          ))}
        </div>
      ) : null}

      {config.showProgress ? (
        <div className="absolute bottom-0 left-0 right-0 z-50 h-0.5 bg-muted/70">
          <div
            key={activeSlide.id}
            className="h-full bg-primary"
            style={{ width: "0%", animation: `slider-progress ${Math.max(2000, activeSlide.duration)}ms linear forwards` }}
          />
        </div>
      ) : null}
    </section>
  )
}

export default memo(FullScreenSlider)
