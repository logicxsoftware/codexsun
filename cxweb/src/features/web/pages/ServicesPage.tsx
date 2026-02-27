import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function ServicesPage() {
  return (
    <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
      <Card>
        <CardHeader>
          <CardTitle>Services</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">
          Platform engineering, application modernization, and multi-tenant SaaS architecture implementation.
        </CardContent>
      </Card>
    </section>
  )
}

export default ServicesPage
