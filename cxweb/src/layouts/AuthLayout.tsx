import { Outlet } from "react-router"

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function AuthLayout() {
  return (
    <div className="grid min-h-screen place-items-center bg-muted/30 px-4 py-10">
      <Card className="w-full max-w-md">
        <CardHeader>
          <CardTitle className="text-xl">Authentication</CardTitle>
        </CardHeader>
        <CardContent>
          <Outlet />
        </CardContent>
      </Card>
    </div>
  )
}

export default AuthLayout
