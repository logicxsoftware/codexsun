import type { BlogPostImageItem } from "@/features/blog/types/blog-types"

type ImageGalleryProps = {
  images: BlogPostImageItem[]
}

export default function ImageGallery({ images }: ImageGalleryProps) {
  if (images.length === 0) {
    return null
  }

  return (
    <div className="grid grid-cols-1 gap-3 sm:grid-cols-2">
      {images.map((image) => (
        <figure key={image.id} className="overflow-hidden rounded-xl border border-border bg-card">
          <img src={image.imagePath} alt={image.altText ?? "Blog image"} className="h-48 w-full object-cover" loading="lazy" />
          {image.caption ? <figcaption className="px-3 py-2 text-xs text-muted-foreground">{image.caption}</figcaption> : null}
        </figure>
      ))}
    </div>
  )
}
