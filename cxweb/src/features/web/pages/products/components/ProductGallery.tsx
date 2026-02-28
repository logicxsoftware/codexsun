type ProductGalleryProps = {
  productName: string
  images: string[]
  selectedImage: number
  onSelectImage: (index: number) => void
}

export default function ProductGallery({ productName, images, selectedImage, onSelectImage }: ProductGalleryProps) {
  const selectedImageUrl = images[selectedImage] ?? images[0] ?? null

  return (
    <div className="space-y-3">
      <div className="overflow-hidden rounded-xl border border-border bg-card">
        {selectedImageUrl ? (
          <img src={selectedImageUrl} alt={productName} className="h-[360px] w-full object-cover md:h-[460px]" loading="lazy" />
        ) : (
          <div className="flex h-[360px] w-full items-center justify-center text-sm text-muted-foreground md:h-[460px]">No image</div>
        )}
      </div>

      {images.length > 1 ? (
        <div className="grid grid-cols-4 gap-2">
          {images.map((image, index) => (
            <button
              type="button"
              key={`${image}-${index}`}
              onClick={() => onSelectImage(index)}
              className={`overflow-hidden rounded-lg border ${selectedImage === index ? "border-primary" : "border-border"}`}
            >
              <img src={image} alt={`${productName} ${index + 1}`} className="h-20 w-full object-cover" loading="lazy" />
            </button>
          ))}
        </div>
      ) : null}
    </div>
  )
}
