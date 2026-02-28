import { memo } from "react"

import FullScreenSlider from "@/features/slider/components/FullScreenSlider"
import SliderSkeleton from "@/features/slider/components/SliderSkeleton"
import { useSlider } from "@/features/slider/context/SliderProvider"

function GlobalHomeSlider() {
  const { isLoading, homeSlider } = useSlider()

  if (isLoading) {
    return <SliderSkeleton />
  }

  if (!homeSlider || !homeSlider.isActive) {
    return null
  }

  return <FullScreenSlider config={homeSlider} />
}

export default memo(GlobalHomeSlider)
