import { useEffect, useState } from "react"
import type { ReactNode } from "react"

import { cn } from "@/lib/utils"

type NavStickyWrapperProps = {
  children: ReactNode
  sticky: boolean
  blur: boolean
  scrollShadow: boolean
  transparentOnTop: boolean
  borderBottom: boolean
  backgroundClassName: string
  foregroundClassName: string
  borderClassName: string
}

function NavStickyWrapper({
  children,
  sticky,
  blur,
  scrollShadow,
  transparentOnTop,
  borderBottom,
  backgroundClassName,
  foregroundClassName,
  borderClassName,
}: NavStickyWrapperProps) {
  const [isScrolled, setIsScrolled] = useState(false)

  useEffect(() => {
    const handleScroll = () => {
      setIsScrolled(window.scrollY > 8)
    }

    handleScroll()
    window.addEventListener("scroll", handleScroll, { passive: true })
    return () => {
      window.removeEventListener("scroll", handleScroll)
    }
  }, [])

  const transparentClass = transparentOnTop && !isScrolled ? "bg-transparent" : backgroundClassName

  return (
    <header
      className={cn(
        "z-40 w-full transition-all duration-200",
        sticky ? "sticky top-0" : "relative",
        transparentClass,
        foregroundClassName,
        blur ? "backdrop-blur supports-[backdrop-filter]:bg-header-bg/80" : "",
        borderBottom ? cn("border-b", borderClassName) : "",
        scrollShadow && isScrolled ? "shadow-sm" : "",
      )}
    >
      {children}
    </header>
  )
}

export default NavStickyWrapper
