import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function AboutPage() {
  return (
    <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
      <Card>
        <CardHeader>
          <CardTitle>About</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">
          Codexsun provides enterprise-grade modular systems with API-first delivery and long-term maintainability.
        </CardContent>
      </Card>
    </section>
  )
}

export default AboutPage
