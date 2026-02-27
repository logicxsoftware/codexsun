import { Link } from "react-router"

import { Button } from "@/components/ui/button"
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function NotFoundPage() {
  return (
    <div className="grid min-h-[70vh] place-items-center px-4 py-10">
      <Card className="w-full max-w-lg">
        <CardHeader>
          <CardTitle>Page not found</CardTitle>
        </CardHeader>
        <CardContent className="flex flex-col gap-4 text-muted-foreground">
          <p>The requested route is not available.</p>
          <div>
            <Button asChild>
              <Link to="/">Go to home</Link>
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  )
}

export default NotFoundPage
