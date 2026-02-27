import { Toaster } from "sonner"

export function ToastProvider() {
  return (
    <Toaster
      closeButton={false}
      expand={false}
      position="top-right"
      richColors={false}
      toastOptions={{
        className: "pointer-events-auto",
        unstyled: true,
      }}
      visibleToasts={4}
    />
  )
}
