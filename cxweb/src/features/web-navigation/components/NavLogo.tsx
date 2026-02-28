import { Link } from "react-router"

import type { HeaderLogoConfig } from "@/features/web-navigation/types/navigation-types"
import { cn } from "@/lib/utils"

type NavLogoProps = {
  logo: HeaderLogoConfig
  homePath: string
}

const logoSizeClassMap: Record<HeaderLogoConfig["size"], string> = {
  small: "h-7 w-7 text-sm",
  medium: "h-9 w-9 text-base",
  large: "h-11 w-11 text-lg",
}

const toSvgDataUri = (svg: string): string => {
  const normalized = svg.replace(/[\n\r\t]+/g, " ").trim()
  return `data:image/svg+xml;utf8,${encodeURIComponent(normalized)}`
}

function NavLogo({ logo, homePath }: NavLogoProps) {
  const mediaClassName = cn("inline-flex items-center justify-center rounded-md bg-brand/10 font-semibold text-brand", logoSizeClassMap[logo.size])
  const wrapperClass = cn("inline-flex items-center gap-2", logo.textPosition === "below" ? "flex-col items-start gap-1" : "")

  return (
    <Link to={homePath} className={wrapperClass}>
      {logo.type === "text" ? (
        <span className={mediaClassName}>{logo.text.slice(0, 1).toUpperCase()}</span>
      ) : (
        <span className={mediaClassName}>
          <img
            src={logo.type === "inline-svg" && logo.svg ? toSvgDataUri(logo.svg) : logo.src}
            alt={logo.text}
            className="h-full w-full rounded-md object-contain"
          />
        </span>
      )}
      {logo.showText ? <span className="font-semibold tracking-tight text-foreground">{logo.text}</span> : null}
    </Link>
  )
}

export default NavLogo
