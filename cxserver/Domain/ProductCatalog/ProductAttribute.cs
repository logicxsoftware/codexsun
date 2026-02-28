namespace cxserver.Domain.ProductCatalog;

public sealed class ProductAttribute
{
    private ProductAttribute(Guid id, Guid productId, string key, string value)
    {
        Id = id;
        ProductId = productId;
        Key = key;
        Value = value;
    }

    private ProductAttribute()
    {
        Key = string.Empty;
        Value = string.Empty;
    }

    public Guid Id { get; private set; }
    public Guid ProductId { get; private set; }
    public string Key { get; private set; }
    public string Value { get; private set; }

    public Product Product { get; private set; } = default!;

    public static ProductAttribute Create(Guid id, Guid productId, string key, string value)
    {
        return new ProductAttribute(
            id,
            productId,
            key.Trim().ToLowerInvariant(),
            value.Trim());
    }

    public void Update(string key, string value)
    {
        Key = key.Trim().ToLowerInvariant();
        Value = value.Trim();
    }
}
