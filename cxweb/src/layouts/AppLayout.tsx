import { Link, NavLink, Outlet } from "react-router"

import { Button } from "@/components/ui/button"
import { appEnvironment } from "@/shared/config/env"

const appNav = [{ to: "/app", label: "Dashboard" }]

function AppLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto flex w-full max-w-7xl gap-0">
        <aside className="hidden min-h-screen w-64 flex-col border-r border-sidebar-border bg-sidebar text-sidebar-foreground md:flex">
          <div className="border-b border-sidebar-border px-5 py-5 text-base font-semibold tracking-tight text-sidebar-foreground">
            {appEnvironment.appName}
          </div>
          <nav aria-label="Application" className="flex flex-1 flex-col gap-1 p-3">
            {appNav.map((item) => (
              <Button key={item.to} asChild variant="ghost" className="justify-start text-sidebar-foreground hover:bg-sidebar-accent hover:text-sidebar-accent-foreground">
                <NavLink to={item.to}>{item.label}</NavLink>
              </Button>
            ))}
          </nav>
        </aside>

        <div className="flex min-h-screen flex-1 flex-col">
          <header className="border-b border-border/70 bg-header-bg px-4 py-4 text-header-foreground backdrop-blur supports-[backdrop-filter]:bg-header-bg/80 md:px-6">
            <div className="flex items-center justify-between gap-3">
              <div className="flex items-center gap-2 md:hidden">
                {appNav.map((item) => (
                  <Button key={`${item.to}-mobile`} asChild variant="ghost" size="sm" className="text-header-foreground hover:bg-menu-hover">
                    <NavLink to={item.to}>{item.label}</NavLink>
                  </Button>
                ))}
              </div>
              <div className="ml-auto flex items-center gap-2">
                <Button asChild variant="outline" size="sm" className="border-border bg-background text-foreground hover:bg-menu-hover">
                  <Link to="/">Website</Link>
                </Button>
              </div>
            </div>
          </header>

          <main className="flex-1 p-4 md:p-6">
            <Outlet />
          </main>
        </div>
      </div>
    </div>
  )
}

export default AppLayout
