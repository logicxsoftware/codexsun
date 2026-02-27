import { useContext } from "react"

import { GlobalLoadingContext } from "@/shared/components/ui/GlobalLoader/GlobalLoadingProvider"

export const useGlobalLoading = () => {
  const context = useContext(GlobalLoadingContext)
  if (!context) {
    throw new Error("useGlobalLoading must be used within GlobalLoadingProvider")
  }

  return context
}
