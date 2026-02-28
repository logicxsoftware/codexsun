import { Outlet } from "react-router"

import { WebFooter, WebNavigationHeader } from "@/features/web-navigation/components"
import { WebNavigationProvider } from "@/features/web-navigation/context/WebNavigationProvider"

function WebLayout() {
  return (
    <WebNavigationProvider>
      <div className="min-h-screen bg-background text-foreground">
        <WebNavigationHeader />

        <main className="pb-8 md:pb-10">
          <Outlet />
        </main>

        <WebFooter />
      </div>
    </WebNavigationProvider>
  )
}

export default WebLayout
