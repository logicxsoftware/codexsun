import { sliderDirection, sliderLayerAnimationFrom } from "@/features/slider/types/slider-types"

export const getSlideAnimationClass = (direction: number): string => {
  if (direction === sliderDirection.Left) {
    return "animate-in fade-in slide-in-from-left-8 duration-500"
  }

  if (direction === sliderDirection.Right) {
    return "animate-in fade-in slide-in-from-right-8 duration-500"
  }

  return "animate-in fade-in duration-500"
}

export const getLayerFromClass = (from: number): string => {
  if (from === sliderLayerAnimationFrom.Left) {
    return "translate-x-[-16px] opacity-0"
  }

  if (from === sliderLayerAnimationFrom.Right) {
    return "translate-x-[16px] opacity-0"
  }

  if (from === sliderLayerAnimationFrom.Top) {
    return "translate-y-[-16px] opacity-0"
  }

  if (from === sliderLayerAnimationFrom.Bottom) {
    return "translate-y-[16px] opacity-0"
  }

  if (from === sliderLayerAnimationFrom.Zoom) {
    return "scale-95 opacity-0"
  }

  return "opacity-0"
}
