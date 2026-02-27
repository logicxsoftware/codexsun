import { createBrowserRouter } from "react-router"

import DashboardPage from "@/features/app/pages/DashboardPage"
import LoginPage from "@/features/auth/pages/LoginPage"
import ThemePreviewPage from "@/features/theme-preview/pages/ThemePreviewPage"
import UiTemplatePage from "@/features/ui-template/pages/UiTemplatePage"
import WebPage from "@/features/web/pages/WebPage"
import AppLayout from "@/layouts/AppLayout"
import AuthLayout from "@/layouts/AuthLayout"
import WebLayout from "@/layouts/WebLayout"
import NotFoundPage from "@/shared/components/NotFoundPage"
import RouteErrorBoundary from "@/shared/components/RouteErrorBoundary"

export const appRouter = createBrowserRouter([
  {
    path: "/",
    element: <WebLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [
      { index: true, element: <WebPage defaultSlug="home" /> },
      { path: ":slug", element: <WebPage /> },
    ],
  },
  {
    path: "/app",
    element: <AppLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [{ index: true, element: <DashboardPage /> }],
  },
  {
    path: "/auth",
    element: <AuthLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [{ path: "login", element: <LoginPage /> }],
  },
  {
    path: "/theme-preview",
    element: import.meta.env.DEV ? <ThemePreviewPage /> : <NotFoundPage />,
  },
  {
    path: "/ui-template",
    element: <UiTemplatePage />,
  },
  {
    path: "*",
    element: <NotFoundPage />,
  },
])
