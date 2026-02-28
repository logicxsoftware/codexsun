import { RouterProvider } from "react-router"
import { useEffect } from "react"

import { appRouter } from "@/routes/router"
import { ToastProvider } from "@/shared/components/ui/toast"
import { TenantThemeProvider } from "@/shared/theme"

const SCROLL_RESTORE_KEY = "cxweb:scroll-y"

function App() {
  useEffect(() => {
    if (typeof window === "undefined") {
      return
    }

    const restoreScroll = (): void => {
      const saved = window.sessionStorage.getItem(SCROLL_RESTORE_KEY)
      if (!saved) {
        return
      }

      const y = Number.parseInt(saved, 10)
      if (!Number.isFinite(y)) {
        return
      }

      // Defer once to let async sections/layout complete before restoring.
      window.requestAnimationFrame(() => {
        window.requestAnimationFrame(() => {
          window.scrollTo(0, y)
        })
      })
    }

    const saveScroll = (): void => {
      window.sessionStorage.setItem(SCROLL_RESTORE_KEY, String(window.scrollY))
    }

    restoreScroll()
    window.addEventListener("pagehide", saveScroll)
    window.addEventListener("beforeunload", saveScroll)
    return () => {
      window.removeEventListener("pagehide", saveScroll)
      window.removeEventListener("beforeunload", saveScroll)
    }
  }, [])

  return (
    <TenantThemeProvider>
      <RouterProvider router={appRouter} />
      <ToastProvider />
    </TenantThemeProvider>
  )
}

export default App
