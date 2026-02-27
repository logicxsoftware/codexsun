import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"

function BlogPage() {
  return (
    <section className="mx-auto w-full max-w-6xl px-4 py-16 md:px-6">
      <Card>
        <CardHeader>
          <CardTitle>Blog</CardTitle>
        </CardHeader>
        <CardContent className="text-muted-foreground">
          Technical updates, architecture decisions, and platform evolution notes are published here.
        </CardContent>
      </Card>
    </section>
  )
}

export default BlogPage
