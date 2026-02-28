type FooterAboutProps = {
  enabled: boolean
  title: string
  content: string
}

function FooterAbout({ enabled, title, content }: FooterAboutProps) {
  if (!enabled) {
    return null
  }

  return (
    <section className="space-y-2">
      <h3 className="text-sm font-semibold text-foreground">{title}</h3>
      <p className="text-sm text-muted-foreground">{content}</p>
    </section>
  )
}

export default FooterAbout
