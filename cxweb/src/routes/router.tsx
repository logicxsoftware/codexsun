import { createBrowserRouter } from "react-router"

import AppLayout from "@/layouts/AppLayout"
import AuthLayout from "@/layouts/AuthLayout"
import WebLayout from "@/layouts/WebLayout"
import DashboardPage from "@/features/app/pages/DashboardPage"
import LoginPage from "@/features/auth/pages/LoginPage"
import AboutPage from "@/features/web/pages/AboutPage"
import BlogPage from "@/features/web/pages/BlogPage"
import HomePage from "@/features/web/pages/HomePage"
import ServicesPage from "@/features/web/pages/ServicesPage"
import WebContactPage from "@/features/web/pages/WebContactPage"
import NotFoundPage from "@/shared/components/NotFoundPage"
import RouteErrorBoundary from "@/shared/components/RouteErrorBoundary"

export const appRouter = createBrowserRouter([
  {
    path: "/",
    element: <WebLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [
      { index: true, element: <HomePage /> },
      { path: "about", element: <AboutPage /> },
      { path: "services", element: <ServicesPage /> },
      { path: "blog", element: <BlogPage /> },
      { path: "contact", element: <WebContactPage /> },
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
