export const sliderHeightMode = {
  Fullscreen: 1,
  Vh: 2,
  Px: 3,
} as const

export const sliderContainerMode = {
  Full: 1,
  Containered: 2,
} as const

export const sliderContentAlignment = {
  Left: 1,
  Center: 2,
  Right: 3,
} as const

export const sliderVariant = {
  Saas: 1,
  Classic: 2,
  Luxury: 3,
  Industrial: 4,
  Cinematic: 5,
  Default: 6,
} as const

export const sliderIntensity = {
  Low: 1,
  Medium: 2,
  High: 3,
} as const

export const sliderDirection = {
  Left: 1,
  Right: 2,
  Fade: 3,
} as const

export const sliderBackgroundMode = {
  Normal: 1,
  Parallax: 2,
  ThreeD: 3,
  Cinematic: 4,
} as const

export const sliderMediaType = {
  Image: 1,
  Video: 2,
  Youtube: 3,
} as const

export const sliderLayerType = {
  Text: 1,
  Image: 2,
  Button: 3,
  Badge: 4,
  Custom: 5,
} as const

export const sliderLayerAnimationFrom = {
  Left: 1,
  Right: 2,
  Top: 3,
  Bottom: 4,
  Fade: 5,
  Zoom: 6,
} as const

export const sliderCtaColor = {
  Primary: 1,
  Secondary: 2,
  Success: 3,
  Warning: 4,
  Info: 5,
  Danger: 6,
} as const

export type SliderLayerDto = {
  id: string
  order: number
  type: number
  content: string
  mediaUrl: string | null
  positionX: number
  positionY: number
  width: string
  animationFrom: number
  animationDelay: number
  animationDuration: number
  animationEasing: string
  responsiveVisibility: string
}

export type SliderHighlightDto = {
  id: string
  text: string
  variant: string
  order: number
}

export type SlideDto = {
  id: string
  order: number
  title: string
  tagline: string
  actionText: string | null
  actionHref: string | null
  ctaColor: number
  duration: number
  direction: number
  variant: number
  intensity: number
  backgroundMode: number
  showOverlay: boolean
  overlayToken: string
  backgroundUrl: string
  mediaType: number
  youtubeVideoId: string | null
  isActive: boolean
  layers: SliderLayerDto[]
  highlights: SliderHighlightDto[]
}

export type SliderConfigDto = {
  id: string
  tenantId: string | null
  isActive: boolean
  heightMode: number
  heightValue: number
  containerMode: number
  contentAlignment: number
  autoplay: boolean
  loop: boolean
  showProgress: boolean
  showNavArrows: boolean
  showDots: boolean
  parallax: boolean
  particles: boolean
  defaultVariant: number
  defaultIntensity: number
  defaultDirection: number
  defaultBackgroundMode: number
  scrollBehavior: number
  slides: SlideDto[]
}

export type HighlightInput = {
  text: string
  variant: string
  order: number
}

export type CreateSlidePayload = {
  order: number
  title: string
  tagline: string
  actionText: string | null
  actionHref: string | null
  ctaColor: number
  duration: number
  direction: number
  variant: number
  intensity: number
  backgroundMode: number
  showOverlay: boolean
  overlayToken: string
  backgroundUrl: string
  mediaType: number
  youtubeVideoId: string | null
  isActive: boolean
  highlights: HighlightInput[]
}

export type CreateLayerPayload = {
  slideId: string
  order: number
  type: number
  content: string
  mediaUrl: string | null
  positionX: number
  positionY: number
  width: string
  animationFrom: number
  animationDelay: number
  animationDuration: number
  animationEasing: string
  responsiveVisibility: string
}

export type UpdateSliderConfigPayload = {
  isActive: boolean
  heightMode: number
  heightValue: number
  containerMode: number
  contentAlignment: number
  autoplay: boolean
  loop: boolean
  showProgress: boolean
  showNavArrows: boolean
  showDots: boolean
  parallax: boolean
  particles: boolean
  defaultVariant: number
  defaultIntensity: number
  defaultDirection: number
  defaultBackgroundMode: number
  scrollBehavior: number
}
