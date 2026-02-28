namespace cxserver.Domain.ProductCatalog;

public sealed class ProductImage
{
    private ProductImage(Guid id, Guid productId, string imageUrl, int order)
    {
        Id = id;
        ProductId = productId;
        ImageUrl = imageUrl;
        Order = order;
    }

    private ProductImage()
    {
        ImageUrl = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string ImageUrl { get; private set; }
    public int Order { get; private set; }

    public Product Product { get; private set; } = default!;

    public static ProductImage Create(Guid id, Guid productId, string imageUrl, int order)
    {
        return new ProductImage(id, productId, imageUrl.Trim(), order);
    }

    public void Update(string imageUrl, int order)
    {
        ImageUrl = imageUrl.Trim();
        Order = order;
    }
}
