import { Link, NavLink, Outlet } from "react-router"

import { appEnvironment } from "@/shared/config/env"
import { Button } from "@/components/ui/button"

const appNav = [{ to: "/app", label: "Dashboard" }]

function AppLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <div className="mx-auto flex w-full max-w-7xl gap-0">
        <aside className="hidden min-h-screen w-64 flex-col border-r border-border/70 bg-muted/20 md:flex">
          <div className="border-b border-border/70 px-5 py-5 text-base font-semibold tracking-tight">{appEnvironment.appName}</div>
          <nav aria-label="Application" className="flex flex-1 flex-col gap-1 p-3">
            {appNav.map((item) => (
              <Button key={item.to} asChild variant="ghost" className="justify-start">
                <NavLink to={item.to}>{item.label}</NavLink>
              </Button>
            ))}
          </nav>
        </aside>

        <div className="flex min-h-screen flex-1 flex-col">
          <header className="border-b border-border/70 bg-background/95 px-4 py-4 backdrop-blur supports-[backdrop-filter]:bg-background/80 md:px-6">
            <div className="flex items-center justify-between gap-3">
              <div className="flex items-center gap-2 md:hidden">
                {appNav.map((item) => (
                  <Button key={`${item.to}-mobile`} asChild variant="ghost" size="sm">
                    <NavLink to={item.to}>{item.label}</NavLink>
                  </Button>
                ))}
              </div>
              <div className="ml-auto flex items-center gap-2">
                <Button asChild variant="outline" size="sm">
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
