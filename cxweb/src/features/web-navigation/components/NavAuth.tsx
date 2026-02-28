import { useMemo } from "react"
import { Link, useNavigate } from "react-router"
import { ChevronDown } from "lucide-react"

import { Button } from "@/components/ui/button"
import { useAuth } from "@/shared/auth"
import type { HeaderAuthConfig } from "@/features/web-navigation/types/navigation-types"

type NavAuthProps = {
  config: HeaderAuthConfig
}

function NavAuth({ config }: NavAuthProps) {
  const { isAuthenticated, signOut } = useAuth()
  const navigate = useNavigate()

  const menuItems = useMemo(
    () => [
      { label: "Dashboard", to: config.dashboardPath },
      { label: "Profile", to: `${config.dashboardPath.replace(/\/$/, "")}/profile` },
      { label: "Settings", to: `${config.dashboardPath.replace(/\/$/, "")}/settings` },
    ],
    [config.dashboardPath],
  )

  if (!config.enabled) {
    return null
  }

  if (!isAuthenticated) {
    return (
      <Button asChild size="sm" className="h-9 bg-cta-bg px-3 text-cta-foreground hover:bg-cta-bg/90">
        <Link to={config.loginPath}>Login</Link>
      </Button>
    )
  }

  return (
    <div className="group relative">
      <Button size="sm" variant="outline" className="h-9 gap-1 border-border bg-background px-3 text-foreground hover:bg-menu-hover">
        Account
        <ChevronDown className="h-3.5 w-3.5" />
      </Button>
      <div className="invisible absolute right-0 top-full z-50 mt-2 w-44 rounded-md border border-border bg-card p-1 opacity-0 shadow-sm transition group-hover:visible group-hover:opacity-100">
        {menuItems.map((item) => (
          <Link key={item.label} to={item.to} className="block rounded-sm px-2 py-1.5 text-sm text-foreground hover:bg-menu-hover">
            {item.label}
          </Link>
        ))}
        <button
          type="button"
          className="block w-full rounded-sm px-2 py-1.5 text-left text-sm text-foreground hover:bg-menu-hover"
          onClick={() => {
            signOut()
            void navigate(config.loginPath)
          }}
        >
          Logout
        </button>
      </div>
    </div>
  )
}

export default NavAuth
