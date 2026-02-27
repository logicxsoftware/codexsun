import { createBrowserRouter } from "react-router"

import AppLayout from "@/layouts/AppLayout"
import AuthLayout from "@/layouts/AuthLayout"
import WebLayout from "@/layouts/WebLayout"
import DashboardPage from "@/features/app/pages/DashboardPage"
import LoginPage from "@/features/auth/pages/LoginPage"
import WebPage from "@/features/web/pages/WebPage"
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
    path: "*",
    element: <NotFoundPage />,
  },
])
