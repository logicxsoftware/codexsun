import { sliderMediaType } from "@/features/slider/types/slider-types"

export const getYoutubeEmbedUrl = (videoId: string): string => `https://www.youtube.com/embed/${encodeURIComponent(videoId)}?autoplay=1&mute=1&controls=0&loop=1&playlist=${encodeURIComponent(videoId)}&modestbranding=1&rel=0`

export const detectMediaType = (type: number): "image" | "video" | "youtube" => {
  if (type === sliderMediaType.Video) {
    return "video"
  }

  if (type === sliderMediaType.Youtube) {
    return "youtube"
  }

  return "image"
}
