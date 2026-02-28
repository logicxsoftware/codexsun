import { createBrowserRouter } from "react-router"

import DashboardPage from "@/features/app/pages/DashboardPage"
import LoginPage from "@/features/auth/pages/LoginPage"
import MenuGroupMenusPage from "@/features/menu-admin/pages/MenuGroupMenusPage"
import MenuGroupsPage from "@/features/menu-admin/pages/MenuGroupsPage"
import MenuItemsPage from "@/features/menu-admin/pages/MenuItemsPage"
import ThemePreviewPage from "@/features/theme-preview/pages/ThemePreviewPage"
import UiTemplatePage from "@/features/ui-template/pages/UiTemplatePage"
import WebPage from "@/features/web/pages/WebPage"
import WebMenuBuilderPage from "@/features/web-navigation/pages/WebMenuBuilderPage"
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
    path: "/admin",
    element: <AppLayout />,
    errorElement: <RouteErrorBoundary />,
    children: [
      { index: true, element: <MenuGroupsPage /> },
      { path: "menu-groups", element: <MenuGroupsPage /> },
      { path: "menu-groups/:id/menus", element: <MenuGroupMenusPage /> },
      { path: "menus/:id/items", element: <MenuItemsPage /> },
      { path: "web-menu-builder", element: <WebMenuBuilderPage /> },
    ],
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
