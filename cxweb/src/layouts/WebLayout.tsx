import { Outlet } from "react-router"

import WebHeader from "@/features/web/components/WebHeader"
import { appEnvironment } from "@/shared/config/env"

function WebLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <WebHeader />

      <main className="pb-8 md:pb-10">
        <Outlet />
      </main>

      <footer className="border-t border-border/70 bg-footer-bg text-footer-foreground">
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-3 px-4 py-8 text-sm md:flex-row md:items-center md:justify-between md:px-6">
          <p className="font-medium text-foreground">{appEnvironment.appName}</p>
          <p className="text-footer-foreground">Dynamic multi-tenant experience powered by tenant theme tokens.</p>
        </div>
      </footer>
    </div>
  )
}

export default WebLayout
