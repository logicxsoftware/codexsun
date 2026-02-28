import { useEffect, useRef, useState } from "react"

type CounterProps = {
  value: number
  suffix?: string
  duration?: number
  className?: string
}

export default function Counter({ value, suffix = "", duration = 2000, className = "" }: CounterProps) {
  const [count, setCount] = useState(0)
  const nodeRef = useRef<HTMLDivElement | null>(null)
  const hasAnimatedRef = useRef(false)

  useEffect(() => {
    const element = nodeRef.current
    if (!element) {
      return
    }

    let observer: IntersectionObserver | null = null
    let animationFrameId = 0

    const runAnimation = (): void => {
      if (hasAnimatedRef.current) {
        return
      }

      hasAnimatedRef.current = true
      const startTime = performance.now()
      const endValue = Math.max(0, Math.floor(value))

      const tick = (time: number): void => {
        const elapsed = time - startTime
        const progress = Math.min(1, elapsed / duration)
        const next = Math.floor(endValue * progress)
        setCount(next)

        if (progress < 1) {
          animationFrameId = window.requestAnimationFrame(tick)
        } else {
          setCount(endValue)
        }
      }

      animationFrameId = window.requestAnimationFrame(tick)
    }

    observer = new IntersectionObserver(
      ([entry]) => {
        if (entry.isIntersecting) {
          runAnimation()
          observer?.disconnect()
          observer = null
        }
      },
      { threshold: 0.5 },
    )

    observer.observe(element)

    return () => {
      if (observer) {
        observer.disconnect()
      }
      if (animationFrameId) {
        window.cancelAnimationFrame(animationFrameId)
      }
    }
  }, [duration, value])

  return (
    <div ref={nodeRef} className={className}>
      {count}
      {suffix}
    </div>
  )
}

