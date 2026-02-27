import { StrictMode } from "react"
import { createRoot } from "react-dom/client"

import App from "./App"
import { AuthProvider } from "@/shared/auth"
import { GlobalLoadingProvider } from "@/shared/components/ui/GlobalLoader"
import "./css/app.css"

const rootElement = document.getElementById("root")

if (!rootElement) {
  throw new Error("Root element was not found")
}

createRoot(rootElement).render(
  <StrictMode>
    <GlobalLoadingProvider>
      <AuthProvider>
        <App />
      </AuthProvider>
    </GlobalLoadingProvider>
  </StrictMode>
)
