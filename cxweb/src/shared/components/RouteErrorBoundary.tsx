import { isRouteErrorResponse, Link, useRouteError } from "react-router"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function RouteErrorBoundary() {
  const error = useRouteError()

  const title = isRouteErrorResponse(error)
    ? `Request failed (${error.status})`
    : "Unexpected error"

  const description = isRouteErrorResponse(error)
    ? error.statusText
    : "An unexpected application error occurred."

  return (
    <div className="grid min-h-screen place-items-center px-4 py-10">
      <Card className="w-full max-w-xl">
        <CardHeader>
          <CardTitle>{title}</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-4 text-muted-foreground">
          <p>{description}</p>
          <div>
            <Button asChild>
              <Link to="/">Return home</Link>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default RouteErrorBoundary
