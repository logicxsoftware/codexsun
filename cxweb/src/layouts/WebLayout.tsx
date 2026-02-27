import { Link, NavLink, Outlet } from "react-router"

import { appEnvironment } from "@/shared/config/env"
import { Button } from "@/components/ui/button"

const publicRoutes = [
  { to: "/", label: "Home" },
  { to: "/about", label: "About" },
  { to: "/services", label: "Services" },
  { to: "/blog", label: "Blog" },
  { to: "/contact", label: "Contact" },
]

function WebLayout() {
  return (
    <div className="min-h-screen bg-background text-foreground">
      <header className="border-b border-border/70 bg-background/95 backdrop-blur supports-[backdrop-filter]:bg-background/80">
        <div className="mx-auto flex w-full max-w-6xl items-center justify-between px-4 py-4 md:px-6">
          <Link to="/" className="text-lg font-semibold tracking-tight">
            {appEnvironment.appName}
          </Link>

          <nav aria-label="Primary" className="hidden items-center gap-2 md:flex">
            {publicRoutes.map((item) => (
              <Button key={item.to} asChild variant="ghost" size="sm">
                <NavLink to={item.to}>{item.label}</NavLink>
              </Button>
            ))}
          </nav>

          <div className="flex items-center gap-2">
            <Button asChild size="sm" variant="outline" className="hidden md:inline-flex">
              <Link to="/auth/login">Sign in</Link>
            </Button>
            <Button asChild size="sm">
              <Link to="/app">Open app</Link>
            </Button>
          </div>
        </div>

        <nav aria-label="Mobile" className="border-t border-border/70 px-4 py-2 md:hidden">
          <div className="mx-auto flex max-w-6xl flex-wrap items-center gap-1">
            {publicRoutes.map((item) => (
              <Button key={`${item.to}-mobile`} asChild variant="ghost" size="sm">
                <NavLink to={item.to}>{item.label}</NavLink>
              </Button>
            ))}
          </div>
        </nav>
      </header>

      <main>
        <Outlet />
      </main>

      <footer className="border-t border-border/70 bg-muted/30">
        <div className="mx-auto flex w-full max-w-6xl flex-col gap-2 px-4 py-8 text-sm text-muted-foreground md:flex-row md:items-center md:justify-between md:px-6">
          <p>{appEnvironment.appName} web experience</p>
          <p>Production-ready frontend foundation</p>
        </div>
      </footer>
    </div>
  )
}

export default WebLayout
