import { RouterProvider } from "react-router"

import { appRouter } from "@/routes/router"
import { ToastProvider } from "@/shared/components/ui/toast"
import { TenantThemeProvider } from "@/shared/theme"

function App() {
  return (
    <TenantThemeProvider>
      <RouterProvider router={appRouter} />
      <ToastProvider />
    </TenantThemeProvider>
  )
}

export default App
