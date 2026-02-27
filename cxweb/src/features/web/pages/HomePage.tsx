import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { Link } from "react-router"

function HomePage() {
  return (
    <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
      <Card>
        <CardHeader>
          <CardTitle className="text-3xl">Welcome to Codexsun</CardTitle>
          <CardDescription>Frontend foundation aligned to API-first architecture and multi-tenant readiness.</CardDescription>
        </CardHeader>
        <CardContent className="flex flex-col gap-4 sm:flex-row">
          <Button asChild>
            <Link to="/app">Open dashboard</Link>
          </Button>
          <Button asChild variant="outline">
            <Link to="/auth/login">Sign in</Link>
          </Button>
        </CardContent>
      </Card>
    </section>
  )
}

export default HomePage
