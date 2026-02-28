import { useMemo, useState } from "react"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Slider } from "@/components/ui/slider"
import FullScreenSlider from "@/features/slider/components/FullScreenSlider"
import { useSlider } from "@/features/slider/context/SliderProvider"
import { sliderApi } from "@/features/slider/services/slider-api"
import type { SlideDto } from "@/features/slider/types/slider-types"
import { sliderBackgroundMode, sliderCtaColor, sliderDirection, sliderHeightMode, sliderIntensity, sliderLayerAnimationFrom, sliderLayerType, sliderMediaType, sliderVariant } from "@/features/slider/types/slider-types"
import { showToast } from "@/shared/components/ui/toast"

const getEmptySlidePayload = (order: number) => ({
  order,
  title: "New Slide",
  tagline: "Add content",
  actionText: "Learn More",
  actionHref: "/",
  ctaColor: sliderCtaColor.Primary,
  duration: 6000,
  direction: sliderDirection.Fade,
  variant: sliderVariant.Default,
  intensity: sliderIntensity.Medium,
  backgroundMode: sliderBackgroundMode.Normal,
  showOverlay: true,
  overlayToken: "background/50",
  backgroundUrl: "/assets/techmedia/placeholder.jpg",
  mediaType: sliderMediaType.Image,
  youtubeVideoId: null,
  isActive: true,
  highlights: [],
})

function SliderBuilderPage() {
  const { config, refresh, isLoading } = useSlider()
  const [selectedSlideId, setSelectedSlideId] = useState<string | null>(null)

  const slides = useMemo(() => (config ? [...config.slides].sort((a, b) => a.order - b.order) : []), [config])
  const selectedSlide = useMemo(() => slides.find((slide) => slide.id === selectedSlideId) ?? slides[0] ?? null, [selectedSlideId, slides])

  const saveConfig = async () => {
    if (!config) {
      return
    }

    await sliderApi.updateConfig({
      isActive: config.isActive,
      heightMode: config.heightMode,
      heightValue: config.heightValue,
      containerMode: config.containerMode,
      contentAlignment: config.contentAlignment,
      autoplay: config.autoplay,
      loop: config.loop,
      showProgress: config.showProgress,
      showNavArrows: config.showNavArrows,
      showDots: config.showDots,
      parallax: config.parallax,
      particles: config.particles,
      defaultVariant: config.defaultVariant,
      defaultIntensity: config.defaultIntensity,
      defaultDirection: config.defaultDirection,
      defaultBackgroundMode: config.defaultBackgroundMode,
      scrollBehavior: config.scrollBehavior,
    })

    await refresh()
    showToast({ title: "Slider config saved", variant: "success" })
  }

  const createSlide = async () => {
    await sliderApi.createSlide(getEmptySlidePayload(slides.length))
    await refresh()
    showToast({ title: "Slide created", variant: "success" })
  }

  const deleteSlide = async (slideId: string) => {
    await sliderApi.deleteSlide(slideId)
    await refresh()
    showToast({ title: "Slide deleted", variant: "warning" })
  }

  const moveSlide = async (slideId: string, direction: "up" | "down") => {
    const current = slides.find((slide) => slide.id === slideId)
    if (!current) {
      return
    }

    const targetOrder = direction === "up" ? current.order - 1 : current.order + 1
    const target = slides.find((slide) => slide.order === targetOrder)
    if (!target) {
      return
    }

    await sliderApi.reorderSlides([
      { slideId: current.id, order: target.order },
      { slideId: target.id, order: current.order },
    ])
    await refresh()
  }

  const saveSlide = async () => {
    if (!selectedSlide) {
      return
    }

    await sliderApi.updateSlide(selectedSlide.id, {
      order: selectedSlide.order,
      title: selectedSlide.title,
      tagline: selectedSlide.tagline,
      actionText: selectedSlide.actionText,
      actionHref: selectedSlide.actionHref,
      ctaColor: selectedSlide.ctaColor,
      duration: selectedSlide.duration,
      direction: selectedSlide.direction,
      variant: selectedSlide.variant,
      intensity: selectedSlide.intensity,
      backgroundMode: selectedSlide.backgroundMode,
      showOverlay: selectedSlide.showOverlay,
      overlayToken: selectedSlide.overlayToken,
      backgroundUrl: selectedSlide.backgroundUrl,
      mediaType: selectedSlide.mediaType,
      youtubeVideoId: selectedSlide.youtubeVideoId,
      isActive: selectedSlide.isActive,
      highlights: selectedSlide.highlights.map((item) => ({ text: item.text, variant: item.variant, order: item.order })),
    })

    await refresh()
    showToast({ title: "Slide updated", variant: "success" })
  }

  const createLayer = async () => {
    if (!selectedSlide) {
      return
    }

    await sliderApi.createLayer({
      slideId: selectedSlide.id,
      order: selectedSlide.layers.length,
      type: sliderLayerType.Text,
      content: "Layer text",
      mediaUrl: null,
      positionX: 10,
      positionY: 10,
      width: "200px",
      animationFrom: sliderLayerAnimationFrom.Left,
      animationDelay: 100,
      animationDuration: 600,
      animationEasing: "ease-out",
      responsiveVisibility: "all",
    })
    await refresh()
    showToast({ title: "Layer created", variant: "success" })
  }

  const deleteLayer = async (layerId: string) => {
    await sliderApi.deleteLayer(layerId)
    await refresh()
    showToast({ title: "Layer deleted", variant: "warning" })
  }

  const updateSelectedSlide = (updater: (slide: SlideDto) => SlideDto) => {
    if (!config || !selectedSlide) {
      return
    }

    const updatedSlides = config.slides.map((slide) => (slide.id === selectedSlide.id ? updater(slide) : slide))
    const updated = updatedSlides.find((slide) => slide.id === selectedSlide.id) ?? selectedSlide
    setSelectedSlideId(updated.id)
    ;(config as { slides: SlideDto[] }).slides = updatedSlides
  }

  if (isLoading || !config) {
    return <div className="text-sm text-muted-foreground">Loading slider builder...</div>
  }

  return (
    <div className="grid gap-4">
      <div className="grid gap-4 lg:grid-cols-[320px_minmax(0,1fr)]">
        <Card>
          <CardHeader>
            <CardTitle>Global Settings</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3">
            <label className="text-sm">
              Height Mode
              <select
                className="mt-1 h-9 w-full rounded-md border border-input bg-background px-3 text-sm"
                value={config.heightMode}
                onChange={(event) => {
                  config.heightMode = Number(event.target.value)
                }}
              >
                <option value={sliderHeightMode.Fullscreen}>fullscreen</option>
                <option value={sliderHeightMode.Vh}>vh</option>
                <option value={sliderHeightMode.Px}>px</option>
              </select>
            </label>
            <label className="text-sm">
              Height Value
              <Input
                type="number"
                value={config.heightValue}
                onChange={(event) => {
                  config.heightValue = Number(event.target.value)
                }}
              />
            </label>
            <label className="text-sm">
              Autoplay
              <input
                type="checkbox"
                className="ml-2"
                checked={config.autoplay}
                onChange={(event) => {
                  config.autoplay = event.target.checked
                }}
              />
            </label>
            <label className="text-sm">
              Loop
              <input
                type="checkbox"
                className="ml-2"
                checked={config.loop}
                onChange={(event) => {
                  config.loop = event.target.checked
                }}
              />
            </label>
            <Button type="button" onClick={() => void saveConfig()}>
              Save Settings
            </Button>
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Live Preview</CardTitle>
          </CardHeader>
          <CardContent className="space-y-3">
            <div className="overflow-hidden rounded-md border border-border">
              <FullScreenSlider config={config} />
            </div>
          </CardContent>
        </Card>
      </div>

      <div className="grid gap-4 lg:grid-cols-[320px_minmax(0,1fr)]">
        <Card>
          <CardHeader>
            <CardTitle>Slides</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-2">
            <Button type="button" variant="secondary" onClick={() => void createSlide()}>
              Add Slide
            </Button>
            {slides.map((slide) => (
              <div key={slide.id} className="rounded-md border border-border p-2">
                <button type="button" className="w-full text-left text-sm font-medium text-foreground" onClick={() => setSelectedSlideId(slide.id)}>
                  {slide.order + 1}. {slide.title}
                </button>
                <div className="mt-2 flex gap-1">
                  <Button type="button" size="sm" variant="outline" onClick={() => void moveSlide(slide.id, "up")}>
                    Up
                  </Button>
                  <Button type="button" size="sm" variant="outline" onClick={() => void moveSlide(slide.id, "down")}>
                    Down
                  </Button>
                  <Button type="button" size="sm" variant="outline" onClick={() => void deleteSlide(slide.id)}>
                    Delete
                  </Button>
                </div>
              </div>
            ))}
          </CardContent>
        </Card>

        <Card>
          <CardHeader>
            <CardTitle>Slide Editor</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-3">
            {!selectedSlide ? <p className="text-sm text-muted-foreground">Select a slide.</p> : null}
            {selectedSlide ? (
              <>
                <label className="text-sm">
                  Title
                  <Input value={selectedSlide.title} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, title: event.target.value }))} />
                </label>
                <label className="text-sm">
                  Tagline
                  <Input value={selectedSlide.tagline} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, tagline: event.target.value }))} />
                </label>
                <label className="text-sm">
                  CTA Text
                  <Input value={selectedSlide.actionText ?? ""} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, actionText: event.target.value }))} />
                </label>
                <label className="text-sm">
                  CTA Href
                  <Input value={selectedSlide.actionHref ?? ""} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, actionHref: event.target.value }))} />
                </label>
                <label className="text-sm">
                  Background URL
                  <Input value={selectedSlide.backgroundUrl} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, backgroundUrl: event.target.value }))} />
                </label>
                <label className="text-sm">
                  Duration (ms)
                  <Slider
                    value={[selectedSlide.duration]}
                    min={2000}
                    max={12000}
                    step={500}
                    onValueChange={(value) => updateSelectedSlide((slide) => ({ ...slide, duration: value[0] ?? 6000 }))}
                  />
                </label>
                <label className="text-sm">
                  Media Type
                  <select
                    className="mt-1 h-9 w-full rounded-md border border-input bg-background px-3 text-sm"
                    value={selectedSlide.mediaType}
                    onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, mediaType: Number(event.target.value) }))}
                  >
                    <option value={sliderMediaType.Image}>image</option>
                    <option value={sliderMediaType.Video}>video</option>
                    <option value={sliderMediaType.Youtube}>youtube</option>
                  </select>
                </label>
                {selectedSlide.mediaType === sliderMediaType.Youtube ? (
                  <label className="text-sm">
                    YouTube ID
                    <Input value={selectedSlide.youtubeVideoId ?? ""} onChange={(event) => updateSelectedSlide((slide) => ({ ...slide, youtubeVideoId: event.target.value }))} />
                  </label>
                ) : null}
                <Button type="button" onClick={() => void saveSlide()}>
                  Save Slide
                </Button>
              </>
            ) : null}
          </CardContent>
        </Card>
      </div>

      {selectedSlide ? (
        <Card>
          <CardHeader>
            <CardTitle>Layer Editor</CardTitle>
          </CardHeader>
          <CardContent className="grid gap-2">
            <Button type="button" variant="secondary" onClick={() => void createLayer()}>
              Add Layer
            </Button>
            {selectedSlide.layers
              .slice()
              .sort((a, b) => a.order - b.order)
              .map((layer) => (
                <div key={layer.id} className="flex items-center justify-between rounded-md border border-border px-3 py-2">
                  <div className="text-sm text-foreground">
                    {layer.order + 1}. {layer.content}
                  </div>
                  <div className="flex gap-2">
                    <Button type="button" size="sm" variant="outline" onClick={() => void deleteLayer(layer.id)}>
                      Delete
                    </Button>
                  </div>
                </div>
              ))}
          </CardContent>
        </Card>
      ) : null}
    </div>
  )
}

export default SliderBuilderPage
