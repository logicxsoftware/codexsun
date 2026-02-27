import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function WebContactPage() {
  return (
    <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
      <Card>
        <CardHeader>
          <CardTitle>Contact</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">
          Reach the delivery team through the designated enterprise communication channel for onboarding and support.
        </CardContent>
      </Card>
    </section>
  )
}

export default WebContactPage
